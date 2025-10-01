/*
    FakeAuthService.cs pretends to authenticate a user, will always login
    regardless of the password and if the username is admin, it will 
    assign the role of admin; otherwise user. also generates a fake JWT token
*/

using CapstoneAPI.Dtos;
namespace CapstoneAPI.Services;

public class FakeAuthService : IAuthService
{
    public (string token, string role) Login(string username, string password)
    {
        var role = username.Equals("admin", StringComparison.OrdinalIgnoreCase) ? "admin" : "user";

        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

        return (token, role);
    }
}

/*
    need to swap fakeauthservice for a real auth service such as JWT + user sto
*/
