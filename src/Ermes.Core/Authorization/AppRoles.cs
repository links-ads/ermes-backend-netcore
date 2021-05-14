using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Authorization
{
    public static class AppRoles
    {
        public const string ADMINISTRATOR = "administrator";
        public const string DECISION_MAKER = "decision_maker";
        public const string ORGANIZATION_MANAGER = "organization_manager";
        public const string TEAM_LEADER = "team_leader";
        public const string CITIZEN = "Citizen";

        public static readonly string[] ADMINISTRATOR_PERMISSION_LIST = new string[]
        {
            AppPermissions.Backoffice,
            AppPermissions.Roles.Roles_PermissionInitialize,
            AppPermissions.Imports.Import,
            AppPermissions.Imports.Import_Activities,
            AppPermissions.Imports.Import_Categories,
            AppPermissions.Organizations.Organization,
            AppPermissions.Organizations.Organization_CanViewAll,
            AppPermissions.Organizations.Organization_CanCreate,
            AppPermissions.Organizations.Organization_CanUpdateAll,
            AppPermissions.Organizations.Organization_CanAssignPersonCrossOrganization,
            AppPermissions.Teams.Team_CanCreateTeamCrossOrganization
        };

        public static readonly string[] ORGANIZATION_MANAGER_PERMISSION_LIST = new string[]
        {
            AppPermissions.Backoffice,
            AppPermissions.Organizations.Organization,
            AppPermissions.Organizations.Organization_CanUpdate
        };

        public static readonly string[] DECISION_MAKER_PERMISSION_LIST = new string[]
        {
            AppPermissions.Backoffice,
            AppPermissions.Organizations.Organization,
            AppPermissions.Organizations.Organization_CanUpdate
        };
    }
}
