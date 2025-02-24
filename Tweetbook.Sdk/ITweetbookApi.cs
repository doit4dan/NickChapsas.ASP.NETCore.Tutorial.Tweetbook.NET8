using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetbook.Contracts.V1.Requests;
using Tweetbook.Contracts.V1.Requests.Queries;
using Tweetbook.Contracts.V1.Responses;

namespace Tweetbook.Sdk
{
    [Headers("Authorization: Bearer")]
    public interface ITweetbookApi
    {
        [Get("/api/v1/posts")]
        Task<ApiResponse<PagedResponse<PostResponse>>> GetAllAsync([Query] GetAllPostsQuery query, [Query] PaginationQuery paginationQuery);

        [Get("/api/v1/posts/{postId}")]
        Task<ApiResponse<Response<PostResponse>>> GetAsync(Guid postId);

        [Post("/api/v1/posts")]
        Task<ApiResponse<Response<PostResponse>>> CreateAsync([Body] CreatePostRequest createPostRequest);

        [Put("/api/v1/posts/{postId}")]
        Task<ApiResponse<Response<PostResponse>>> UpdateAsync(Guid postId, [Body] UpdatePostRequest updatePostRequest);

        [Delete("/api/v1/posts/{postId}")]
        Task<ApiResponse<string>> DeleteAsync(Guid postId);
    }
}
