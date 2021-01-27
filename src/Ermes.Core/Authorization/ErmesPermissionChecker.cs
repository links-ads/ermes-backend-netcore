using Abp;
using Abp.Authorization;
using Abp.Dependency;
using Ermes.Permissions;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Linq;
using System.Threading.Tasks;
using PermissionManager = Ermes.Permissions.PermissionManager;
using Microsoft.EntityFrameworkCore;

namespace Ermes.Authorization
{
    public class ErmesPermissionChecker : IPermissionChecker
    {
        private readonly PermissionManager _permissionManager;

        public ErmesPermissionChecker(PermissionManager permissionManager)
        {
            _permissionManager = permissionManager;
        }

        public bool IsGranted(string[] roleList, string permissionName)
        {
            return _permissionManager.IsPermissionGrantedForRoles(roleList, permissionName);
        }

        public bool IsGranted(UserIdentifier user, string permissionName)
        {
            throw new NotImplementedException();
        }

        public bool IsGranted(string permissionName)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsGrantedAsync(string permissionName)
        {
            return await _permissionManager.Permissions.Where(p => p.Name == permissionName && p.RoleId == 2).SingleOrDefaultAsync() != null;
        }

        public Task<bool> IsGrantedAsync(UserIdentifier user, string permissionName)
        {
            throw new NotImplementedException();
        }
    }
}
