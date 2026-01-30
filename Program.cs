using System.Security.Claims;
using Survey.Components;
using Survey.Data;
using Survey.Repositories;
using Survey.Repositories.IRepositories.IUserRepository;
using Survey.Repositories.RoleRepository;
using Survey.Repositories.IRepositories.ISurveyRepository;
using Survey.Services.Auth;
using Survey.Services.User;
using Survey.Services.Password;
using Survey.Services.Survey;
using Survey.Mappers;
using Survey.Mappers.IMappers;
using Survey.Seeders;
using Survey.Helpers;
using Survey.DTOs.Auth;
using Survey.Services.Document;
using Survey.Repositories.IRepositories.IDocumentRepository;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// DB
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped(sp =>
{
    var accessor = sp.GetRequiredService<IHttpContextAccessor>();
    var ctx = accessor.HttpContext;

    var baseUrl = ctx != null
        ? $"{ctx.Request.Scheme}://{ctx.Request.Host}/"
        : builder.Configuration["AppBaseUrl"] ?? "https://localhost:5191/";

    return new HttpClient { BaseAddress = new Uri(baseUrl) };
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/";
        o.LogoutPath = "/logout";
        o.AccessDeniedPath = "/dashboard";
        o.SlidingExpiration = true;
        o.ExpireTimeSpan = TimeSpan.FromHours(8);

        o.Cookie.Name = "SurveyAuth";
        o.Cookie.HttpOnly = true;
        o.Cookie.SameSite = SameSiteMode.Lax;
        o.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

//Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<ISurveyTemplateService, SurveyTemplateService>();
builder.Services.AddScoped<IPositionService, PositionService>();
builder.Services.AddScoped<IDocumentSurveyService, DocumentSurveyService>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IPositionRepository, PositionRepository>();
builder.Services.AddScoped<IUserRelationRepository, UserRelationRepository>();
builder.Services.AddScoped<ISurveyHeaderRepository, SurveyHeaderRepository>();
builder.Services.AddScoped<ISurveyItemRepository, SurveyItemRepository>();
builder.Services.AddScoped<IDocumentSurveyRepository, DocumentSurveyRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Mappers
builder.Services.AddScoped<IUserMapper, UserMapper>();

var app = builder.Build();

// SEEDING
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleRepo = services.GetRequiredService<IRoleRepository>();
    var positionRepo = services.GetRequiredService<IPositionRepository>();
    var userRepo = services.GetRequiredService<IUserRepository>();
    var passwordService = services.GetRequiredService<IPasswordService>();

    await new RoleSeeder(roleRepo).SeedAsync();
    await new PositionSeeder(positionRepo).SeedAsync();

    await new SupervisorSeeder(userRepo, roleRepo, positionRepo, passwordService).SeedAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();


app.MapPost("/auth/login", async (HttpContext http, IAuthService auth) =>
{
    try
    {
        var form = await http.Request.ReadFormAsync();
        var username = form["Username"].ToString();
        var password = form["Password"].ToString();

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return Results.Redirect("/?error=Username+dan+Password+harus+diisi");
        }

        var result = await auth.LoginAsync(new LoginRequestDto
        {
            Username = username,
            Password = password
        });

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, result.UserId.ToString()),
            new Claim(ClaimTypes.Name, result.Username),
            new Claim(ClaimTypes.Role, result.Role),
            new Claim("PositionId", result.PositionId.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await http.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            });

        return Results.Redirect("/dashboard");
    }
    catch (UnauthorizedAccessException ex)
    {
        return Results.Redirect("/?error=Username+atau+Password+salah");
    }
    catch (Exception ex)
    {
        return Results.Redirect($"/?error={Uri.EscapeDataString(ex.Message)}");
    }
});

app.MapGet("/auth/logout", async (HttpContext http) =>
{
    await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/");
});

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();