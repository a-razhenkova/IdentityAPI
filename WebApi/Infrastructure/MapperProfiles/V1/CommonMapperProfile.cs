using Application;
using AutoMapper;
using Domain;
using Microsoft.IdentityModel.Tokens;

namespace WebApi.V1
{
    public class CommonMapperProfile : Profile
    {
        public CommonMapperProfile()
        {
            CreateMap<TokenDto, TokenModel>();

            CreateMap<TokenValidationResult, TokenValidationResultModel>()
                .ForMember(dest => dest.IsValid, opt => opt.MapFrom(src => src.IsValid))
                .ForMember(dest => dest.Exception, opt =>
                {
                    opt.PreCondition(src => src.Exception is not null);
                    opt.MapFrom(src => src.Exception.Message);
                });

            CreatClientMaps();
            CreateUserMaps();
        }

        private void CreatClientMaps()
        {
            CreateMap<ClientRegistrationModel, ClientDto>()
                .ForMember(dest => dest.Key, opt => opt.Ignore())
                .ForMember(dest => dest.Subscriptions, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => new ClientStatusDto()
                {
                    Value = src.IsInternal ? ClientStatuses.Active : ClientStatuses.Disabled,
                    Reason = src.IsInternal ? ClientStatusReasons.None : ClientStatusReasons.ExpiredSubscription
                }));

            CreateMap<ClientUpdateModel, ClientDto>()
                .ForMember(dest => dest.Key, opt => opt.Ignore())
                .ForMember(dest => dest.Subscriptions, opt => opt.Ignore());

            CreateMap<ClientStatusModel, ClientStatusDto>()
                .ReverseMap();

            CreateMap<ClientRightModel, ClientRightDto>()
                .ReverseMap();

            CreateMap<ClientSubscriptionModel, ClientSubscriptionDto>()
                .ReverseMap();

            CreateMap<ClientDto, ClientModel>()
                .ReverseMap();

            CreateMap<PaginatedReport<ClientDto>, PaginatedReport<ClientModel>>();
        }

        private void CreateUserMaps()
        {
            CreateMap<UserRegistrationModel, UserDto>()
                .ForMember(dest => dest.PublicId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => new UserStatusDto()
                {
                    Value = UserStatuses.Restricted,
                    Reason = UserStatusReasons.NewUser
                }))
                .ForMember(dest => dest.RegistrationTimestamp, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<UserUpdateModel, UserDto>()
                .ForMember(dest => dest.PublicId, opt => opt.Ignore())
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.RegistrationTimestamp, opt => opt.Ignore());

            CreateMap<UserStatusModel, UserStatusDto>()
                .ReverseMap();

            CreateMap<UserDto, UserModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PublicId))
                .ReverseMap();

            CreateMap<PaginatedReport<UserDto>, PaginatedReport<UserModel>>();
        }
    }
}