using Abp.Application.Services;
using Ermes.Communications.Dto;
using Ermes.Dto;
using Ermes.Dto.Datatable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Communications
{
    public interface ICommunicationsAppService : IApplicationService
    {
        Task<DTResult<CommunicationDto>> GetCommunications(GetCommunicationsInput input);
        Task<GetEntityByIdOutput<CommunicationDto>> GetCommunicationById(GetEntityByIdInput<int> input);
        Task<CreateOrUpdateCommunicationOutput> CreateOrUpdateCommunication(CreateOrUpdateCommunicationInput input);
        Task<bool> DeleteCommunication(IdInput<int> input);
    }
}
