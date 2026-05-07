using Application.RabbitMq;
using AutoMapper;
using Domain;

namespace Application
{
    public class EventsProfile : Profile
    {
        public EventsProfile()
        {
            UserPasswordChangedEventMaps();
        }
        private void UserPasswordChangedEventMaps()
        {
            CreateMap<User, UserPasswordChangedEvent>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Password.LastChangedTimestamp))
                .ForMember(dest => dest.UserIpAddress, opt => opt.Ignore());
        }
    }
}