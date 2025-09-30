namespace CapstoneMaui.Core.Services.Abstractions
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
        Task LogoutAsync(CancellationToken cancellationToken = default);
        Task<bool> IsUserLoggedInAsync(CancellationToken cancellationToken = default);

        Task<string> GetAuthTokenAsync(CancellationToken cancellationToken = default);
    }
}