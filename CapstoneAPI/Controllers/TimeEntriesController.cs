using CapstoneAPI.Data;             
using CapstoneAPI.Models;            
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;


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
            var entry = new TimeEntry
            {
                EmployeeId = req.EmployeeId,
                AssignmentId = req.AssignmentId,
                StartTime = req.StartTime,
                EndTime = req.EndTime
            };

            _db.Set<TimeEntry>().Add(entry);
            await _db.SaveChangesAsync(ct);
            return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status201Created, entry);
        }

        [HttpPost("clock-out")]
        public async System.Threading.Tasks.Task<ActionResult<TimeEntry>> ClockOut([FromBody] ClockOutRequest req, System.Threading.CancellationToken ct)
        {
            var entry = await _db.Set<TimeEntry>()
                .Where(t => t.EmployeeId && t.EndTimeUtc == null)
                .OrderByDescending(t => t.StartTimeUtc)
                .FirstOrDefaultAsync(ct);


            if (entry is null)
                return NotFound(new { error = "No open time entry found for this employee." });

            entry.EndTimeUtc = System.DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);

            return DayOfWeek(entry);
        }

        
        public record ClockInRequest(int EmployeeId, int? AssignmentId);
        public record ClockOutRequest(int EmployeeId);
        
    }
}