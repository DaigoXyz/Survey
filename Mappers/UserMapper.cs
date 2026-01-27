using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Survey.Mappers.IMappers;
using Survey.DTOs.User;
using Survey.Entities;

namespace Survey.Mappers
{
    public class UserMapper : IUserMapper
    {
        public UserDto ToDto(User entity) => new UserDto
        {
            Id = entity.Id,
            Username = entity.Username,
            Role = entity.Role?.Name ?? "",
            PositionId = entity.PositionId,
            PositionName = entity.PositionName
        };
    }
}