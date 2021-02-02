using Abp.Application.Services;
using Ermes.Dto;
using Ermes.Dto.Datatable;
using Ermes.Teams.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Teams
{
    public interface ITeamsAppService: IApplicationService
    {
        Task<TeamOutputDto> GetTeamById(IdInput<int> input);
        Task<DTResult<TeamOutputDto>> GetTeams(GetTeamsInput input);
        Task<int> CreateOrUpdateTeam(CreateUpdateTeamInput input);
        Task<bool> DeleteTeam(IdInput<int> input);
        Task<bool> SetTeamMembers(SetTeamMembersInput input);

    }
}
