/*


--------------------------------------------------------------------------------------
Defines the input format that the client must send to the API when it
wants to refresh tokens. the short lived Jwt token expires and the app needs
a new one without reentering a pwd

*/

namespace CapstoneBlazorApp.Dtos
{
    public class RefreshRequest
    {
        public string RefreshToken { get; set; } = null!;
    }
}