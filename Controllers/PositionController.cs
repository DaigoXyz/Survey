using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Survey.Data;
using Survey.DTOs.User;

namespace Survey.Controllers
{
    [ApiController]
    [Route("api/positions")]
    public class PositionsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public PositionsController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<List<PositionDto>>> GetAll(CancellationToken ct)
        {
            var data = await _db.Positions
                .AsNoTracking()
                .OrderBy(p => p.Id)
                .Select(p => new PositionDto(p.Id, p.Name)) // sesuaikan field: Name/PositionName
                .ToListAsync(ct);

            return Ok(data);
        }
    }
}