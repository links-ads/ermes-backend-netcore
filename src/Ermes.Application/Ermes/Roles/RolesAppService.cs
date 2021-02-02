using Abp.ObjectMapping;
using Abp.UI;
using Ermes.Attributes;
using Ermes.Authorization;
using Ermes.Roles.Dto;
using Ermes.Roles;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Ermes.Permissions;
using Microsoft.Extensions.Options;
using FusionAuthNetCore;
using io.fusionauth;
using System.Linq;
using io.fusionauth.domain;
using Ermes.Persons;
using io.fusionauth.domain.api.user;
using Ermes.Organizations;

namespace Ermes.Roles
{
    //[FasterAuthorize(AppPermissions.Backoffice)]
    public class RolesAppService : ErmesAppServiceBase, IRolesAppService
    {
        private readonly IObjectMapper _objectMapper;
        private readonly PermissionManager _permissionManager;
        private readonly IOptions<FusionAuthSettings> _fusionAuthSettings;
        private readonly PersonManager _personManager;
        private readonly OrganizationManager _organizationManager;
        private readonly ErmesAppSession _session;
        public RolesAppService(
                    IObjectMapper objectMapper,
                    PermissionManager permissionManager,
                    PersonManager personManager,
                    IOptions<FusionAuthSettings> fusionAuthSettings,
                    OrganizationManager organizationManager,
                    ErmesAppSession session
            )
        {
            _objectMapper = objectMapper;
            _permissionManager = permissionManager;
            _fusionAuthSettings = fusionAuthSettings;
            _personManager = personManager;
            _organizationManager = organizationManager;
            _session = session;
        }

        /*public virtual async Task<CreateOrUpdateRoleOutput> CreateOrUpdateRole(CreateOrUpdateRoleInput input)
        {
            var role = _objectMapper.Map<Role>(input.Role);
            int newRoleId = await _permissionManager.CreateOrUpdateRoleAsync(role);
            if (newRoleId < 0)
                throw new UserFriendlyException(string.Format("Role with name {0} already exists", input.Role.Name));
            else
                input.Role.Id = newRoleId;

            Logger.Info("Ermes: CreateOrUpdateRole with Id: " + newRoleId);
            return new CreateOrUpdateRoleOutput() { Role = input.Role };
        }

        public virtual async Task<bool> DeleteRole(int roleId)
        {
            if (await _permissionManager.DeleteRoleAsync(roleId) >= 0)
            {
                Logger.Info("Ermes: DeleteRole with Id: " + roleId);
                return true;
            }
            else
                throw new UserFriendlyException("Invalid Role Id");
        }*/

        public virtual async Task<GetRolesOutput> GetRoles()
        {
            var list = await _permissionManager.GetRolesAsync();

            return new GetRolesOutput()
            {
                Roles = _objectMapper.Map<List<RoleDto>>(list)
            };
        }

        public virtual async Task<bool> AssignPermissionToRole(AssignPermissionToRoleInput input)
        {
            var permission = _objectMapper.Map<ErmesPermission>(input.Permission);
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

        public virtual async Task<bool> DeletePermissionForRole(DeletePermissionForRoleInput input)
        {
            var permission = _objectMapper.Map<ErmesPermission>(input.Permission);
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
            if(response.statusCode == 200)            
            {
                foreach(ApplicationRole appRole in response.successResponse.application.roles)
                {
                    if(roleMap.ContainsKey(appRole.name))
                    {
                        _objectMapper.Map(appRole, roleMap[appRole.name]);
                        roleMap[appRole.name] = null;
                    }    
                    else
                        await _permissionManager.CreateOrUpdateRoleAsync(_objectMapper.Map<Role>(appRole));
                }
                List<string> rolesToDelete = roleMap.Where(kv => kv.Value != null).Select(kv => kv.Value.Name).ToList();
                if(rolesToDelete.Count > 0)
                {
                    throw new UserFriendlyException("Some roles have been deleted from the FusionAuth remote. Please restore them: " + rolesToDelete.Aggregate((s1,s2)=>s1+", "+s2));
                }
            }
            else
            {
                throw new UserFriendlyException(L("FusionAuthUnknonwError"));
            }
            return true;
        }

        public virtual async Task<bool> AssignRolesToPerson(AssignRolesToPersonInput input)
        {            
            Person person = await _personManager.GetPersonByIdAsync(input.PersonId);
            if (person == null)
               throw new UserFriendlyException(L("InvalidPersonId"));

            List<Role> rolesToAssign = await GetRolesAndCheckOrganization(input.Roles, person.OrganizationId, _personManager, _organizationManager, _session);

            FusionAuthClient client = FusionAuth.GetFusionAuthClient(_fusionAuthSettings.Value);

            RegistrationRequest regReq =  new RegistrationRequest(){
                registration = new UserRegistration(){
                    applicationId = new Guid(_fusionAuthSettings.Value.ApplicationId),
                    roles = rolesToAssign.Select(r => r.Name).ToList()
                }
            };

            ClientResponse<RegistrationResponse> response = await client.UpdateRegistrationAsync(person.FusionAuthUserGuid, regReq);
            if (response.statusCode == 200)
            {
                await _personManager.DeletePersonRolesAsync(person.Id);
                foreach(Role rta in rolesToAssign)
                {
                    PersonRole pr = new PersonRole(){
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

        
    }
}
