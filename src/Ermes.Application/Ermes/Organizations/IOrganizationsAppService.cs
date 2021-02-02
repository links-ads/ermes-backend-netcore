using Ermes.Dto.Datatable;
using Ermes.Interfaces;
using Ermes.Organizations.Dto;
using System.Threading.Tasks;

namespace Ermes.Organizations
{
    public interface IOrganizationsAppService : IBackofficeApi
    {
        Task<DTResult<OrganizationDto>> GetOrganizations(GetOrganizationsInput input);
        Task<CreateOrUpdateOrganizationOutput> CreateOrUpdateOrganization(CreateOrUpdateOrganizationInput input);
        Task<bool> DeleteOrganization(DeleteOrganizationInput input);
        Task<bool> AssignOrganizationToCompetenceAreas(AssignOrganizationToCompetenceAreasInput input);
        Task<bool> AssignPersonToOrganization(AssignPersonToOrganizationInput input);
    }
}
