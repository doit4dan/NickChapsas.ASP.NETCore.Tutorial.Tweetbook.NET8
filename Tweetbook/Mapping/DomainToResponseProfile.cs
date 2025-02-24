using AutoMapper;
using Tweetbook.Contracts.V1.Responses;
using Tweetbook.Domain;

namespace Tweetbook.Mapping
{
    public class DomainToResponseProfile : Profile
    {
        public DomainToResponseProfile()
        {
            CreateMap<Post, PostResponse>()
                .ForMember(dest => dest.Tags, opt =>
                    opt.MapFrom(src => src.Tags.Select(x => new TagResponse { Id = x.TagId, Name = x.TagName })));

            CreateMap<Tag, TagResponse>();

            CreateMap<Domain.AuthenticationResult, AuthSuccessResponse>();

            CreateMap<Domain.AuthenticationResult, AuthFailedResponse>();
        }
    }
}
