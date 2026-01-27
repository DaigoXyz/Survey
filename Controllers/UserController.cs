using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Survey.DTOs.User;
using Survey.Services.User;
using System.Security.Claims;


namespace Survey.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService) => _userService = userService;

        [Authorize(Roles = "Supervisor")]
        [HttpPost("user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            var supervisorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            try
            {
                var res = await _userService.CreateUserAsync(supervisorId, dto);
                return Ok(res);
            }
            catch (Exception ex)
            {
                // kamu bisa bikin global exception middleware kalau mau lebih clean
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Supervisor")]
        [HttpGet("user")]
        public async Task<IActionResult> GetUser()
        {
            var supervisorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var res = await _userService.GetUserAsync(supervisorId);
            return Ok(res);
        }
    }
}