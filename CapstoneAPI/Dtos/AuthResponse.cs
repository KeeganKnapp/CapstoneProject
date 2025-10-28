/*

Success response object for all /auth endpoints
--------------------------------------------------------------------------------------
Defines what the API sends back to the client when authentication
succeeds either after registration, login, or refresh

*/

namespace CapstoneAPI.Dtos
{
    public class AuthResponse
    {
        public string AccessToken { get; set; } = null!;                 // short-lived JWT
        public DateTimeOffset AccessTokenExpiresAt { get; set; }         // client can preemptively refresh
        public string RefreshToken { get; set; } = null!;                // random string, used to get a new access token after expiration
        public DateTimeOffset RefreshTokenExpiresAt { get; set; }       // tells exactly when the token will expire
        public string Email { get; set; } = null!;                      // lets the client show which user is logged in
        public string? DisplayName { get; set; }
    }
}