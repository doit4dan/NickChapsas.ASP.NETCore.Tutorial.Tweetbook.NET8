using Tweetbook.Contracts.V1.Requests.Queries;
using Tweetbook.Contracts.V1.Responses;
using Tweetbook.Domain;
using Tweetbook.Services;
using static Tweetbook.Contracts.V1.ApiRoutes;

namespace Tweetbook.Helpers
{
    public static class PaginationHelpers
    {
        public static PagedResponse<T> CreatePaginatedResponse<T>(IUriService uriService, PaginationFilter pagination, List<T> response)
        {
            string? nextPage = null;
            string? prevPage = null;

            if(typeof(T) == typeof(PostResponse))
            {
                nextPage = pagination.PageNumber >= 1 ? uriService
                    .GetAllPostsUri(new PaginationQuery(pagination.PageNumber + 1, pagination.PageSize)).ToString()
                    : null;

                prevPage = pagination.PageNumber - 1 >= 1 ? uriService
                    .GetAllPostsUri(new PaginationQuery(pagination.PageNumber - 1, pagination.PageSize)).ToString()
                    : null;
            }
            else if(typeof(T) == typeof(TagResponse))
            {
                nextPage = pagination.PageNumber >= 1 ? uriService
                    .GetAllTagsUri(new PaginationQuery(pagination.PageNumber + 1, pagination.PageSize)).ToString()
                    : null;

                prevPage = pagination.PageNumber - 1 >= 1 ? uriService
                    .GetAllTagsUri(new PaginationQuery(pagination.PageNumber - 1, pagination.PageSize)).ToString()
                    : null;
            }            

            return new PagedResponse<T>
            {
                Data = response,
                PageNumber = pagination.PageNumber >= 1 ? pagination.PageNumber : (int?)null,
                PageSize = pagination.PageSize >= 1 ? pagination.PageSize : (int?)null,
                NextPage = response.Any() ? nextPage ?? "" : "",
                PreviousPage = prevPage ?? ""
            };
        }
    }
}
