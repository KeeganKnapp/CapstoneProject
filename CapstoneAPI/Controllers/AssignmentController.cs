
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
        // manager creating a new asssignment
        // returns 201 and newly created assignment DTO

        [HttpPost]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(AssignmentResponse), 201)]
        public async Task<ActionResult<AssignmentResponse>> CreateAssignment([FromBody] AssignmentCreateRequest req)
        {
            try
            {
                var creatorUserId = GetUserId();

                var res = await _assignmentService.CreateAssignmentAsync(creatorUserId, req);
                return CreatedAtAction(nameof(GetAssignmentById), new {id = res.AssignmentId}, res);
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(new {error = ex.Message});
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new {error = ex.Message});
            }
        }


        // manager updating an assignment

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(AssignmentResponse), 200)]
        public async Task<ActionResult<AssignmentResponse>> UpdateAssignment(int id, [FromBody] AssignmentUpdateRequest req)
        {
            try
            {
                var userId = GetUserId();

                var res = await _assignmentService.UpdateAsync(id, userId, req);
                return Ok(res);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }




        // manager delete assignment

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> RemoveAssignment(int id)
        {
            try
            {
                var userId = GetUserId();
                await _assignmentService.DeleteAsync(id, userId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }


        // manager or employee get assignments by id

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AssignmentResponse), 200)]
        public async Task<ActionResult<AssignmentResponse>> GetAssignmentById(int id)
        {
            try
            {
                var userId = GetUserId();

                var res = await _assignmentService.GetByIdAsync(id, userId);
                return Ok(res);
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(new {error = ex.Message});
            }
        }


        // manager get assignments for a jobsite

        [HttpGet("jobsite/{jobsiteId:int}")]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(IEnumerable<AssignmentResponse>), 200)]
        public async Task<ActionResult<IEnumerable<AssignmentResponse>>> GetAssignmentByJobsite(int jobsiteId)
        {
            try
            {
                var userId = GetUserId();

                var res = await _assignmentService.GetForJobsiteAsync(jobsiteId, userId);
                return Ok(res);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }
    }
}
