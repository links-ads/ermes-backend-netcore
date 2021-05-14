using Abp;
using Abp.Authorization;
using System;
using System.Threading.Tasks;
using PermissionManager = Ermes.Permissions.PermissionManager;

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

        public Task<bool> IsGrantedAsync(UserIdentifier user, string permissionName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsGrantedAsync(string permissionName)
        {
            throw new NotImplementedException();
        }
    }
}
