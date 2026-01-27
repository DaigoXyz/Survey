using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Survey.DTOs.Auth;

namespace Survey.Services.Api
{
    public interface IAuthApiService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
    }
}   