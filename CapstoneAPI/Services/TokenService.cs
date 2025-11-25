using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Emit;
using System.Security.Claims;           // defines user claims (user id, email)
using System.Security.Cryptography;     // secure random number generation
using System.Text;                      // encoding secret key as bites
using CapstoneAPI.Helpers;
using CapstoneAPI.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CapstoneAPI.Services
{
    // responsible for generating Jwt access tokens and refresh tokens
    public class TokenService : ITokenService
    {
        private readonly JwtOptions _opts;                 // lifetimes, issuer, audience, secret from appsettings.json
        private readonly byte[] _keyBytes;                 // byte array representation of the secret key

        // constructor recieves IOptions<JwtOptions> via dependency injection
        public TokenService(IOptions<JwtOptions> options)
        {
            _opts = options.Value; // holds Jwt config
            _keyBytes = Convert.FromBase64String(_opts.Secret); // convert the secret string into bytes so it can be used as a symmetric encryption key
        }

        // generates a Jwt for a given user
        public (string accessToken, DateTimeOffset accessExpires) CreateAccessToken(User user)
        {
            var now = DateTimeOffset.UtcNow; // get current time
            var expires = now.AddMinutes(_opts.AccessTokenLifetimeMinutes); // calculate the tokens expiration time using JwtOptions.AccessTokenLifetimeMinutes

            // claims are small pieces of user data encoded into the token payload
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),   // subject = user id, identifies user
                new(JwtRegisteredClaimNames.Email, user.Email),             // provides user email
                new("displayName", user.DisplayName ?? string.Empty),        // custom claim
                new(ClaimTypes.Role, user.Role),
                new("userId", user.UserId.ToString())
            };

            // creating signing credentials
            var creds = new SigningCredentials(new SymmetricSecurityKey(_keyBytes), SecurityAlgorithms.HmacSha256);

            // JwtSecurityToken represents the final token before its serialized into a string
            var token = new JwtSecurityToken(
                issuer: _opts.Issuer,
                audience: _opts.Audience,
                claims: claims,
                notBefore: now.UtcDateTime,
                expires: expires.UtcDateTime,
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token); // JwtSecurityTokenHandler converts the token obj into a string
            return (jwt, expires); // return the token string and its expiration timestamp 
        }

        // creates a refresh token
        public (string refreshToken, DateTimeOffset refreshExpires) CreateRefreshToken()
        {
            // creates a 64-byte random array using cryptographically secure RNG
            var bytes = new byte[64];
            RandomNumberGenerator.Fill(bytes);

            // convert random bytes to Base64 text so it can be stored
            var token = Convert.ToBase64String(bytes);

            // set refresh token expiration date
            var expires = DateTimeOffset.UtcNow.AddDays(_opts.RefreshTokenLifetimeDays);
            return (token, expires); // returns both the opaque string and its expiration time
        }
    }
}