using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Survey.DTOs.User;
using Survey.Entities;

namespace Survey.Mappers.IMappers
{
    public interface IUserMapper
    {
        UserDto ToDto(User Entity);
    }
}