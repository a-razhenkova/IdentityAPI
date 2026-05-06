using AutoMapper;
using Domain;

namespace Application
{
    public class DomainMapperProfile : Profile
    {
        public DomainMapperProfile()
        {
            CreateClientMaps();
            CreateUserMaps();
        }

        private void CreateClientMaps()
        {
            CreateMap<ClientDto, Client>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Version, opt => opt.Ignore())
                .ForMember(dest => dest.Key, opt => opt.Ignore())
                .ForMember(dest => dest.Secret, opt => opt.Ignore())
                .ForMember(dest => dest.Subscriptions, opt => opt.Ignore())
                .ForMember(dest => dest.WrongLoginAttemptsCounter, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<ClientStatusDto, ClientStatus>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Version, opt => opt.Ignore())
                .ForMember(dest => dest.ClientId, opt => opt.Ignore())
                .ForMember(dest => dest.Client, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<ClientRightDto, ClientRight>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Version, opt => opt.Ignore())
                .ForMember(dest => dest.ClientId, opt => opt.Ignore())
                .ForMember(dest => dest.Client, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<ClientSubscription, ClientSubscriptionDto>()
                .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(src => src.Subscription.ExpirationDate))
                .ForMember(dest => dest.ContractId, opt => opt.MapFrom(src => src.Subscription.Contract.Id))
                .ForMember(dest => dest.ContractName, opt => opt.MapFrom(src => src.Subscription.Contract.Name))
                .ReverseMap();
        }

        private void CreateUserMaps()
        {
            CreateMap<UserDto, User>()
               .ForMember(dest => dest.Id, opt => opt.Ignore())
               .ForMember(dest => dest.Version, opt => opt.Ignore())
               .ForMember(dest => dest.PublicId, opt => opt.Ignore())
               .ForMember(dest => dest.OtpSecret, opt => opt.Ignore())
               .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(src => false))
               .ForMember(dest => dest.Password, opt => opt.Ignore())
               .ForMember(dest => dest.Login, opt => opt.Ignore())
               .ReverseMap();
           
            CreateMap<UserStatusDto, UserStatus>()
               .ForMember(dest => dest.Id, opt => opt.Ignore())
               .ForMember(dest => dest.Version, opt => opt.Ignore())
               .ForMember(dest => dest.UserId, opt => opt.Ignore())
               .ForMember(dest => dest.User, opt => opt.Ignore())
               .ReverseMap();
        }
    }
}