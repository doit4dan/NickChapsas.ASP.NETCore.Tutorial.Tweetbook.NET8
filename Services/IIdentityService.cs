
namespace Tweetbook.Services
{
    public interface IIdentityService
    {
        Task<Domain.AuthenticationResult> RegistrationAsync(string email, string password);
    }
}
