using Microsoft.AspNetCore.WebUtilities;
using Tweetbook.Contracts.V1;
using Tweetbook.Contracts.V1.Requests.Queries;

namespace Tweetbook.Services
{
    public class UriService : IUriService
    {
        private readonly string _baseUri;

        public UriService(string baseUri)
        {
            _baseUri = baseUri;
        }        

        public Uri GetPostUri(string postId)
        {
            return new Uri(_baseUri + ApiRoutes.Posts.Get.Replace("{postId}",postId));
        }

        public Uri GetAllPostsUri(PaginationQuery? pagination = null)
        {
            var uri = new Uri(_baseUri + ApiRoutes.Posts.GetAll);

            if (pagination == null)
            {
                return uri;
            }

            var modifiedUri = QueryHelpers.AddQueryString(uri.ToString(), "pageNumber", pagination.PageNumber.ToString());
            modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "pageSize", pagination.PageSize.ToString());

            return new Uri(modifiedUri);
        }
    }
}
