/*

ITokenService interface
--------------------------------------------------------------------------------------
Defines the exact behavior for creating Jwt access tokens and opaque refresh tokens

*/

using CapstoneAPI.Models; // imports User model

namespace CapstoneAPI.Services
{
    // defines a contract for generating tokens, tells the rest of the app that any token service 
    // must be able to create an access token (Jwt) that identifies a user and create refresh token
    public interface ITokenService
    {
        (string accessToken, DateTimeOffset accessExpires) CreateAccessToken(User user);  // creates a Jwt that the frontend will send with each request
        (string refreshToken, DateTimeOffset refreshExpires) CreateRefreshToken();        // creates a refresh token
    }
}