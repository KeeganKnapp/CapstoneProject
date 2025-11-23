using CapstoneAPI.Data;
using CapstoneAPI.Models;
using CapstoneAPI.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CapstoneAPI.Controllers
{
    [ApiController]
    [Route("time-entries")]
    public class TimeEntriesController : ControllerBase
    {
        private readonly CapstoneDbContext _db;
        public TimeEntriesController(CapstoneDbContext db) => _db = db;

        // POST /time-entries/clock-in
        [HttpPost("clock-in")]
        public async Task<ActionResult<TimeEntry>> ClockIn([FromBody] ClockInRequest req, CancellationToken ct)
        {
            if (req is null) return BadRequest("Request body is required.");
            if (req.UserId <= 0) return BadRequest("UserId must be positive.");

            var start = req.StartTime ?? DateTimeOffset.UtcNow;

            var entry = new TimeEntry
            {
                UserId   = req.UserId,
                AssignmentId = req.AssignmentId,
                StartTime    = start,
                EndTime      = null
            };

            _db.TimeEntries.Add(entry);
            await _db.SaveChangesAsync(ct);

            return Created(string.Empty, entry);
        }

        // POST /time-entries/clock-out
        [HttpPost("clock-out")]
        public async Task<ActionResult<TimeEntry>> ClockOut([FromBody] ClockOutRequest req, CancellationToken ct)
        {
            if (req is null) return BadRequest("Request body is required.");

            if (req.TimeEntryId.HasValue && req.TimeEntryId.Value > 0)
            {
                var entryById = await _db.TimeEntries
                    .FirstOrDefaultAsync(t => t.TimeEntryId == req.TimeEntryId.Value, ct);

                if (entryById is null)
                    return NotFound(new { error = "Time entry not found." });

                entryById.EndTime = req.EndTime ?? DateTimeOffset.UtcNow;
                await _db.SaveChangesAsync(ct);
                return Ok(entryById);
            }

           
            if (!req.UserId.HasValue || req.UserId.Value <= 0)
                return BadRequest("Provide TimeEntryId or a positive UserId.");

            var openEntry = await _db.TimeEntries
                .Where(t => t.UserId == req.UserId.Value && t.EndTime == null)
                .OrderByDescending(t => t.StartTime)
                .FirstOrDefaultAsync(ct);

            if (openEntry is null)
                return NotFound(new { error = "No open time entry found for this user." });

            openEntry.EndTime = req.EndTime ?? DateTimeOffset.UtcNow;
            await _db.SaveChangesAsync(ct);
            return Ok(openEntry);
        }

        // GET /time-entries/history
        [HttpGet("history")]
        [Authorize]
        public async Task<ActionResult<TimeEntry>> History()
        {
           var q = _db.TimeEntries.AsQueryable();

            var items = await q
                .OrderByDescending(x => x.StartTime)
                .Select(x => new TimeEntryDto(x.TimeEntryId, x.UserId, x.AssignmentId, x.StartTime, x.EndTime))
                .ToListAsync();
        
            return Ok(items);
        }

        // GET /time-entries/active
        [HttpGet("active")]
        [Authorize]
        public async Task<ActionResult<TimeEntry>> Active()
        {
           var q = _db.TimeEntries.AsQueryable();

            // If EndTime == null assume employee is clocked in
            q = q.Where(x => x.EndTime == null);

            var items = await q
                .OrderByDescending(x => x.StartTime)
                .Select(x => new TimeEntryDto(x.TimeEntryId, x.UserId, x.AssignmentId, x.StartTime, x.EndTime))
                .ToListAsync();
        
            return Ok(items);
        }
    }
}