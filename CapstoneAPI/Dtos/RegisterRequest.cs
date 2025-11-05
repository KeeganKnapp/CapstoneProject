/*


--------------------------------------------------------------------------------------
Defines the data structure that the frontend sends to the backend when
a user wants to create an account, used by auth/register endpoint

*/

namespace CapstoneAPI.Dtos
{
    public class RegisterRequest
    {
        public string Email { get; set; } = null!;     // login name (citext in db => case-insensitive)
        public string Password { get; set; } = null!;  // raw password
        public string? DisplayName { get; set; }       // optional friendly name for UI
    }
}