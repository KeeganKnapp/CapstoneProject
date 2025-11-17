using CapstoneAPI.Data;
using CapstoneAPI.Models;
using CapstoneAPI.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CapstoneAPI.Controllers
{
    [ApiController]
    [Route("time-entries")]
    [Authorize]
    public class TimeEntriesController : ControllerBase
    {
        private readonly CapstoneDbContext _db;
        public TimeEntriesController(CapstoneDbContext db) => _db = db;

        // added helper to get authenticated user's id from JWT claims
        private int GetUserId()
        {
            var raw =
                User.FindFirstValue("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue("sub");

            if (string.IsNullOrWhiteSpace(raw))
            {
                throw new UnauthorizedAccessException("UserId claim missing");
            }

            return int.Parse(raw);
        }

        // POST /time-entries/clock-in
        [HttpPost("clock-in")]
        public async Task<ActionResult<TimeEntry>> ClockIn([FromBody] ClockInRequest req, CancellationToken ct)
        {
            if (req is null) return BadRequest("Request body is required");

            var start = req.StartTime ?? DateTimeOffset.UtcNow;

            // UserId is now binded from the authenticated user and not from the request body
            var me = GetUserId();

            var entry = new TimeEntry
            {
                UserId   = me,
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
            if (req is null) return BadRequest("Request body is required");

            var me = GetUserId();

            // explicit TimeEntryId
            if (req.TimeEntryId.HasValue && req.TimeEntryId.Value > 0)
            {
                var entryById = await _db.TimeEntries
                    .FirstOrDefaultAsync(t => t.TimeEntryId == req.TimeEntryId.Value, ct);

                if (entryById is null)
                    return NotFound(new { error = "Time entry not found." });

                if (entryById.UserId != me)
                {
                    return Forbid();
                }

                entryById.EndTime = req.EndTime ?? DateTimeOffset.UtcNow;
                await _db.SaveChangesAsync(ct);
                return Ok(entryById);
            }

            // no longer accepting arbitrary UserId in the body
            //
            // if (!req.UserId.HasValue || req.UserId.Value <= 0)
            //     return BadRequest("Provide TimeEntryId or a positive UserId.");

            var openEntry = await _db.TimeEntries
                .Where(t => t.UserId == me && t.EndTime == null)
                .OrderByDescending(t => t.StartTime)
                .FirstOrDefaultAsync(ct);

            if (openEntry is null)
                return NotFound(new { error = "No open time entry found for this user." });

            openEntry.EndTime = req.EndTime ?? DateTimeOffset.UtcNow;
            await _db.SaveChangesAsync(ct);
            return Ok(openEntry);
        }
    }
}