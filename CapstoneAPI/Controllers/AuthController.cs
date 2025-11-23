/*

Entry point of the authentication system
----------------------------------------
Responsible for handling HTTP requests that come from the frontend or Swagger for testing purposes.
It calls the service layer (AuthService) which contains the authentication logic

*/

//   - returns 200 with AuthResponse on success
//   - returns 400 for user errors (email already taken)
//   - returns 401 for bad credentials or invalid/expired refresh tokens

using System.Security.Claims;               // allows reading information from JWT
using CapstoneAPI.Dtos;                     // links request/response data models such as LoginRequest or AuthResponse
using CapstoneAPI.Services;                 // IAuthService dependency
using Microsoft.AspNetCore.Authorization;   // enables [Authorize] attributes on endpoints
using Microsoft.AspNetCore.Mvc;             // web api framework such as controllers or routes

namespace CapstoneAPI.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase        // inherited helper methods
    {
        private readonly IAuthService _auth;
        public AuthController(IAuthService auth) => _auth = auth; // injects AuthService impl at runtime

        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponse), 200)] // success type for Swagger
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest req, CancellationToken ct) // RegisterRequest reads JSON body and converts into obj
        {
            // _auth.RegisterAsync(req, ct) call to handle registration logic in service layer
            try
            {
                var res = await _auth.RegisterAsync(req, ct);   
                return Ok(res);                                  
            }                                                   
            catch (InvalidOperationException ex)                
            {                                                   
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), 200)]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest req, CancellationToken ct)
        {
            // calls _auth.LoginAsync() to verify email and pwd and issue tokens, returns 200 with valid AuthResponse if credentials are correct
            // returns 401 Unauthorized if credentials are invalid
            try
            {
                var res = await _auth.LoginAsync(req, ct);
                return Ok(res);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

        // used when the users access token expires
        // frontend sends refresh token, not the pwd
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(AuthResponse), 200)]
        public async Task<ActionResult<AuthResponse>> Refresh([FromBody] RefreshRequest req, CancellationToken ct)
        {
            // calls _auth.RefreshAsync() and if refresh token is valid and active, a new access and refresh pair is issued
            // if the refresh token was revoked or expired results in 401 Unauthorized
            try
            {
                var res = await _auth.RefreshAsync(req.RefreshToken, ct);
                return Ok(res);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }


        // manual logout, API revokes the provided refresh token so it cant be reused
        // returns 204 No Content cuz theres nothing to send back
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshRequest req, CancellationToken ct)
        {
            await _auth.LogoutAsync(req.RefreshToken, ct);
            return NoContent();
        }

        // for if a user wants to logout on all devices
        [Authorize]
        [HttpPost("logout-all")]
        public async Task<IActionResult> LogoutAll(CancellationToken ct)
        {
            var raw =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue("UserId") ??
                User.FindFirstValue("sub");

            if (string.IsNullOrWhiteSpace(raw) || !int.TryParse(raw, out var userId))
                return Unauthorized(new { error = "Invalid or missing user id claim." });

            await _auth.LogoutAllAsync(userId, ct);
            return NoContent();
        }
    }
}