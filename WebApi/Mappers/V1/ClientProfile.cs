using Application;
using AutoMapper;

namespace WebApi.V1
{
    public class ClientProfile : Profile
    {
        public ClientProfile()
        {
            Create_CreateClientRequest_To_CreateClientCommand_Map();

            Create_UpdateClientRequest_To_UpdateClientCommand_Map();

            Create_ClientDto_To_ClientResponse_Map();

            CreateMap<PaginatedReportDto<ClientDto>, PaginatedReportDto<ClientResponse>>();
        }

        private void Create_CreateClientRequest_To_CreateClientCommand_Map()
        {
            CreateMap<CreateClientRequest, CreateClientCommand>();

            CreateMap<CreateClientRightRequest, CreateClientRightCommand>();
        }

        private void Create_UpdateClientRequest_To_UpdateClientCommand_Map()
        {
            CreateMap<UpdateClientRequest, UpdateClientCommand>();

            CreateMap<UpdateClientStatusRequest, UpdateClientStatusCommand>();

            CreateMap<UpdateClientRightRequest, UpdateClientRightCommand>();
        }

        private void Create_ClientDto_To_ClientResponse_Map()
        {
            CreateMap<ClientDto, ClientResponse>();

            CreateMap<ClientStatusDto, ClientStatusResponse>();

            CreateMap<ClientRightDto, ClientRightResponse>();

            CreateMap<ClientSubscriptionDto, ClientSubscriptionResponse>();
        }
    }
}