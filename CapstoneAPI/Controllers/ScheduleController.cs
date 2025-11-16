using CapstoneAPI.Data;
using CapstoneAPI.DTOs;
using CapstoneAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CapstoneAPI.Controllers
{
    [ApiController]
    [Route("schedule")]
    public class ScheduleController : ControllerBase
    {
        private readonly CapstoneDbContext _db;
        public ScheduleController(CapstoneDbContext db) => _db = db;

        private int GetUserId()
        {
            // try common claim types in order
            var id = User.FindFirstValue("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");

            if (id is null) throw new UnauthorizedAccessException("UserId claim missing.");
            return int.Parse(id);
        }

        // GET /schedule/getAll
        [HttpGet("getAll")]
        [Authorize]
        public async Task<ActionResult<ScheduleEntry>> History()
        {
           var q = _db.ScheduleEntry.AsQueryable();

            var items = await q
                .Select(x => new ScheduleEntryDto(x.ScheduleEntryId, x.UserId, x.AssignmentId, x.StartTime, x.EndTime))
                .ToListAsync();
        
            return Ok(items);
        }

        // POST /schedule/createSchedule
        [HttpPost("createSchedule")]
        [Authorize]
        public async Task<ActionResult<ScheduleEntryDto>> Create([FromBody] ScheduleEntryCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            if (dto.StartTime > dto.EndTime) return BadRequest("StartTime must be <= EndTime.");

            var entity = new ScheduleEntry
            {
                UserId = dto.UserId,
                AssignmentId = dto.AssignmentId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
            };

            _db.ScheduleEntry.Add(entity);
            await _db.SaveChangesAsync();

            var result = new ScheduleEntryDto(entity.ScheduleEntryId, entity.UserId, entity.AssignmentId, entity.StartTime, entity.EndTime);
            return CreatedAtAction(nameof(GetMineById), new { id = entity.ScheduleEntryId }, result);
        }

        // GET /schedule/{id}   (self-only)
        [HttpGet("schedule/self/{id:long}")]
        [Authorize]
        public async Task<ActionResult<ScheduleEntryDto>> GetMineById(long id)
        {
            var me = GetUserId();
            var entity = await _db.ScheduleEntry.AsNoTracking()
                .FirstOrDefaultAsync(r => r.ScheduleEntryId == id && r.UserId == me);

            if (entity is null) return NotFound();

            return new ScheduleEntryDto(entity.ScheduleEntryId, entity.UserId, entity.AssignmentId, entity.StartTime, entity.EndTime);
        }

        // GET /schedule/{id}
        [HttpGet("schedule/job/{id:int}")]
        [Authorize]
        public async Task<ActionResult<ScheduleEntryDto>> byJob(int id)
        {
           var q = _db.ScheduleEntry.AsQueryable();

            // If EndTime == null assume employee is clocked in
            q = q.Where(x => x.AssignmentId == id);

            var items = await q
                .OrderByDescending(x => x.StartTime)
                .Select(x => new ScheduleEntryDto(x.ScheduleEntryId, x.UserId, x.AssignmentId, x.StartTime, x.EndTime))
                .ToListAsync();
        
            return Ok(items);
        }

        // DELETE /api/requestoff/{id} 
        [HttpDelete("delete/{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(long id)
        {
            var entity = await _db.ScheduleEntry
                .FirstOrDefaultAsync(r => r.ScheduleEntryId == id);

            if (entity is null) return NotFound();

            _db.ScheduleEntry.Remove(entity);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}