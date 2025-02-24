using Tweetbook.Contracts.V1.Requests.Queries;

namespace Tweetbook.Services
{
    public interface IUriService
    {
        Uri GetPostUri(string postId);

        Uri GetAllPostsUri(PaginationQuery? paginationQuery = null);

        Uri GetTagUri(string tagId);

        Uri GetAllTagsUri(PaginationQuery? paginationQuery = null);
    }
}
