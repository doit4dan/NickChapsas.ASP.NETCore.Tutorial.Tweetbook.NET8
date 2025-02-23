using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Validations;
using Tweetbook.Cache;
using Tweetbook.Contracts.V1;
using Tweetbook.Contracts.V1.Requests;
using Tweetbook.Contracts.V1.Responses;
using Tweetbook.Domain;
using Tweetbook.Extensions;
using Tweetbook.Services;

namespace Tweetbook.Controllers.V1
{
    [ApiController]        
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PostsController : Controller
    {
        private IPostService _postService;
        private IMapper _mapper;
        public PostsController(IPostService postService, IMapper mapper)
        {
            _postService = postService;
            _mapper = mapper;
        }
        
        [HttpGet(ApiRoutes.Posts.GetAll)]
        [Cached(600)]
        public async Task<IActionResult> GetAll()
        {
            var posts = await _postService.GetPostsAsync();            
            return Ok(_mapper.Map<List<PostResponse>>(posts));
        }        

        [HttpGet(ApiRoutes.Posts.Get)]
        [Cached(600)]
        public async Task<IActionResult> Get([FromRoute]Guid postId)
        {           
            var post = await _postService.GetPostByIdAsync(postId);

            if (post == null)
                return NotFound();

            return Ok(_mapper.Map<PostResponse>(post));
        }                

        [HttpPut(ApiRoutes.Posts.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid postId, [FromBody] UpdatePostRequest request)
        {
            bool userOwnsPost = await _postService.UserOwnsPostAsync(postId, HttpContext.GetUserId());

            if(!userOwnsPost)
            {
                return BadRequest(new { error = "You do not own this post" }); // we will improve this in future
            }

            // we can refactor this, because if post does not exist, it will return above
            var post = await _postService.GetPostByIdAsync(postId);

            if (post == null)
                return NotFound();

            post.Name = request.Name;

            var updated = await _postService.UpdatePostAsync(post);

            if (updated)
                return Ok(_mapper.Map<PostResponse>(post));

            return NotFound();
        }

        [HttpDelete(ApiRoutes.Posts.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid postId)
        {
            bool userOwnsPost = await _postService.UserOwnsPostAsync(postId, HttpContext.GetUserId());

            if (!userOwnsPost)
            {
                return BadRequest(new { error = "You do not own this post" }); // we will improve this in future
            }

            var deleted = await _postService.DeletePostAsync(postId);

            if (deleted) // two ways to delete: Return Ok with full body of deleted item. Or you can return 204, no content
                return NoContent();

            return NotFound();
        }

        [HttpPost(ApiRoutes.Posts.Create)]
        public async Task<IActionResult> Create([FromBody] CreatePostRequest request)
        {
            var userId = HttpContext.GetUserId();
            var newPostId = Guid.NewGuid();
            var post = new Post
            {
                Id = newPostId,
                Name = request.Name,
                UserId = userId                
            };
            // add tags to post
            request.Tags.ForEach(t => post.Tags.Add(new PostTag
            {
                PostId = newPostId,
                TagName = t,
                Tag = new Tag { Name = t, CreatorId = userId, CreatedOn = DateTime.UtcNow }
            }));

            await _postService.CreatePostAsync(post);

            // gives us absolute URL
            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = baseUrl + "/" + ApiRoutes.Posts.Get.Replace("{postId}", post.Id.ToString());            
            return Created(locationUri, _mapper.Map<PostResponse>(post));
        }
    }
}
