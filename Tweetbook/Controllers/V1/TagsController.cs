using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tweetbook.Contracts.V1;
using Tweetbook.Contracts.V1.Requests;
using Tweetbook.Extensions;
using Tweetbook.Services;

namespace Tweetbook.Controllers.V1
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Poster")] // Roles accepts comma separated roles
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
            return Ok(await _postService.GetAllTagsAsync());
        }

        [HttpGet(ApiRoutes.Tags.Get)]
        public async Task<IActionResult> Get([FromRoute] Guid tagId)
        {
            var tag = await _postService.GetTagByIdAsync(tagId);
            if (tag == null)
                return NotFound();

            return Ok(tag);
        }

        [HttpPost(ApiRoutes.Tags.Create)]
        public async Task<IActionResult> Create([FromBody] CreateTagRequest request)
        {
            var userId = HttpContext.GetUserId();
            var tag = new Domain.Tag { Name = request.TagName, CreatedOn = DateTime.UtcNow, CreatorId = userId };
            
            await _postService.CreateTagAsync(tag);
            
            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = baseUrl + "/" + ApiRoutes.Tags.Get.Replace("{tagId}", tag.Id.ToString());
            return Created(locationUri, tag);            
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
                return Ok(tag);

            return NotFound();
        }

        [HttpDelete(ApiRoutes.Tags.Delete)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] Guid tagId)
        {
            var deleted = await _postService.DeleteTagAsync(tagId);
            if (deleted)
                return NoContent();

            return NotFound();
        }
    }
}
