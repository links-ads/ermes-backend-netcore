using Abp.Application.Services;
using Ermes.Actions.Dto;
using Ermes.Dto.Datatable;
using System.Threading.Tasks;

namespace Ermes.Actions
{
    public interface IActionsAppService : IApplicationService
    {
        Task<CreatePersonActionOutput> CreatePersonAction(CreatePersonActionInput input);
        //Task<GetActionsOutput> GetActions(GetActionsInput input);
        Task<DTResult<PersonActionDto>> GetActions(GetActionsInput input);
    }
}
