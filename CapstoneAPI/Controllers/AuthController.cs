/*
    controller for handling login, uses IAuthService (implemented by FakeAuthService)
    and accepts a username/password, asks IAuthService.Login() and returns a fake token
    and role
*/

using CapstoneAPI.Dtos;
using CapstoneAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService auth) : ControllerBase
{
    [HttpPost("login")]
    public ActionResult<LoginResponse> Login([FromBody] LoginRequest req)
    {
        var (token, role) = auth.Login(req.username, req.password);
        return Ok(new LoginResponse(token, role));
    }
}