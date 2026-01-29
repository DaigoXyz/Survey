using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Survey.Repositories.IRepositories.IUserRepository;
using Survey.DTOs.User;

namespace Survey.Services.User
{
    public class PositionService : IPositionService
    {
        private readonly IPositionRepository _repo;

        public PositionService(IPositionRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<PositionDto>> GetAllAsync()
        {
            var entities = await _repo.GetAllAsync();
            return entities
                .Select(p => new PositionDto(p.Id, p.Name))
                .ToList();
        }
        public async Task<string?> GetPositionNameAsync(int positionId)
        {
            var pos = await _repo.GetByIdAsync(positionId);
            return pos?.Name;
        }

    }
}