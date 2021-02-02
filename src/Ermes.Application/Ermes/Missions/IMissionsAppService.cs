using Abp.Application.Services;
using Ermes.Dto;
using Ermes.Dto.Datatable;
using Ermes.Dto.Spatial;
using Ermes.Missions.Dto;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using System.Threading.Tasks;

namespace Ermes.Missions
{
    public interface IMissionsAppService : IApplicationService
    {
        Task<DTResult<MissionDto>> GetMissions(GetMissionsInput input);
        Task<GetEntityByIdOutput<MissionDto>> GetMissionById(GetEntityByIdInput<int> input);
        /*Task<GetMissionWithHistoryOutput> GetMissionWithHistory(IdInput input);*/
        Task<bool> UpdateMissionStatus(UpdateMissionStatusInput input);
        Task<int> CreateOrUpdateMission(CreateOrUpdateMissionInput input);
        Task<bool> DeleteMission(IdInput<int> input);
    }
}
