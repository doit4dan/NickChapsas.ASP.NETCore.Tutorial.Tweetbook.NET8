using Humanizer.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Tweetbook.Contracts.V1;
using Tweetbook.Contracts.V1.Requests;
using Tweetbook.Contracts.V1.Responses;
using Tweetbook.Data;

namespace Tweetbook.IntegrationTests
{    
    public class IntegrationTest
    {
        protected readonly HttpClient TestClient;

        protected IntegrationTest()
        {
            // We can use Web Applicaton Factory to test real application running. This is the real deal, it is just that server is running in memory
            // Sets up in-memory database ( doesn't seem to work on .NET 8 )
            var appFactory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {                    
                    builder.ConfigureServices(services =>
                    {
                        // could not get in-memory database to function properly
                        // utilized local sql server with different db name
                        services.RemoveAll(typeof(DataContext));
                        services.RemoveAll(typeof(DbContextOptions<DataContext>));
                        var connectionString = "Server=(localdb)\\mssqllocaldb;Database=Tweetbook_Tests;Trusted_Connection=True;MultipleActiveResultSets=true";
                        services.AddDbContext<DataContext>(options =>
                           options.UseSqlServer(connectionString));

                        // commented out, this in-memory approach did not seem to work for me in .NET 8
                        //services.AddDbContext<DataContext>(options =>
                        //{
                        //    options.UseInMemoryDatabase("TestDb");
                        //});
                    });
                });            

            TestClient = appFactory.CreateClient();
        }

        protected async Task AuthenticateAsync()
        {
            TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetJwtAsync());
        }

        protected async Task<PostResponse?> CreatePostAsync(CreatePostRequest request)
        {
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Posts.Create, request);
            return await response.Content.ReadFromJsonAsync<PostResponse>();
        }

        protected async Task<HttpResponseMessage?> DeletePostAsync(Guid postId)
        {
            return await TestClient.DeleteAsync(ApiRoutes.Posts.Delete.Replace("{postId}",postId.ToString()));            
        }

        private async Task<string> GetJwtAsync()
        {
            // I couldn't get the in-memory database to work, this is workaround using actual Sql Server
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Register, new UserRegistrationRequest
            {
                Email = "test@integration.com",
                Password = "SomePass1234!"
            });            

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var regResponse = await response.Content.ReadFromJsonAsync<AuthSuccessResponse>();
                return (regResponse is null ? "" : regResponse.Token);
            }

            response = await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Login, new UserLoginRequest
            {
                Email = "test@integration.com",
                Password = "SomePass1234!"
            });

            var loginResponse = await response.Content.ReadFromJsonAsync<AuthSuccessResponse>();
            return (loginResponse is null ? "" : loginResponse.Token);
        }
    }
}
