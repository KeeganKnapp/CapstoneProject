/*
using System.Security.Claims;
using CapstoneAPI.Dtos;
using CapstoneAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [Authorize]

    public class AssignmentController : ControllerBase
    {
        private readonly IAssignmentService _assignmentService;
        public AssignmentController(IAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }
        
        // helper for getting UserId's
        private int GetUserId()
        {
            // try common claim types in order
            var id = User.FindFirstValue("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");

            if (id is null) throw new UnauthorizedAccessException("UserId claim missing.");
            
            return int.Parse(id);
        }


        // POST api/AssignmentController
        // manager creating a new jobsite
        // returns 201 and newly created assignment DTO

        [HttpPost]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(AssignmentResponse), 201)]
        public async Task<ActionResult<AssignmentResponse>> CreateAssignment([FromBody] CreateAssiignmentRequest req, CancellationToken ct)
        {
            try
            {
                var res = await _assignmentService.CreateAssignmentAsync(req, ct);
                return CreateAtAction(nameof(GetAssignmentById), new {id = res.Id}, res);
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(new {error = ex.Message});
            }
            catch (InvalidOperationException)
            {
                return BadRequest(new {error = ex.Message});
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(AssignmentResponse), 200)]
        public async Task<ActionResponse<AssignmentResponse>> UpdateAssignment(int id, [FromBody] UpdateAssignmentRequest req, CancellationToken ct)
        {
            try
            {
                var res = await _assignmentService.UpdateAssignemntAsync(id, req, ct);
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
        public async Task<IActionResult> RemoveAssignment(int id, CancellationToken ct)
        {
            try
            {
                await _assignmentService.RemoveAssignmentAsync(id, ct);
                return NoContent();
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(new {error = ex.Message});
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AssignmentResponse), 200)]
        public async Task<ActionResult<AssignmentResponse>> GetAssignmentById(int id, CancellationToken ct)
        {
            try
            {
                var res = await _assignmentService.GetAssignmentByIdAsync(id,ct);
                return Ok(res);
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(new {error = ex.Message});
            }
        }

        [HttpGet("jobsite/{JobsiteID}")]
        [ProducesResponseType(typeof(List<AssignmentResponse>), 200)]
        public async Task<ActionResult<List<AssignmentResult>>> GetAssignmentByJobsite(int jobsiteId, CancellationToken ct)
        {
            try
            {
                var res = await _assignmentService.GetAssignmentByJobsiteIdAsync(jobsiteId, ct);
                return Ok(res);
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(new {error = ex.Message});
            }
        }
    }
}
*/