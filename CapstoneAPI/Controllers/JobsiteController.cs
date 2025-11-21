/*
    create jobsite
    update jobiste
    delete jobsite
    get all jobsites
    get jobsite by id
*/

using CapstoneAPI.Dtos;
using CapstoneAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneAPI.Controller
{
    
[ApiController]
[Route("/api/[controllers]")]
[Authorize]
public class JobsiteController : ControllerBase
{
    private readonly IJobsiteService _jobsiteService;
    public JobsiteController(IJobsiteService jobsiteService) => _jobsiteService = jobsiteService;

    [HttpPost]
    [ProducesResponseType(typeof(JobsiteResponse), 201)]
    public async Task<ActionResult<JobsiteResponse>> CreateJobsite([FromBody] CreateJobsiteRequest req, CancellationToken ct)
    {
        try
        {
            var res = await _jobsiteService.CreateJobsiteAsync(req, ct);
            return CreatedAtAction(nameof(GetJobsiteById), new {id = res.Id}, res);
        }
        catch (InvalidOperationException)
        {
            return BadRequest(new {error = ex.Message});
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(JobsiteResponse), 200)]
    public async Task<ActionResult<JobsiteResponse>> UpdateJobsite(int id, [FromBody] UpdateJobsiteRequest req, CancellationToken ct)
    {
        try
        {
            var res = await _jobsiteService.UpdateJobsiteAsync(id, req, ct);
            return Ok(res);
        }
        catch(KeyNotFoundException ex)
        {
            return NotFound(new {error = ex.Message});
        }
        catch(InvalidOperationException ex)
        {
            return BadRequest(new {error = ex.Message});
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteJobsite(int id, CancellationToken ct)
    {
        try
        {
            await _jobsiteService.DeleteJobsiteAsync(id, ct);
            return NoContent();
        }
        catch(KeyNotFoundException ex)
        {
            return NotFound(new {error = ex.Message});
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<JobsiteResponse>), 200)]
    public async Task<ActionResult<List<JobsiteResponse>>> GetAllJobsite(CancellationToken ct)
    {
        var res = await _jobsiteService.GetAllJobsiteAsync(ct);
        return Ok(res);
    }
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(JobsiteResponse), 200)]
    public async Task<ActionResult<JobsiteResponse>> GetJobsiteById(int id, CancellationToken ct)
    {
        try
        {
            var res = await _jobsiteService.GetJobsiteByIdAsync(id, ct);
            return Ok(res);
        }
        catch(KeyNotFoundException ex)
        {
            return NotFound(new {error = ex.Message});
        }
    }
}
}