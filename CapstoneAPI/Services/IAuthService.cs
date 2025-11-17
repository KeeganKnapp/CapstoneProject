/*

IAuthService interface
--------------------------------------------------------------------------------------
Specifies what methods any AuthService must implement

*/

using CapstoneAPI.Dtos;  // importing DTO classes

namespace CapstoneAPI.Services
{
    // IAuthService defines what an authentication service must be able to do
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest req, CancellationToken ct); // handles user registration, takes a RegisterRequest obj and returns an AuthResponse
        Task<AuthResponse> LoginAsync(LoginRequest req, CancellationToken ct);       // handles user login, takes LoginRequest and returns AuthResponse with new access and refresh tokens
        Task<AuthResponse> RefreshAsync(string refreshToken, CancellationToken ct);  // handles access token renewal, takes a string refresh token, verifies it, rotates it, and returns new tokens
        Task LogoutAsync(string refreshToken, CancellationToken ct); // revoke a single refresh token
        Task LogoutAllAsync(int userId, CancellationToken ct);      // revoke all refresh tokens for a user
    }
}

