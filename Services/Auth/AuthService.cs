using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Survey.DTOs.Auth;
using Survey.Services.Password;
using Survey.Repositories.IRepositories.IUserRepository;
using Microsoft.IdentityModel.Tokens;

namespace Survey.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IConfiguration _config;
        private readonly IPasswordService _passwordService;

        public AuthService(
            IUserRepository userRepo,
            IConfiguration config,
            IPasswordService passwordService)
        {
            _userRepo = userRepo;
            _config = config;
            _passwordService = passwordService;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
        {
            var user = await _userRepo.GetByUsernameAsync(dto.Username);
            if (user is null) throw new UnauthorizedAccessException("Username/Password salah.");

            var ok = _passwordService.Verify(
                user.PasswordHash,
                dto.Password,
                user
            );

            if (!ok)
                throw new UnauthorizedAccessException("Username / Password salah");

            var token = GenerateJwt(user.Id, user.Username, user.Role.Name);

            return new LoginResponseDto
            {
                AccessToken = token,
                UserId = user.Id,
                Username = user.Username,
                Role = user.Role.Name
            };
        }

        private string GenerateJwt(int userId, string username, string role)
        {
            var jwt = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(jwt["ExpiresMinutes"]!)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}