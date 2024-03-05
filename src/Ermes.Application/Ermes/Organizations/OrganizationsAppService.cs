using Abp.Application.Services.Dto;
using Abp.Linq.Extensions;
using Abp.UI;
using Ermes.Attributes;
using Ermes.Authorization;
using Ermes.Communications;
using Ermes.CompetenceAreas;
using Ermes.Dto.Datatable;
using Ermes.Gamification;
using Ermes.Linq.Extensions;
using Ermes.MapRequests;
using Ermes.Missions;
using Ermes.Notifications;
using Ermes.Operations;
using Ermes.Organizations.Dto;
using Ermes.Persons;
using Ermes.Reports;
using Ermes.Teams;
using FusionAuthNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NSwag.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ermes.Organizations
{
    [ErmesAuthorize(AppPermissions.Backoffice)]
    public class OrganizationsAppService : ErmesAppServiceBase, IOrganizationsAppService
    {
        private readonly OrganizationManager _organizationManager;
        private readonly TeamManager _teamManager;
        private readonly CompetenceAreaManager _compAreaManager;
        private readonly PersonManager _personManager;
        private readonly ErmesAppSession _session;
        private readonly ErmesPermissionChecker _permissionChecker;
        private readonly NotificationManager _notificationManager;
        private readonly OperationManager _operationManager;
        private readonly ReportManager _reportManager;
        private readonly MapRequestManager _mapRequestManager;
        private readonly CommunicationManager _communicationManager;
        private readonly IOptions<FusionAuthSettings> _fusionAuthSettings;
        private readonly MissionManager _missionManager;
        private readonly GamificationManager _gamificationManager;

        public OrganizationsAppService(
                OrganizationManager organizationManager,
                TeamManager teamManager,
                CompetenceAreaManager compAreaManager,
                PersonManager personManager,
                ErmesAppSession session,
                ErmesPermissionChecker permissionChecker,
                NotificationManager notificationManager,
                OperationManager operationManager,
                ReportManager reportManager,
                MapRequestManager mapRequestManager,
                CommunicationManager communicationManager,
                MissionManager missionManager,
                GamificationManager gamificationManager,
                IOptions<FusionAuthSettings> fusionAuthSettings
            )
        {
            _organizationManager = organizationManager;
            _teamManager = teamManager;
            _compAreaManager = compAreaManager;
            _personManager = personManager;
            _session = session;
            _permissionChecker = permissionChecker;
            _fusionAuthSettings = fusionAuthSettings;
            _notificationManager = notificationManager;
            _operationManager = operationManager;
            _reportManager = reportManager;
            _communicationManager = communicationManager;
            _mapRequestManager = mapRequestManager;
            _missionManager = missionManager;
            _gamificationManager = gamificationManager;
        }

        #region Private Methods
        private async Task<int> CreateOrganization(OrganizationDto newOrganization)
        {
            var person = _session.LoggedUserPerson;

            if (!person.OrganizationId.HasValue)
            {
                if (!_permissionChecker.IsGranted(_session.Roles, AppPermissions.Organizations.Organization_CanCreate_Parent))
                    throw new UserFriendlyException(L("MissingPermission", AppPermissions.Organizations.Organization_CanCreate_Parent));
            }
            else
            {
                if (!_permissionChecker.IsGranted(_session.Roles, AppPermissions.Organizations.Organization_CanCreate_Child))
                    throw new UserFriendlyException(L("MissingPermission", AppPermissions.Organizations.Organization_CanCreate_Child));
                if (person.OrganizationId.Value != newOrganization.ParentId)
                    throw new UserFriendlyException(L("MissingPermission"));
            }

            if (!(await _organizationManager.CheckParent(newOrganization.ParentId)))
                throw new UserFriendlyException(L("InvalidParentId", newOrganization.ParentId.Value));

            var newOrg = ObjectMapper.Map<Organization>(newOrganization);
            newOrg.IsActive = true;
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

        private async Task<bool> BulkDeleteUsersAsync(int organizationId)
        {
            var client = FusionAuth.GetFusionAuthClient(_fusionAuthSettings.Value);
            var persons = await _personManager.GetPersonsByOrganizationIdAsync(organizationId);
            var response = await client.DeleteUsersByQueryAsync(new io.fusionauth.domain.api.UserDeleteRequest()
            {
                hardDelete = true,
                userIds = persons.Select(a => a.FusionAuthUserGuid).ToList()
            });

            if (response.WasSuccessful())
            {
                return true;
            }
            else
            {
                var fa_error = FusionAuth.ManageErrorResponse(response);
                throw new UserFriendlyException(fa_error.ErrorCode, fa_error.HasTranslation ? L(fa_error.Message) : fa_error.Message);
            }
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

        [OpenApiOperation("Delete Organization",
            @"
                Remove:
                    - organization (soft)
                    - teams (hard)
                    - members of the organization (soft)
                Input: OrganizationId
                Output: true if the operation completes with success
                Note: Admin can delete father organizations, 
                while organization managers can only delete organizations that are child of the org they belong to
            "
        )]
        public virtual async Task<bool> DeleteOrganization(DeleteOrganizationInput input)
        {
            var org = await _organizationManager.GetOrganizationByIdAsync(input.OrganizationId);
            if (org == null)
                throw new UserFriendlyException(L("InvalidOrganizationId", input.OrganizationId));

            if (!_permissionChecker.IsGranted(_session.Roles, AppPermissions.Organizations.Organization_CanDelete))
                throw new UserFriendlyException(L("MissingPermission"));

            var person = _session.LoggedUserPerson;

            if (person.OrganizationId.HasValue && person.OrganizationId.Value != org.ParentId)
                throw new UserFriendlyException(L("MissingPermission"));
            else
            {
                if (!_permissionChecker.IsGranted(_session.Roles, AppPermissions.Organizations.Organization_CanDeleteCrossOrganization))
                    throw new UserFriendlyException(L("MissingPermission"));
                if (!await _personManager.CanOrganizationBeDeletedAsync(input.OrganizationId))
                    throw new UserFriendlyException(L("OrganizationCannotBeDeleted", input.OrganizationId));
            }


            var persons = await _personManager.GetPersonsByOrganizationIdAsync(input.OrganizationId);

            foreach (var item in persons)
            {
                item.TeamId = null;
                item.Team = null;
            }
            var teams = await _teamManager.GetTeamsByOrganizationIdAsync(input.OrganizationId);
            foreach (var item in teams)
            {
                await _teamManager.DeleteTeamAsync(item.Id);
                Logger.Info(string.Format("Ermes: Deleted Team {0} with Id {1}", item.Name, item.Id));
            }

            foreach (var item in persons)
            {
                await DeleteUserInternalAsync(
                        item.FusionAuthUserGuid,
                        item,
                        false,
                        _notificationManager,
                        _operationManager,
                        _reportManager,
                        _communicationManager,
                        _mapRequestManager,
                        _missionManager,
                        _personManager,
                        _gamificationManager,
                        _fusionAuthSettings
                    );
                Logger.Info(string.Format("Ermes: Deleted Person {0} with Id {1}", item.Username, item.Id));
            }

            org.IsActive = false;
            Logger.Info(string.Format("Ermes: De-activate Organization {0} with Id {1} by {2}", org.Name, input.OrganizationId, person.Username));

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
