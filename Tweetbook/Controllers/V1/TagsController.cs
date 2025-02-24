using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tweetbook.Contracts.V1;
using Tweetbook.Contracts.V1.Requests;
using Tweetbook.Contracts.V1.Requests.Queries;
using Tweetbook.Contracts.V1.Responses;
using Tweetbook.Domain;
using Tweetbook.Extensions;
using Tweetbook.Helpers;
using Tweetbook.Services;

namespace Tweetbook.Controllers.V1
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // Roles accepts comma separated roles
    [Produces("application/json")]
    public class TagsController : Controller
    {
        private readonly IPostService _postService;
        private readonly IUriService _uriService;
        private readonly IMapper _mapper;

        public TagsController(IPostService postService, IUriService uriService, IMapper mapper)
        {
            _postService = postService;
            _uriService = uriService;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns all the tags in the system
        /// </summary>
        /// <response code="200">Returns all the tags in the system</response>
        [HttpGet(ApiRoutes.Tags.GetAll)]        
        public async Task<IActionResult> GetAll([FromQuery]GetAllTagsQuery query, [FromQuery]PaginationQuery paginationQuery)
        {
            var filter = _mapper.Map<GetAllTagsFilter>(query);
            var pagination = _mapper.Map<PaginationFilter>(paginationQuery);            
            var tags = await _postService.GetAllTagsAsync(filter, pagination);
            var tagsResponse = _mapper.Map<List<TagResponse>>(tags);

            if(pagination == null || pagination.PageNumber < 1 || pagination.PageSize < 1)
            {
                return Ok(new PagedResponse<TagResponse>(tagsResponse));
            }

            var paginatedResponse = PaginationHelpers.CreatePaginatedResponse<TagResponse>(_uriService, pagination, tagsResponse);
            return Ok(paginatedResponse);
        }

        [HttpGet(ApiRoutes.Tags.Get)]
        public async Task<IActionResult> Get([FromRoute] Guid tagId)
        {
            var tag = await _postService.GetTagByIdAsync(tagId);
            if (tag == null)
                return NotFound();

            return Ok(new Response<TagResponse>(_mapper.Map<TagResponse>(tag)));
        }

        /// <summary>
        /// Creates a tag in the system
        /// </summary>       
        /// <response code="201">Creates a tag in the system</response>
        /// <response code="400">Unable to create the tag due to validation error</response>
        [HttpPost(ApiRoutes.Tags.Create)]
        [ProducesResponseType(typeof(TagResponse),201)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> Create([FromBody] CreateTagRequest request)
        {
            var userId = HttpContext.GetUserId();
            var tag = new Domain.Tag { Name = request.TagName, CreatedOn = DateTime.UtcNow, CreatorId = userId };
            
            var created = await _postService.CreateTagAsync(tag);
            if(!created)
            {
                return BadRequest(new ErrorResponse 
                { 
                    Errors = new List<ErrorModel> 
                    { 
                        new ErrorModel 
                        { 
                            Message = "Unable to create tag" 
                        }
                    } 
                });
            }
            var locationUri = _uriService.GetTagUri(tag.Id.ToString());
            //var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            //var locationUri = baseUrl + "/" + ApiRoutes.Tags.Get.Replace("{tagId}", tag.Id.ToString());
            return Created(locationUri, new Response<TagResponse>(_mapper.Map<TagResponse>(tag)));            
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
                return Ok(new Response<TagResponse>(_mapper.Map<TagResponse>(tag)));

            return NotFound();
        }

        [HttpDelete(ApiRoutes.Tags.Delete)]
        //[Authorize(Policy = "MustWorkForApei")]
        public async Task<IActionResult> Delete([FromRoute] Guid tagId)
        {
            var deleted = await _postService.DeleteTagAsync(tagId);
            if (deleted)
                return NoContent();

            return NotFound();
        }
    }
}
