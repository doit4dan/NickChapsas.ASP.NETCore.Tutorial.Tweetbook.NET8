using Tweetbook.Domain;

namespace Tweetbook.Services
{
    public interface IPostService
    {
        Task<List<Post>> GetPostsAsync(GetAllPostsFilter? filter = null, PaginationFilter? paginationFilter = null);
        Task<Post?> GetPostByIdAsync(Guid postId);
        Task<bool> CreatePostAsync(Post post);
        Task<bool> UpdatePostAsync(Post post);
        Task<bool> DeletePostAsync(Guid postId);
        Task<bool> UserOwnsPostAsync(Guid postId, string userId);
        Task<List<Tag>> GetAllTagsAsync(GetAllTagsFilter? filter = null, PaginationFilter? paginationFilter = null);
        Task<Tag?> GetTagByIdAsync(Guid tagId);
        Task<Tag?> GetTagByNameAsync(string tagName);
        Task<bool> CreateTagAsync(Tag tag);
        Task<bool> UpdateTagAsync(Tag tag);
        Task<bool> DeleteTagAsync(Guid tagId);
    }
}
