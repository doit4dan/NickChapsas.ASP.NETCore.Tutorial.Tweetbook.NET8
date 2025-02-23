using Refit;
using Tweetbook.Contracts.V1.Requests;

namespace Tweetbook.Sdk.Sample
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var cachedToken = string.Empty;

            // both should be using the same HttpClient ( that is best practice )
            var identityApi = RestService.For<IIdentityApi>("https://localhost:5001"); // this will generate implementation for api
            
            var tweetBookApi = RestService.For<ITweetbookApi>("https://localhost:5001", new RefitSettings
            {
                AuthorizationHeaderValueGetter = (r,ct) => Task.FromResult(cachedToken)
            });

            var registerResponse = await identityApi.RegisterAsync(new UserRegistrationRequest
            {
                Email = "testaccount@gmail.com",
                Password = "Password123!"
            });

            var loginResponse = await identityApi.LoginAsync(new UserLoginRequest
            {
                Email = "testaccount@gmail.com",
                Password = "Password123!"
            });

            // in real system, you would want to check if token is invalid. If so, have it call login endpoint to refresh token
            cachedToken = loginResponse.Content.Token;

            var allPosts = await tweetBookApi.GetAllAsync();

            var createdPost = await tweetBookApi.CreateAsync(new CreatePostRequest
            {
                Name = "This is created by the SDK",
                Tags = new List<string>{ "sdk" }
            });

            var retrievedPost = await tweetBookApi.GetAsync(createdPost.Content.Id);

            var updatedPost = await tweetBookApi.UpdateAsync(createdPost.Content.Id, new UpdatePostRequest
            {
                Name = "This is updated by the SDK"
            });

            var deletePost = await tweetBookApi.DeleteAsync(createdPost.Content.Id);
        }
    }
}
