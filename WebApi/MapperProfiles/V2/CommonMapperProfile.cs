using Application;
using AutoMapper;

namespace WebApi.V2
{
    public class CommonMapperProfile : Profile
    {
        public CommonMapperProfile()
        {
            CreateMap<TokenDto, TokenModel>();
        }
    }
}