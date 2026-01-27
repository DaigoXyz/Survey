using Survey.Components;
using Survey.Data;
using Survey.Repositories.IRepositories.IUserRepository;
using Survey.Repositories;
using Survey.Services.Auth;
using Survey.Services.User;
using Survey.Services.Password;
using Survey.Mappers.IMappers;
using Survey.Mappers;
using Survey.Repositories.RoleRepository;
using Survey.Seeders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers(); 
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    );
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IPositionRepository, PositionRepository>();
builder.Services.AddScoped<IUserRelationRepository, UserRelationRepository>();

builder.Services.AddScoped<IUserMapper, UserMapper>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwt = builder.Configuration.GetSection("Jwt");

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwt["Issuer"],
        ValidAudience = jwt["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleRepo = services.GetRequiredService<IRoleRepository>();
    var positionRepo = services.GetRequiredService<IPositionRepository>();
    var userRepo = services.GetRequiredService<IUserRepository>();
    var passwordService = services.GetRequiredService<IPasswordService>();

    var roleSeeder = new RoleSeeder(roleRepo);
    await roleSeeder.SeedAsync();

    var positionSeeder = new PositionSeeder(positionRepo);
    await positionSeeder.SeedAsync();

    var supervisorSeeder = new SupervisorSeeder(
        userRepo,
        roleRepo,
        positionRepo,
        passwordService
    );

    await supervisorSeeder.SeedAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapControllers();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
