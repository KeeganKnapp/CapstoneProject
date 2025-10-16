/*
    controller for handling clock in / out and view history. Uses 
    FakeStorage.timeEntries for the in memory list, when you clock
    in/out it creates a TimeEntryDto with the current UTC time
    (needs changed to EST) and stores it in memory
*/

using CapstoneAPI.Dtos;
using CapstoneAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimeEntriesController : ControllerBase
{
    [HttpPost("clockin")]
    public ActionResult<TimeEntryDto> ClockIn([FromBody] ClockRequest req)
    {
        // must add geofence validation and user identity later
        var entry = new TimeEntryDto(id: Random.Shared.Next(1, 99), TimestampUtc: DateTimeOffset.UtcNow,
        jobsiteiD: req.jobsiteiD, type: "in");

        FakeStorage.timeEntries.Add(entry);
        return Ok(entry);
    }

    [HttpPost("clockout")]
    public ActionResult<TimeEntryDto> Clockout([FromBody] ClockRequest req)
    {
        var entry = new TimeEntryDto(id: Random.Shared.Next(1, 99), TimestampUtc: DateTimeOffset.UtcNow,
        jobsiteiD: req.jobsiteiD, type: "out");

        FakeStorage.timeEntries.Add(entry);
        return Ok(entry);
    }

    [HttpGet("mine")]
    public ActionResult<IEnumerable<TimeEntryDto>> Mine() => Ok(FakeStorage.timeEntries);
}