using Application;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;

namespace WebApi.V1
{
    public class TokenProfile : Profile
    {
        public TokenProfile()
        {
            CreateMap<TokenDto, TokenResponse>();

            CreateMap<TokenValidationResult, TokenValidationResultResponse>()
                .ForMember(dest => dest.IsValid, opt => opt.MapFrom(src => src.IsValid))
                .ForMember(dest => dest.Exception, opt =>
                {
                    opt.PreCondition(src => src.Exception is not null);
                    opt.MapFrom(src => src.Exception.Message);
                });
        }
    }
}