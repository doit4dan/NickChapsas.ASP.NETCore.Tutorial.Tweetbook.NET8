using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tweetbook.Contracts.V1;
using Tweetbook.Contracts.V1.Requests;
using Tweetbook.Contracts.V1.Responses;
using Tweetbook.Domain;
using Tweetbook.Extensions;
using Tweetbook.Services;

namespace Tweetbook.Controllers.V1
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // Roles accepts comma separated roles
    public class TagsController : Controller
    {
        private readonly IPostService _postService;

        public TagsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet(ApiRoutes.Tags.GetAll)]        
        public async Task<IActionResult> GetAll()
        {
            var tags = await _postService.GetAllTagsAsync();
            var tagResponses = tags
                .Select(tag => new TagResponse
                {
                    Id = tag.Id,
                    Name = tag.Name
                }).ToList();                   
            
            return Ok(tagResponses);
        }

        [HttpGet(ApiRoutes.Tags.Get)]
        public async Task<IActionResult> Get([FromRoute] Guid tagId)
        {
            var tag = await _postService.GetTagByIdAsync(tagId);
            if (tag == null)
                return NotFound();

            return Ok(new TagResponse { Id = tag.Id, Name = tag.Name });
        }

        [HttpPost(ApiRoutes.Tags.Create)]
        public async Task<IActionResult> Create([FromBody] CreateTagRequest request)
        {
            var userId = HttpContext.GetUserId();
            var tag = new Domain.Tag { Name = request.TagName, CreatedOn = DateTime.UtcNow, CreatorId = userId };
            
            var created = await _postService.CreateTagAsync(tag);
            if(!created)
            {
                return BadRequest(new { error = "Unable to create tag" });
            }

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = baseUrl + "/" + ApiRoutes.Tags.Get.Replace("{tagId}", tag.Id.ToString());
            return Created(locationUri, new TagResponse { Id = tag.Id, Name = tag.Name });            
        }

        [HttpPut(ApiRoutes.Tags.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid tagId, [FromBody] UpdateTagRequest request)
        {
            var tag = await _postService.GetTagByIdAsync(tagId);
            if (tag == null)
                return NotFound();

            tag.Name = request.TagName;
            var updated = await _postService.UpdateTagAsync(tag);
            if (updated)
                return Ok(new TagResponse { Id = tag.Id, Name = tag.Name });

            return NotFound();
        }

        [HttpDelete(ApiRoutes.Tags.Delete)]
        [Authorize(Policy = "MustWorkForApei")]
        public async Task<IActionResult> Delete([FromRoute] Guid tagId)
        {
            var deleted = await _postService.DeleteTagAsync(tagId);
            if (deleted)
                return NoContent();

            return NotFound();
        }
    }
}
