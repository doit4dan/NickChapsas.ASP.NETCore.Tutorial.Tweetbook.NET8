
namespace Tweetbook.Services
{
    public interface IIdentityService
    {
        Task<Domain.AuthenticationResult> RegistrationAsync(string email, string password);
        Task<Domain.AuthenticationResult> LoginAsync(string email, string password);
        Task<Domain.AuthenticationResult> RefreshTokenAsync(string token, string refreshToken);
    }
}
