using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Survey.DTOs.User;
using Survey.Services.User;
using Survey.Repositories.IRepositories.IUserRepository;
using System.Security.Claims;

namespace Survey.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepo;

        public UsersController(IUserService userService, IUserRepository userRepo)
        {
            _userService = userService;
            _userRepo = userRepo;
        }

        [Authorize(Roles = "Supervisor")]
        [HttpGet("all")]
        public async Task<ActionResult<List<UserDto>>> GetAllUsers()
        {
            var res = await _userService.GetAllUsersAsync();
            return Ok(res);
        }
        [Authorize(Roles = "Supervisor")]
        [HttpPost("user")]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto dto)
        {
            var supervisorId = GetUserIdFromClaims();
            if (supervisorId <= 0)
                return Unauthorized("SupervisorId tidak ditemukan di token.");

            var created = await _userService.CreateUserAsync(supervisorId, dto);
            return Ok(created);
        }

        [Authorize(Roles = "Supervisor")]
        [HttpGet("user")]
        public async Task<IActionResult> GetUser()
        {
            var supervisorId = GetUserIdFromClaims();
            var res = await _userService.GetUserAsync(supervisorId);
            return Ok(res);
        }

        [Authorize]
        [HttpGet("supervisor/{id:int}")]
        public async Task<ActionResult<SuperVisorInfoDto>> GetSupervisorInfo(int id)
        {
            var supervisor = await _userRepo.GetByIdWithRoleAsync(id);
            if (supervisor is null)
                return NotFound("Supervisor tidak ditemukan.");

            if (!supervisor.Role.Name.Equals("Supervisor", StringComparison.OrdinalIgnoreCase))
                return BadRequest("User ini bukan Supervisor.");

            return Ok(new SuperVisorInfoDto
            {
                Id = supervisor.Id,
                Username = supervisor.Username,
                PositionId = supervisor.PositionId,
                PositionName = supervisor.PositionName
            });
        }

        [Authorize]
        [HttpGet("supervisors")]
        public async Task<ActionResult<List<SuperVisorInfoDto>>> GetSupervisors()
        {
            var list = await _userRepo.GetAllSupervisorsAsync();

            return Ok(list.Select(s => new SuperVisorInfoDto
            {
                Id = s.Id,
                Username = s.Username,
                PositionId = s.PositionId,
                PositionName = s.PositionName
            }).ToList());
        }

        [Authorize(Roles = "Supervisor")]
        [HttpPut("user/{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            if (id != dto.Id)
                return BadRequest("Id mismatch.");

            var supervisorId = GetUserIdFromClaims();
            var res = await _userService.UpdateUserAsync(supervisorId, dto);
            return Ok(res);
        }

        private int GetUserIdFromClaims()
        {
            var idStr =
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub")
                ?? User.FindFirstValue("id");

            return int.TryParse(idStr, out var id) ? id : 0;
        }
    }
}
