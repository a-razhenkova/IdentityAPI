using Application;
using AutoMapper;

namespace WebApi.V2
{
    public class CommonProfile : Profile
    {
        public CommonProfile()
        {
            CreateMap<TokenDto, TokenModel>();
        }
    }
}