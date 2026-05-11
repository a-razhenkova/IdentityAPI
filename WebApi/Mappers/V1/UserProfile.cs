using Application;
using AutoMapper;

namespace WebApi.V1
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<CreateUserRequest, CreateUserCommand>();

            Create_UpdateUserRequest_To_UpdateUserCommand_Map();

            Create_UserDto_To_UserResponse_Map();

            CreateMap<PaginatedReportDto<UserDto>, PaginatedReportResponse<UserResponse>>();
        }

        private void Create_UpdateUserRequest_To_UpdateUserCommand_Map()
        {
            CreateMap<UpdateUserRequest, UpdateUserCommand>();

            CreateMap<UpdateUserStatusRequest, UpdateUserStatusCommand>();
        }

        private void Create_UserDto_To_UserResponse_Map()
        {
            CreateMap<UserDto, UserResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

            CreateMap<UserStatusDto, UserStatusResponse>();
        }
    }
}