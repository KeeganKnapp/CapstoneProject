using CapstoneAPI.Dtos;
namespace CapstoneAPI.Services;

public interface IAuthService
{
    (string token, string role) Login(string username, string password);
}