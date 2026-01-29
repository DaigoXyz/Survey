using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Survey.DTOs.User;
using Survey.DTOs.Document;

namespace Survey.Services.User
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsersAsync();
        Task<UserDto> CreateUserAsync(int supervisorId, CreateUserDto dto);
        Task<List<UserDto>> GetUserAsync(int supervisorId);
        Task<RequesterProfileDto> GetRequesterProfileAsync(int userId);
        Task<UserDto> UpdateUserAsync(int supervisorId, UpdateUserDto dto);
        Task DeleteUserAsync(int supervisorId, int userId);

    }
}