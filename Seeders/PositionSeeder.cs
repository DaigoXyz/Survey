using Survey.Repositories.IRepositories.IUserRepository;

namespace Survey.Seeders
{
    public class PositionSeeder
    {
        private readonly IPositionRepository _positionRepo;

        public PositionSeeder(IPositionRepository positionRepo)
        {
            _positionRepo = positionRepo;
        }

        public async Task SeedAsync()
        {
            // idempotent
            if (await _positionRepo.AnyAsync())
                return;

            await _positionRepo.AddDefaultAsync();
            await _positionRepo.SaveChangesAsync();
        }
    }
}
