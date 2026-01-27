using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Survey.DTOs.User;

namespace Survey.Services.User
{
    public interface IUserService
    {
        Task<UserDto> CreateUserAsync(int supervisorId, CreateUserDto dto);
        Task<List<UserDto>> GetUserAsync(int supervisorId);
    }
}