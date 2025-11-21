/*
    create jobsite
    update jobiste
    delete jobsite
    get all jobsites
    get jobsite by id
*/

using System.Security.Claims;
using CapstoneAPI.Dtos;
using CapstoneAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneAPI.Controllers
{
    
[ApiController]
[Route("api/[controller]")]
[Authorize]
    public class JobsiteController : ControllerBase
    {
        private readonly IJobsiteService _jobsiteService;
        public JobsiteController(IJobsiteService jobsiteService)
        {
                _jobsiteService = jobsiteService;
        }


        private int GetUserId()
        {
            var id = User.FindFirstValue("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"); // typical JWT subject claim

            if (id is null)
                throw new UnauthorizedAccessException("UserId claim missing.");

            return int.Parse(id);
        }


        [HttpPost]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(JobsiteResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<JobsiteResponse>> CreateJobsite([FromBody] CreateJobsiteRequest req, CancellationToken ct)
        {
            try
            {
                var managerUserId = GetUserId();


                var res = await _jobsiteService.CreateJobsiteAsync(req, ct);
                return CreatedAtAction(nameof(GetJobsiteById), new {id = res.JobsiteId}, res);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new {error = ex.Message});
            }
        }


        // PUT /api/Jobsite/{id}
        // update existing jobsite

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(JobsiteResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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


        // DELETE /api/Jobsite/{id}
        // for manager to delete a jobsite
        // 204 on success

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]   
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


        // GET /api/Jobsite/{id}
        // any user can fetch an assignment by its id
        // returns 200 OK + JobsiteResponse or 404 Not Found

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(JobsiteResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]        
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


        // GET /api/Jobsite
        // drop down list for any user
        [HttpGet]
        [ProducesResponseType(typeof(List<JobsiteResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]    
        public async Task<ActionResult<List<JobsiteResponse>>> GetAllJobsite(CancellationToken ct)
        {
            var res = await _jobsiteService.GetAllJobsiteAsync(ct);
            return Ok(res);
        }


    }
}