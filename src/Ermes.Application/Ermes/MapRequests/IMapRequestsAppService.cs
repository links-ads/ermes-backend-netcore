using Abp.Application.Services;
using Ermes.Dto;
using Ermes.Dto.Datatable;
using Ermes.MapRequests.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.MapRequests
{
    public interface IMapRequestsAppService: IApplicationService
    {
        Task<DTResult<MapRequestDto>> GetMapRequests(GetMapRequestsInput input);
        Task<GetEntityByIdOutput<MapRequestDto>> GetMapRequestById(GetEntityByIdInput<int> input);
        Task<CreateOrUpdateMapRequestOutput> CreateOrUpdateMapRequest(CreateOrUpdateMapRequestInput input);
        Task<object> DeleteMapRequest(string mapRequestCode);
    }
}
