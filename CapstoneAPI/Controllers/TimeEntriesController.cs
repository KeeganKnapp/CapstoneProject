using CapstoneAPI.Data;
using CapstoneAPI.Models;
using CapstoneAPI.Dtos;
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
            if (req.EmployeeId <= 0) return BadRequest("EmployeeId must be positive.");

            var start = req.StartTime ?? DateTimeOffset.UtcNow;

            var entry = new TimeEntry
            {
                EmployeeId   = req.EmployeeId,
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

            // 1) Close a specific entry if a TimeEntryId is provided
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

            // 2) Otherwise, close the most recent open entry for an employee
            if (!req.EmployeeId.HasValue || req.EmployeeId.Value <= 0)
                return BadRequest("Provide TimeEntryId or a positive EmployeeId.");

            var openEntry = await _db.TimeEntries
                .Where(t => t.EmployeeId == req.EmployeeId.Value && t.EndTime == null)
                .OrderByDescending(t => t.StartTime)
                .FirstOrDefaultAsync(ct);

            if (openEntry is null)
                return NotFound(new { error = "No open time entry found for this employee." });

            openEntry.EndTime = req.EndTime ?? DateTimeOffset.UtcNow;
            await _db.SaveChangesAsync(ct);
            return Ok(openEntry);
        }
    }
}