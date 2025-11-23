/*


--------------------------------------------------------------------------------------
Defines a set of strongly typed config settings that is read from
appsettings.json

Controls how the Jwts are generated and validated, how long they last,
what secret key signs them, and who the token is intended for

*/

namespace CapstoneAPI.Helpers
{
    public class JwtOptions
    {
        public string Issuer { get; set; } = null!;                     // identifies this API
        public string Audience { get; set; } = null!;                   // intended consumer (client app)
        public string Secret { get; set; } = null!;                     // HMAC symmetric key (>=64 chars)
        public int AccessTokenLifetimeMinutes { get; set; } = 15;       // short-lived for security
        public int RefreshTokenLifetimeDays { get; set; } = 7;          // longer-lived session
    }
}