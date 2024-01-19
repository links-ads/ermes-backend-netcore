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
            AppPermissions.Imports.Import_Gamification,
            AppPermissions.Imports.Import_Users,
            AppPermissions.Imports.Import_Layers,
            AppPermissions.Organizations.Organization,
            AppPermissions.Organizations.Organization_CanViewAll,
            AppPermissions.Organizations.Organization_CanCreate_Parent,
            AppPermissions.Organizations.Organization_CanCreate_Child,
            AppPermissions.Organizations.Organization_CanUpdateAll,
            AppPermissions.Organizations.Organization_CanAssignPersonCrossOrganization,
            AppPermissions.Organizations.Organization_CanDeleteCrossOrganization,
            AppPermissions.Teams.Team_CanViewAll,
            AppPermissions.Teams.Team_CanCreate,
            AppPermissions.Teams.Team_CanCreateTeamCrossOrganization,
            AppPermissions.Teams.Team_CanUpdate,
            AppPermissions.Teams.Team_CanDelete,
            AppPermissions.Users.Users_CanCreate,
            AppPermissions.Users.Users_CanCreateCitizenOrPersonCrossOrganization,
            AppPermissions.Users.Users_CanSeeUncompletedUsers,
            AppPermissions.Missions.Mission_CanCreate,
            AppPermissions.Missions.Mission_CanSeeCrossOrganization,
            AppPermissions.Reports.Report_CanSeeCrossOrganization,
            AppPermissions.Actions.Action_CanSeeCrossOrganization,
            AppPermissions.MapRequests.MapRequest_CanCreate,
            AppPermissions.MapRequests.MapRequest_CanDelete,
            AppPermissions.MapRequests.MapRequest_CanSeeCrossOrganization,
            AppPermissions.Communications.Communication_CanCreate,
            AppPermissions.Communications.Communication_CanSeeCrossOrganization,
            AppPermissions.Profiles.Profile_CanDelete
        };

        public static readonly string[] ORGANIZATION_MANAGER_PERMISSION_LIST = new string[]
        {
            AppPermissions.Backoffice,
            AppPermissions.Organizations.Organization,
            AppPermissions.Organizations.Organization_CanCreate_Child,
            AppPermissions.Organizations.Organization_CanUpdate,
            AppPermissions.Users.Users_CanCreate,
            AppPermissions.Users.Users_CanEditColleagues,
            AppPermissions.Communications.Communication_CanCreate,
            AppPermissions.MapRequests.MapRequest_CanCreate,
            AppPermissions.Missions.Mission_CanCreate,
            AppPermissions.Teams.Team_CanCreate,
            AppPermissions.Teams.Team_CanUpdate,
            AppPermissions.Teams.Team_CanDelete
        };

        public static readonly string[] DECISION_MAKER_PERMISSION_LIST = ORGANIZATION_MANAGER_PERMISSION_LIST;

        public static readonly string[] FIRST_RESPONDER_PERMISSION_LIST = new string[]
        {
            AppPermissions.Backoffice
        };

        public static readonly string[] TEAM_LEADER_PERMISSION_LIST = FIRST_RESPONDER_PERMISSION_LIST;

        public static readonly string[] CITIZEN_PERMISSION_LIST = new string[] { };
    }
}
