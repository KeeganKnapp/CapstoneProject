using CapstoneAPI.Data;
using CapstoneAPI.DTOs;
using CapstoneAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CapstoneAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequestOffController : ControllerBase
    {
        private readonly CapstoneDbContext _db;
        public RequestOffController(CapstoneDbContext db) => _db = db;

        private int GetUserId()
        {
            // try common claim types in order
            var id = User.FindFirstValue("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");

            if (id is null) throw new UnauthorizedAccessException("UserId claim missing.");
            return int.Parse(id);
        }

        // POST /api/requestoff
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<RequestOffDto>> Create([FromBody] RequestOffCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            if (dto.StartDate > dto.EndDate) return BadRequest("StartDate must be <= EndDate.");

            var entity = new RequestOff
            {
                UserId = GetUserId(),
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Note = dto.Note,
                Status = "Pending"
            };

            _db.RequestOffs.Add(entity);
            await _db.SaveChangesAsync();

            var result = new RequestOffDto(entity.RequestOffId, entity.UserId, entity.StartDate, entity.EndDate, entity.Note, entity.Status);
            return CreatedAtAction(nameof(GetMineById), new { id = entity.RequestOffId }, result);
        }        
        
        // GET /api/requestoff/{id}   (self-only)
        [HttpGet("{id:long}")]
        [Authorize]
        public async Task<ActionResult<RequestOffDto>> GetMineById(long id)
        {
            var me = GetUserId();
            var entity = await _db.RequestOffs.AsNoTracking()
                .FirstOrDefaultAsync(r => r.RequestOffId == id && r.UserId == me);

            if (entity is null) return NotFound();

            return new RequestOffDto(entity.RequestOffId, entity.UserId, entity.StartDate, entity.EndDate, entity.Note, entity.Status);
        }

        // GET /api/requestoff/mine   (get own requests)
        [HttpGet("mine")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<RequestOffDto>>> GetMyRequests()
        {
            var me = GetUserId();
            var requests = await _db.RequestOffs.AsNoTracking()
                .Where(r => r.UserId == me)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new RequestOffDto(r.RequestOffId, r.UserId, r.StartDate, r.EndDate, r.Note, r.Status))
                .ToListAsync();

            return Ok(requests);
        }

        // DELETE /api/requestoff/{id}   (self-only)
        [HttpDelete("{id:long}")]
        [Authorize]
        public async Task<IActionResult> DeleteMine(long id)
        {
            var me = GetUserId();
            var entity = await _db.RequestOffs
                .FirstOrDefaultAsync(r => r.RequestOffId == id && r.UserId == me);

            if (entity is null) return NotFound();

            _db.RequestOffs.Remove(entity);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // GET /api/requestoff   (management list & filters)
        [HttpGet]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<IEnumerable<RequestOffDto>>> List(
            [FromQuery] int? userId,
            [FromQuery] DateOnly? from,
            [FromQuery] DateOnly? to)
        {
            var q = _db.RequestOffs.AsQueryable();

            if (userId.HasValue) q = q.Where(x => x.UserId == userId.Value);
            if (from.HasValue)   q = q.Where(x => x.EndDate >= from.Value);
            if (to.HasValue)     q = q.Where(x => x.StartDate <= to.Value);

            var items = await q
                .OrderByDescending(x => x.StartDate)
                .Select(x => new RequestOffDto(x.RequestOffId, x.UserId, x.StartDate, x.EndDate, x.Note, x.Status))
                .ToListAsync();

            return Ok(items);
        }

        // PUT /api/requestoff/approve/{id}
        [HttpPut("/approve/{RequestOffId:int}")]
        [Authorize]
        public async Task<IActionResult> Approve(int RequestOffId, [FromBody] CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var request = await _db.RequestOffs.FirstOrDefaultAsync(j => j.RequestOffId == RequestOffId, ct);

            if (request == null)
                throw new KeyNotFoundException($"Request {RequestOffId} not found.");

            request.Status = "Approved";

            request.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);

            return Ok(request);
        } 

        // PUT /api/requestoff/deny/{id}
        [HttpPut("/deny/{RequestOffId:int}")]
        [Authorize]
        public async Task<IActionResult> Deny(int RequestOffId, [FromBody] CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var request = await _db.RequestOffs.FirstOrDefaultAsync(j => j.RequestOffId == RequestOffId, ct);

            if (request == null)
                throw new KeyNotFoundException($"Request {RequestOffId} not found.");

            request.Status = "Denied";

            request.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);

            return Ok(request);
        } 
    }
}