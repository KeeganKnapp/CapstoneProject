/*


--------------------------------------------------------------------------------------
Defines the data format that the frontend must send to the backend
when a user tries to log in, used my /auth/login endpoint

*/

namespace CapstoneAPI.Dtos
{
    public class LoginRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}