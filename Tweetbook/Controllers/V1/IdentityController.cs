using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Tweetbook.Contracts.V1;
using Tweetbook.Contracts.V1.Requests;
using Tweetbook.Contracts.V1.Responses;
using Tweetbook.Services;

namespace Tweetbook.Controllers.V1
{
    [ApiController]    
    public class IdentityController : Controller
    {
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;

        public IdentityController(IIdentityService identityService, IMapper mapper)
        {
            _identityService = identityService;
            _mapper = mapper;
        }

        [HttpPost(ApiRoutes.Identity.Register)]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
        {
            var authResponse = await _identityService.RegistrationAsync(request.Email, request.Password);

            if (!authResponse.Success)
            {
                return BadRequest(_mapper.Map<AuthFailedResponse>(authResponse));
            }

            return Ok(_mapper.Map<AuthSuccessResponse>(authResponse));
        }

        [HttpPost(ApiRoutes.Identity.Login)]
        public async Task<IActionResult> Login([FromBody] UserRegistrationRequest request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }

            var authResponse = await _identityService.LoginAsync(request.Email, request.Password);

            if (!authResponse.Success)
            {
                return BadRequest(_mapper.Map<AuthFailedResponse>(authResponse));
            }

            return Ok(_mapper.Map<AuthSuccessResponse>(authResponse));
        }

        [HttpPost(ApiRoutes.Identity.Refresh)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var authResponse = await _identityService.RefreshTokenAsync(request.Token, request.RefreshToken);

            if (!authResponse.Success)
            {
                return BadRequest(_mapper.Map<AuthFailedResponse>(authResponse));
            }

            return Ok(_mapper.Map<AuthSuccessResponse>(authResponse));
        }
    }
}
