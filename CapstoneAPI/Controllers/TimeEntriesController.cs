// Controllers/TimeEntriesController.cs

using CapstoneAPI.Data;              
using CapstoneAPI.Dtos;              
using CapstoneAPI.Models;            
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CapstoneAPI.Controllers;

[ApiController]

[Route("api/[controller]")] 
public class TimeEntriesController : ControllerBase
{
    private readonly CapstoneDbContext _db;
    public TimeEntriesController(CapstoneDbContext db) => _db = db;

    /*
    private bool AttemptEmployeeId(out int employeeId)
    {
        employeeId = 0;
        var raw = Request.Headers["X-Employee-Id"].FirstOrDefault();
        return int.TryParse(raw, out employeeId) && employeeId > 0;
    }
    */

    [HttpPost("clock-in")]
    public async Task<ActionResult<TimeEntryDto>> ClockIn([FromBody] ClockRequest req, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        if (!TryGetEmployeeId(out var employeeId))
            return BadRequest(new { code = "MissingEmployeeId", message = "Provide X-Employee-Id header (int)." });

        // an employee may have at most one open entry (endTime IS NULL).
        var open = await _db.TimeEntries
            .Where(t => t.EmployeeId == employeeId && t.EndTime == null)
            .FirstOrDefaultAsync(ct);

        if (open is not null)
            return Conflict(new { code = "AlreadyClockedIn", message = "Employee already has an open time entry." });

        var nowUtc = DateTime.UtcNow;

        var entity = new TimeEntry
        {
            EmployeeId = employeeId,
            AssignmentId = req.AssignmentId, // which jobsite/assignment
            StartTime = nowUtc,
            EndTime = null
        };

        _db.TimeEntries.Add(entity);
        await _db.SaveChangesAsync(ct);

        var dto = new TimeEntryDto(
            entity.TimeEntryId,
            entity.EmployeeId,
            entity.AssignmentId,
            entity.StartTime,
            entity.EndTime
        );

        // 201 + location header pointing to GET /api/timeentries/{id}
        return CreatedAtAction(nameof(GetById), new { id = entity.TimeEntryId }, dto);
    }

    [HttpPost("clock-out")]
    public async Task<ActionResult<TimeEntryDto>> ClockOut([FromBody] ClockRequest req, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        if (!TryGetEmployeeId(out var employeeId))
            return BadRequest(new { code = "MissingEmployeeId", message = "Provide X-Employee-Id header (int)." });

        // find the most recent open entry (EndTime is NULL) for this employee.
        var open = await _db.TimeEntries
            .Where(t => t.EmployeeId == employeeId && t.EndTime == null)
            .OrderByDescending(t => t.StartTime)
            .FirstOrDefaultAsync(ct);

        if (open is null)
            return Conflict(new { code = "NoActiveClockIn", message = "No active clock-in to close." });

        open.EndTime = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        var dto = new TimeEntryDto(
            open.TimeEntryId,
            open.EmployeeId,
            open.AssignmentId,
            open.StartTime,
            open.EndTime
        );

        return Ok(dto);
    }

    // ======================
    // GET /api/timeentries/mine
    // Returns this employee's recent entries (newest first).
    // ======================
    [HttpGet("mine")]
    public async Task<ActionResult<IEnumerable<TimeEntryDto>>> Mine(CancellationToken ct)
    {
        if (!TryGetEmployeeId(out var employeeId))
            return BadRequest(new { code = "MissingEmployeeId", message = "Provide X-Employee-Id header (int)." });

        var list = await _db.TimeEntries
            .Where(t => t.EmployeeId == employeeId)
            .OrderByDescending(t => t.StartTime)
            .Select(t => new TimeEntryDto(t.TimeEntryId, t.EmployeeId, t.AssignmentId, t.StartTime, t.EndTime))
            .ToListAsync(ct);

        return Ok(list);
    }
}
