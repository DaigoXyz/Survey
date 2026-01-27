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
            if (user is null)
                throw new UnauthorizedAccessException("Username / Password salah.");

            var ok = _passwordService.Verify(
                user.PasswordHash,
                dto.Password,
                user
            );

            if (!ok)
                throw new UnauthorizedAccessException("Username / Password salah.");

            return new LoginResponseDto
            {
                UserId = user.Id,
                Username = user.Username,
                Role = user.Role.Name
            };
        }
    }
}