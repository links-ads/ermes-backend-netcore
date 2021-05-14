using Ermes.Interfaces;
using Ermes.Roles.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Roles
{
    public interface IRolesAppService : IBackofficeApi
    {
        //Task<CreateOrUpdateRoleOutput> CreateOrUpdateRole(CreateOrUpdateRoleInput input);
        //Task<bool> DeleteRole(int roleId);
        Task<GetRolesOutput> GetRoles();
        Task<bool> AssignRolesToPerson(AssignRolesToPersonInput input);
        Task<bool> AssignPermissionToRole(AssignPermissionToRoleInput input);
        Task<bool> DeletePermissionForRole(DeletePermissionForRoleInput input);
        Task<bool> SyncRolesFromFusionAuth();
        Task<bool> InitilizePermissions();

    }
}
