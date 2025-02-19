﻿using Microsoft.EntityFrameworkCore;
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

        public async Task<List<Post>> GetPostsAsync()
        {            
            return await _dataContext.Posts
                .Include(post => post.Tags)
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
    }
}
