using Abp.Application.Services;
using Ermes.Alerts.Dto;
using Ermes.Dto;
using Ermes.Dto.Datatable;
using System.Threading.Tasks;

namespace Ermes.Ermes.Alerts
{
    public interface IAlertsAppService : IApplicationService
    {
        Task<DTResult<AlertDto>> GetAlerts(GetAlertsInput input);
        Task<GetEntityByIdOutput<AlertDto>> GetAlertById(GetEntityByIdInput<int> input);
    }
}
