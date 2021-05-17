using Abp.Authorization;
using Abp.Collections.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ermes.Authorization
{
    public static class ErmesPermissionCheckerExtensions
    {
        public static void Authorize(this ErmesPermissionChecker permissionChecker, bool requireAll, string[] roles, params string[] permissionNames)
        {
            if (IsGranted(permissionChecker, requireAll, roles, permissionNames))
            {
                return;
            }

            if (requireAll)
            {
                throw new AbpAuthorizationException(
                    string.Format(
                            "Required permissions are not granted. All of these permissions must be granted: {0}",
                        string.Join(", ", permissionNames)
                    )
                );
            }
            else
            {
                throw new AbpAuthorizationException(
                    string.Format(
                            "Required permissions are not granted. At least one of these permissions must be granted: {0}",
                        string.Join(", ", permissionNames)
                    )
                );
            }
        }

        /// <summary>
        /// Checks if current user is granted for given permission.
        /// </summary>
        /// <param name="permissionChecker">Permission checker</param>
        /// <param name="requiresAll">True, to require all given permissions are granted. False, to require one or more.</param>
        /// <param name="permissionNames">Name of the permissions</param>
        public static bool IsGranted(this ErmesPermissionChecker permissionChecker, bool requiresAll, string []roles, params string[] permissionNames)
        {
            if (permissionNames.IsNullOrEmpty())
                return true;

            if (roles.IsNullOrEmpty())
                return false;

            if (requiresAll)
            {
                foreach (var permissionName in permissionNames)
                {
                    if (!(permissionChecker.IsGranted(roles, permissionName)))
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                foreach (var permissionName in permissionNames)
                {
                    if (permissionChecker.IsGranted(roles, permissionName))
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
