using Microsoft.AspNetCore.Mvc;
using Survey.DTOs.Auth;
using Survey.DTOs.Common;
using Survey.Services.Auth;
using System.Text;

namespace Survey.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login()
        {
            // Baca raw body apa adanya
            Request.EnableBuffering();

            using var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true);
            var raw = await reader.ReadToEndAsync();
            Request.Body.Position = 0;

            Console.WriteLine("=== RAW REQUEST BODY ===");
            Console.WriteLine(raw);
            Console.WriteLine("========================");

            // Kalau kosong -> problem di client (Blazor)
            if (string.IsNullOrWhiteSpace(raw))
                return BadRequest("Body kosong (dari client)");

            // Coba deserialize manual
            var dto = System.Text.Json.JsonSerializer.Deserialize<LoginRequestDto>(
                raw,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            Console.WriteLine($"DTO => U='{dto?.Username}' P='{dto?.Password}'");

            if (dto is null || string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("JSON tidak sesuai (Username/Password kosong)");

            var res = await _authService.LoginAsync(dto);
            return Ok(ApiResponse<LoginResponseDto>.Ok(res, "Login berhasil"));
        }
    }
}
