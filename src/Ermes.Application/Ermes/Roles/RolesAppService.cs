using Abp.UI;
using Ermes.Attributes;
using Ermes.Authorization;
using Ermes.Organizations;
using Ermes.Permissions;
using Ermes.Persons;
using Ermes.Roles.Dto;
using Ermes.Teams;
using FusionAuthNetCore;
using io.fusionauth;
using io.fusionauth.domain;
using io.fusionauth.domain.api.user;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ermes.Roles
{
    public class RolesAppService : ErmesAppServiceBase, IRolesAppService
    {
        private readonly PermissionManager _permissionManager;
        private readonly IOptions<FusionAuthSettings> _fusionAuthSettings;
        private readonly PersonManager _personManager;
        private readonly OrganizationManager _organizationManager;
        private readonly TeamManager _teamManager;
        private readonly ErmesPermissionChecker _permissionChecker;
        private readonly ErmesAppSession _session;
        public RolesAppService(
                    PermissionManager permissionManager,
                    PersonManager personManager,
                    IOptions<FusionAuthSettings> fusionAuthSettings,
                    OrganizationManager organizationManager,
                    TeamManager teamManager,
                    ErmesPermissionChecker permissionChecker,
                    ErmesAppSession session
            )
        {
            _permissionManager = permissionManager;
            _fusionAuthSettings = fusionAuthSettings;
            _personManager = personManager;
            _organizationManager = organizationManager;
            _session = session;
            _permissionChecker = permissionChecker;
            _teamManager = teamManager;
        }


        [ErmesAuthorize(AppPermissions.Backoffice)]
        public virtual async Task<GetRolesOutput> GetRoles()
        {
            var list = await _permissionManager.GetRolesAsync();

            return new GetRolesOutput()
            {
                Roles = ObjectMapper.Map<List<RoleDto>>(list)
            };
        }

        [ErmesAuthorize(AppPermissions.Backoffice)]
        public virtual async Task<bool> AssignPermissionToRole(AssignPermissionToRoleInput input)
        {
            var permission = ObjectMapper.Map<ErmesPermission>(input.Permission);
            var res = await _permissionManager.AssignPermissionToRoleAsync(permission);
            if (res == -1)
                throw new UserFriendlyException(string.Format("Permission {0} already associated to Role {1}", input.Permission.Name, input.Permission.RoleId));
            else if (res == -2)
                throw new UserFriendlyException(string.Format("Role with Id {0} doesn't exist", permission.RoleId));
            else
            {
                Logger.InfoFormat("Ermes: AssignPermissionToRole, Permission = {0}, RoleId = {1}", input.Permission.Name, input.Permission.RoleId);
                return true;
            }
        }

        [ErmesAuthorize(AppPermissions.Backoffice)]
        public virtual async Task<bool> DeletePermissionForRole(DeletePermissionForRoleInput input)
        {
            var permission = ObjectMapper.Map<ErmesPermission>(input.Permission);
            if (await _permissionManager.DeletePermissionAsync(permission) < 0)
                throw new UserFriendlyException(string.Format("Permission {0} not associated to Role Id {1}", input.Permission.Name, input.Permission.RoleId));
            Logger.InfoFormat("Ermes: DeletePermissionForRole, Permission = {0}, RoleId = {1}", input.Permission.Name, input.Permission.RoleId);
            return true;
        }


        public virtual async Task<bool> SyncRolesFromFusionAuth()
        {
            List<Role> list = await _permissionManager.GetRolesAsync();
            Dictionary<String, Role> roleMap = list.ToDictionary(r => r.Name);

            FusionAuthClient client = FusionAuth.GetFusionAuthClient(_fusionAuthSettings.Value);

            var response = await client.RetrieveApplicationAsync(new Guid(_fusionAuthSettings.Value.ApplicationId));
            if (response.statusCode == 200)
            {
                foreach (ApplicationRole appRole in response.successResponse.application.roles)
                {
                    if (roleMap.ContainsKey(appRole.name))
                    {
                        ObjectMapper.Map(appRole, roleMap[appRole.name]);
                        roleMap[appRole.name] = null;
                    }
                    else
                        await _permissionManager.CreateOrUpdateRoleAsync(ObjectMapper.Map<Role>(appRole));
                }
                List<string> rolesToDelete = roleMap.Where(kv => kv.Value != null).Select(kv => kv.Value.Name).ToList();
                if (rolesToDelete.Count > 0)
                {
                    throw new UserFriendlyException("Some roles have been deleted from the FusionAuth remote. Please restore them: " + rolesToDelete.Aggregate((s1, s2) => s1 + ", " + s2));
                }
            }
            else
            {
                throw new UserFriendlyException(L("FusionAuthUnknonwError"));
            }
            return true;
        }

        [ErmesAuthorize(AppPermissions.Backoffice)]
        public virtual async Task<bool> AssignRolesToPerson(AssignRolesToPersonInput input)
        {
            Person person = input.PersonId > 0 ? await _personManager.GetPersonByIdAsync(input.PersonId) : await _personManager.GetPersonByFusionAuthUserGuidAsync(input.PersonGuid);

            if (person == null)
                throw new UserFriendlyException(L("InvalidPersonId"));

            List<Role> rolesToAssign = await GetRolesAndCheckOrganizationAndTeam(input.Roles, person.OrganizationId, person.TeamId, input.PersonId, _personManager, _organizationManager, _teamManager, _session, _permissionChecker);

            FusionAuthClient client = FusionAuth.GetFusionAuthClient(_fusionAuthSettings.Value);

            RegistrationRequest regReq = new RegistrationRequest()
            {
                registration = new UserRegistration()
                {
                    applicationId = new Guid(_fusionAuthSettings.Value.ApplicationId),
                    roles = rolesToAssign.Select(r => r.Name).ToList()
                }
            };

            ClientResponse<RegistrationResponse> response = await client.UpdateRegistrationAsync(person.FusionAuthUserGuid, regReq);
            if (response.statusCode == 200)
            {
                await _personManager.DeletePersonRolesAsync(person.Id);
                //Need to manually save changes, otherwise concurrency issues between delete and create
                CurrentUnitOfWork.SaveChanges();
                foreach (Role rta in rolesToAssign)
                {
                    PersonRole pr = new PersonRole()
                    {
                        PersonId = person.Id,
                        RoleId = rta.Id
                    };
                    await _personManager.InsertPersonRoleAsync(pr);
                }
                return true;
            }
            else
            {
                var fa_error = FusionAuth.ManageErrorResponse(response);
                throw new UserFriendlyException(fa_error.ErrorCode, fa_error.HasTranslation ? L(fa_error.Message) : fa_error.Message);
            }
        }

        public virtual async Task<bool> InitilizePermissions()
        {
            //step 1) delete current role-permission associations
            await _permissionManager.DeleteAllPermissionsAsync();
            await CurrentUnitOfWork.SaveChangesAsync();

            //step 2) create new role-permission associations
            var permList = AppRoles.ADMINISTRATOR_PERMISSION_LIST;

            Role role = await _permissionManager.GetRoleByNameAsync(AppRoles.ADMINISTRATOR);
            foreach (var perm in permList)
            {
                await _permissionManager.AssignPermissionToRoleAsync(new ErmesPermission(perm, role.Id));
            }

            permList = AppRoles.DECISION_MAKER_PERMISSION_LIST;
            role = await _permissionManager.GetRoleByNameAsync(AppRoles.DECISION_MAKER);
            foreach (var perm in permList)
            {
                await _permissionManager.AssignPermissionToRoleAsync(new ErmesPermission(perm, role.Id));
            }

            permList = AppRoles.ORGANIZATION_MANAGER_PERMISSION_LIST;
            role = await _permissionManager.GetRoleByNameAsync(AppRoles.ORGANIZATION_MANAGER);
            foreach (var perm in permList)
            {
                await _permissionManager.AssignPermissionToRoleAsync(new ErmesPermission(perm, role.Id));
            }

            permList = AppRoles.FIRST_RESPONDER_PERMISSION_LIST;
            role = await _permissionManager.GetRoleByNameAsync(AppRoles.FIRST_RESPONDER);
            foreach (var perm in permList)
            {
                await _permissionManager.AssignPermissionToRoleAsync(new ErmesPermission(perm, role.Id));
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            return true;
        }


    }
}
