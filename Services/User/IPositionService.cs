using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Survey.DTOs.User;

namespace Survey.Services.User
{
    public interface IPositionService
    {
        Task<List<PositionDto>> GetAllAsync();
        Task<string?> GetPositionNameAsync(int positionId);

    }

}