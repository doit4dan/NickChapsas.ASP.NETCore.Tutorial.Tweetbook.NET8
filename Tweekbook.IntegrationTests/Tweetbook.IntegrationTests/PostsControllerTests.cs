using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Tweetbook.Contracts.V1;
using Tweetbook.Domain;
using System.Net;
using Tweetbook.Contracts.V1.Requests;
using Tweetbook.Contracts.V1.Responses;

namespace Tweetbook.IntegrationTests
{
    public class PostsControllerTests : IntegrationTest
    {
        // controller method name, then test description
        [Fact]
        public async Task GetAll_WithoutAnyPosts_ReturnsEmptyResponse()
        {
            // Arrange
            await AuthenticateAsync();

            // Act
            var response = await TestClient.GetAsync(ApiRoutes.Posts.GetAll);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (await response.Content.ReadFromJsonAsync<List<Post>>()).Should().BeEmpty();
        }

        [Fact]        
        public async Task Get_ReturnsPost_WhenPostsExistInTheDatabase()
        {
            // Arrange
            await AuthenticateAsync();
            var createdPost = await CreatePostAsync(new CreatePostRequest { Name = "Test post" });

            // Act
            var response = await TestClient.GetAsync(ApiRoutes.Posts.Get.Replace("{postId}", createdPost.Id.ToString()));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var post = await response.Content.ReadFromJsonAsync<Post>();

            if(post == null)            
                post = new Post { Id = Guid.Empty, Name = "" };
            
            post.Id.Should().Be(createdPost.Id);
            post.Name.Should().Be("Test post");
            
            await DeletePostAsync(createdPost.Id);                        
        }
    }
}
