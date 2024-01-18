using Abp.Application.Services.Dto;
using Abp.Linq.Extensions;
using Abp.UI;
using Ermes.Attributes;
using Ermes.Authorization;
using Ermes.CompetenceAreas;
using Ermes.Dto.Datatable;
using Ermes.Linq.Extensions;
using Ermes.Organizations.Dto;
using Ermes.Persons;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ermes.Organizations
{
    [ErmesAuthorize(AppPermissions.Backoffice)]
    public class OrganizationsAppService : ErmesAppServiceBase, IOrganizationsAppService
    {
        private readonly OrganizationManager _organizationManager;
        private readonly CompetenceAreaManager _compAreaManager;
        private readonly PersonManager _personManager;
        private readonly ErmesAppSession _session;
        private readonly ErmesPermissionChecker _permissionChecker;
        public OrganizationsAppService(
                OrganizationManager organizationManager,
                CompetenceAreaManager compAreaManager,
                PersonManager personManager,
                ErmesAppSession session,
                ErmesPermissionChecker permissionChecker
            )
        {
            _organizationManager = organizationManager;
            _compAreaManager = compAreaManager;
            _personManager = personManager;
            _session = session;
            _permissionChecker = permissionChecker;
        }

        #region Private Methods
        private async Task<int> CreateOrganization(OrganizationDto newOrganization)
        {
            var person = _session.LoggedUserPerson;

            if (!person.OrganizationId.HasValue)
            {
                if (!_permissionChecker.IsGranted(_session.Roles, AppPermissions.Organizations.Organization_CanCreate_Father))
                    throw new UserFriendlyException(L("MissingPermission", AppPermissions.Organizations.Organization_CanCreate_Father));
            }
            else
            {
                if (person.OrganizationId.Value != newOrganization.ParentId)
                    throw new UserFriendlyException(L("MissingPermission"));
            }

            if (!(await _organizationManager.CheckParent(newOrganization.ParentId)))
                throw new UserFriendlyException(L("InvalidParentId", newOrganization.ParentId.Value));

            var newOrg = ObjectMapper.Map<Organization>(newOrganization);

            var newOrgId = await _organizationManager.InsertOrganizationAsync(newOrg);
            Logger.Info("Ermes: CreateOrganization with new Id: " + newOrgId);
            return newOrgId;
        }

        private async Task<OrganizationDto> UpdateOrganization(OrganizationDto updatedOrganization)
        {
            if (!_permissionChecker.IsGranted(_session.Roles, AppPermissions.Organizations.Organization_CanUpdateAll))
            {
                if (
                    !_permissionChecker.IsGranted(_session.Roles, AppPermissions.Organizations.Organization_CanUpdate) ||
                    (_session.LoggedUserPerson.OrganizationId != updatedOrganization.Id && _session.LoggedUserPerson.OrganizationId != updatedOrganization.ParentId)
                    )
                    throw new UserFriendlyException(L("MissingPermission", AppPermissions.Organizations.Organization_CanUpdate));
            }

            if (!(await _organizationManager.CheckParent(updatedOrganization.ParentId)))
                throw new UserFriendlyException(L("InvalidParentId", updatedOrganization.ParentId.Value));

            var org = await _organizationManager.GetOrganizationByIdAsync(updatedOrganization.Id);

            ObjectMapper.Map(updatedOrganization, org);
            Logger.Info("Ermes: UpdateOrganization with name: " + updatedOrganization.Name);
            return updatedOrganization;
        }

        private async Task<PagedResultDto<OrganizationDto>> InternalGetOrganizations(GetOrganizationsInput input, bool filterByOrganization = true)
        {
            PagedResultDto<OrganizationDto> result = new PagedResultDto<OrganizationDto>();
            var query = _organizationManager.Organizations;

            query = query.DTFilterBy(input);

            var person = _session.LoggedUserPerson;

            filterByOrganization = !(_permissionChecker.IsGranted(_session.Roles, AppPermissions.Organizations.Organization_CanViewAll));

            if (filterByOrganization && person != null && person.OrganizationId.HasValue)
                query = query.DataOwnership(new List<int>() { person.OrganizationId.Value });

            result.TotalCount = await query.CountAsync();

            if (input?.Order != null && input.Order.Count == 0)
            {
                query = query.OrderBy(a => a.Name);
                query = query.PageBy(input);
            }
            else
            {
                query = query
                        .DTOrderedBy(input)
                        .PageBy(input);
            }

            var items = await query.ToListAsync();
            result.Items = ObjectMapper.Map<List<OrganizationDto>>(items);
            return result;
        }
        #endregion
        public virtual async Task<CreateOrUpdateOrganizationOutput> CreateOrUpdateOrganization(CreateOrUpdateOrganizationInput input)
        {
            var res = new CreateOrUpdateOrganizationOutput();
            if (input.Organization.Id == 0)
            {
                input.Organization.Id = await CreateOrganization(input.Organization);
                res.Organization = input.Organization;
            }
            else
                res.Organization = await UpdateOrganization(input.Organization);

            return res;
        }


        public virtual async Task<DTResult<OrganizationDto>> GetOrganizations(GetOrganizationsInput input)
        {
            PagedResultDto<OrganizationDto> result = await InternalGetOrganizations(input);
            return new DTResult<OrganizationDto>(input.Draw, result.TotalCount, result.Items.Count, result.Items.ToList());
        }

        public virtual async Task<bool> DeleteOrganization(DeleteOrganizationInput input)
        {
            var org = await _organizationManager.GetOrganizationByIdAsync(input.OrganizationId);
            if (org == null)
                throw new UserFriendlyException(L("InvalidOrganizationId", input.OrganizationId));

            if (!await _personManager.CanOrganizationBeDeletedAsync(input.OrganizationId))
                throw new UserFriendlyException(L("OrganizationCannotBeDeleted", input.OrganizationId));

            var person = _session.LoggedUserPerson;
            if (!person.OrganizationId.HasValue)
            {
                if (_permissionChecker.IsGranted(_session.Roles, AppPermissions.Organizations.Organization_CanDeleteCrossOrganization))
                    await _organizationManager.DeleteOrganizationAsync(input.OrganizationId);
                else
                    throw new UserFriendlyException(L("MissingPermission"));
            }
            else
            {
                if (person.OrganizationId.Value == org.Id || person.OrganizationId.Value == org.ParentId)
                    await _organizationManager.DeleteOrganizationAsync(input.OrganizationId);
                else
                    throw new UserFriendlyException(L("MissingPermission"));
            }

            Logger.Info("Ermes: DeleteOrganization with Id: " + input.OrganizationId + "by Person: " + person.Username);
            return true;
        }

        public virtual async Task<bool> AssignOrganizationToCompetenceAreas(AssignOrganizationToCompetenceAreasInput input)
        {
            var org = await _organizationManager.GetOrganizationByIdAsync(input.OrganizationId);

            //Delete old relations
            var oldCompAreas = await _organizationManager.GetOrganizationCompetenceAreaIdsByOrganizationId(org.Id);
            foreach (var item in oldCompAreas)
                await _organizationManager.DeleteOrganizationCompetenceAreaAsync(item);


            //Create new relations
            foreach (var compAreaId in input.CompetenceAreaIds)
            {
                //Check if CompetenceArea exists
                if (await _compAreaManager.CheckIfCompetenceAreaIdExists(compAreaId))
                {
                    OrganizationCompetenceArea oca = new OrganizationCompetenceArea()
                    {
                        OrganizationId = org.Id,
                        CompetenceAreaId = compAreaId
                    };

                    oca.Id = await _organizationManager.InsertOrganizationCompetenceAreaAsync(oca);
                }
                else
                    throw new UserFriendlyException(L("InvalidCompetenceAreaId", compAreaId));
            }
            Logger.Info("Ermes: AssignOrganizationToCompetenceAreas, OrgId: " + input.OrganizationId);

            return true;
        }

        public virtual async Task<bool> AssignPersonToOrganization(AssignPersonToOrganizationInput input)
        {
            if ((input.PersonId > 0 || input.PersonGuid != null) && input.OrganizationId > 0)
            {
                Person person = input.PersonId > 0 ? await _personManager.GetPersonByIdAsync(input.PersonId) : await _personManager.GetPersonByFusionAuthUserGuidAsync(input.PersonGuid);
                var organization = await _organizationManager.GetOrganizationByIdAsync(input.OrganizationId);
                if (person != null && organization != null)
                {
                    person.OrganizationId = organization.Id;
                    Logger.InfoFormat("Ermes: AssignPersonToOrganization, PersonId = {0}, OrganizationId = {1}", person.FusionAuthUserGuid, organization.Id);
                    return true;
                }
                else
                    throw new UserFriendlyException("Invalid PersonId or OrganizationId");
            }
            else
                throw new UserFriendlyException("Invalid PersonId or OrganizationId");
        }
    }
}
