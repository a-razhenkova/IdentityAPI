using Application;
using AutoMapper;

namespace WebApi.V2
{
    public class TokenProfile : Profile
    {
        public TokenProfile()
        {
            CreateMap<TokenDto, TokenResponse>();
        }
    }
}