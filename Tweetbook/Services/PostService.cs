using Microsoft.EntityFrameworkCore;
using Tweetbook.Data;
using Tweetbook.Domain;
using static Tweetbook.Contracts.V1.ApiRoutes;

namespace Tweetbook.Services
{
    public class PostService : IPostService
    {
        private readonly DataContext _dataContext;

        public PostService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<Post>> GetPostsAsync(GetAllPostsFilter? filter = null, PaginationFilter? paginationFilter = null)
        {
            var queryable = _dataContext.Posts.AsQueryable();

            if (paginationFilter == null)
            {
                //filter = new PaginationFilter();
                return await queryable
                .Include(post => post.Tags)
                .ToListAsync();
            }

            if(filter != null)
            {
                queryable = AddFiltersOnQuery(filter, queryable);
            }

            // formula to determine how many records we need to skip in DB
            var skip = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;

            return await queryable
                .Include(post => post.Tags)
                .Skip(skip)
                .Take(paginationFilter.PageSize)
                .ToListAsync();
        }        

        public async Task<Post?> GetPostByIdAsync(Guid postId)
        {           
            return await _dataContext.Posts
                .Include(post => post.Tags)
                .SingleOrDefaultAsync(x => x.Id == postId);
        }

        public async Task<bool> UpdatePostAsync(Post post)
        {
            _dataContext.Posts.Update(post);
            var updated = await _dataContext.SaveChangesAsync();
            return updated > 0;
        }
       
        public async Task<bool> DeletePostAsync(Guid postId)
        {
            var post = await GetPostByIdAsync(postId);
            if (post == null)
                return false;

            DeletePostTagsForPostAsync(post);

            _dataContext.Posts.Remove(post);
            var deleted = await _dataContext.SaveChangesAsync();            
            return deleted > 0;
        }        

        public async Task<bool> CreatePostAsync(Post post)
        {
            await _dataContext.AddAsync(post);                        
            var created = await _dataContext.SaveChangesAsync();                       
            return created > 0;            
        }

        public async Task<bool> UserOwnsPostAsync(Guid postId, string userId)
        {
            var post = await _dataContext.Posts.AsNoTracking().SingleOrDefaultAsync(x => x.Id == postId); // this will avoid conflicts since we are going to update/delete if post is valid

            if(post == null)
            {
                return false;
            }

            if(post.UserId != userId)
            {
                return false;
            }

            return true;
        }

        public async Task<List<Tag>> GetAllTagsAsync()
        {
            return await _dataContext.Tags.ToListAsync();
        }

        public async Task<bool> CreateTagAsync(Tag tag)
        {
            await _dataContext.AddAsync(tag);
            var created = await _dataContext.SaveChangesAsync();
            return created > 0;
        }

        public async Task<Tag?> GetTagByIdAsync(Guid tagId)
        {
            return await _dataContext.Tags.SingleOrDefaultAsync(t => t.Id == tagId);
        }

        public async Task<Tag?> GetTagByNameAsync(string tagName)
        {
            return await _dataContext.Tags.SingleOrDefaultAsync(t => t.Name.Equals(tagName));
        }

        public async Task<bool> UpdateTagAsync(Tag tag)
        {
            _dataContext.Update(tag);
            var updated = await _dataContext.SaveChangesAsync();
            return updated > 0;
        }

        public async Task<bool> DeleteTagAsync(Guid tagId)
        {
            var tag = await GetTagByIdAsync(tagId);
            if (tag == null)
                return false;

            DeletePostTagsForTagAsync(tag);

            _dataContext.Remove(tag);
            var deleted = await _dataContext.SaveChangesAsync();
            return deleted > 0;
        }

        private void DeletePostTagsForPostAsync(Post post)
        {
            var postTags = _dataContext.PostTags.Where(x => x.Post == post);
            if (postTags.Any())
            {
                _dataContext.RemoveRange(postTags);
            }
        }

        private void DeletePostTagsForTagAsync(Tag tag)
        {
            var postTags = _dataContext.PostTags.Where(x => x.Tag == tag);
            if(postTags.Any())
            {
                _dataContext.RemoveRange(postTags);                
            }
        }

        private static IQueryable<Post> AddFiltersOnQuery(GetAllPostsFilter filter, IQueryable<Post> queryable)
        {
            if (!string.IsNullOrEmpty(filter?.UserId))
            {
                queryable = queryable.Where(x => x.UserId == filter.UserId);
            }
            return queryable;
        }
    }
}
