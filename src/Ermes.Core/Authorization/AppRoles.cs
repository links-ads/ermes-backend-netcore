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
        public const string CITIZEN = "citizen";
        public const string FIRST_RESPONDER = "first_responder";

        public static readonly string[] ADMINISTRATOR_PERMISSION_LIST = new string[]
        {
            AppPermissions.Backoffice,
            AppPermissions.Imports.Import,
            AppPermissions.Imports.Import_Activities,
            AppPermissions.Imports.Import_Categories,
            AppPermissions.Imports.Import_Users,
            AppPermissions.Organizations.Organization,
            AppPermissions.Organizations.Organization_CanViewAll,
            AppPermissions.Organizations.Organization_CanCreate,
            AppPermissions.Organizations.Organization_CanUpdateAll,
            AppPermissions.Organizations.Organization_CanAssignPersonCrossOrganization,
            AppPermissions.Teams.Team_CanViewAll,
            AppPermissions.Teams.Team_CanCreateTeamCrossOrganization,
            AppPermissions.Users.Users_CanCreateCitizenOrPersonCrossOrganization
        };

        public static readonly string[] ORGANIZATION_MANAGER_PERMISSION_LIST = new string[]
        {
            AppPermissions.Backoffice,
            AppPermissions.Organizations.Organization,
            AppPermissions.Organizations.Organization_CanUpdate,
            AppPermissions.Users.Users_CanEditColleagues
        };

        public static readonly string[] DECISION_MAKER_PERMISSION_LIST = new string[]
        {
            AppPermissions.Backoffice,
            AppPermissions.Organizations.Organization,
            AppPermissions.Organizations.Organization_CanUpdate,
            AppPermissions.Users.Users_CanEditColleagues
        };
    }
}
