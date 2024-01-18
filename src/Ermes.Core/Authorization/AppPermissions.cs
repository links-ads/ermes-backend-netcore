namespace Ermes.Authorization
{
    public static class AppPermissions
    {
        public const string App = "App";
        public const string Permissions = "Permissions";
        public const string Backoffice = "Backoffice";
        public const string CompetenceArea = "CompetenceArea";

        public static class Imports
        {
            public const string Import = "Import";
            public const string Import_Activities = "Import.Activities";
            public const string Import_CompetenceArea = "Import.CompetenceArea";
            public const string Import_Categories = "Import.Categories";
            public const string Import_Gamification = "Import.Gamification";
            public const string Import_Users = "Import.Users";
            public const string Import_Layers = "Import.Layers";
        }

        public static class Organizations
        {
            public const string Organization = "Organizations";
            public const string Organization_CanViewAll = "Organizations.CanViewAll";
            public const string Organization_CanCreate_Father = "Organizations.CanCreate.Father";
            public const string Organization_CanCreate_Child = "Organizations.CanCreate.Child";
            public const string Organization_CanUpdate = "Organizations.CanUpdate";
            public const string Organization_CanUpdateAll = "Organizations.CanUpdateAll";
            public const string Organization_CanAssignPersonCrossOrganization = "Organizations.CanAssignPersonCrossOrganization";
            public const string Organization_CanDeleteCrossOrganization = "Organizations.CanDeleteCrossOrganization";
        }

        public static class Missions
        {
            public const string Mission = "Missions";
            public const string Mission_CanSeeCrossOrganization = "Missions.CanSeeCrossOrganization";
            public const string Mission_CanCreate = "Missions.CanCreate";
        }

        public static class Communications
        {
            public const string Communication = "Communications";
            public const string Communication_CanSeeCrossOrganization = "Communications.CanSeeCrossOrganization";
            public const string Communication_CanCreate = "Communications.CanCreate";
        }

        public static class MapRequests
        {
            public const string MapRequest = "MapRequests";
            public const string MapRequest_CanSeeCrossOrganization = "MapRequests.CanSeeCrossOrganization";
            public const string MapRequest_CanCreate = "MapRequests.CanCreate";
            public const string MapRequest_CanDelete = "MapRequests.CanDelete";
        }

        public static class Reports
        {
            public const string Report = "Reports";
            public const string Report_CanSeeCrossOrganization = "Reports.CanSeeCrossOrganization";
        }

        public static class Profiles
        {
            public const string Profile = "Profile";
            public const string Profile_CanCreate = "Profile.CanCreate";
            public const string Profile_CanUpdate = "Profile.CanUpdate";
            public const string Profile_CanDelete = "Profile.CanDelete";
        }

        public static class Actions
        {
            public const string Action = "Actions";
            public const string Action_CanSeeCrossOrganization = "Actions.CanSeeCrossOrganization";
        }

        public static class Teams
        {
            public const string Team_CanCreateTeamCrossOrganization = "Teams.CanCreateTeamCrossOrganization";
            public const string Team_CanViewAll = "Teams.CanViewAll";
            public const string Team_CanCreate = "Teams.CanCreate";
            public const string Team_CanUpdate = "Teams.CanUpdate";
        }

        public static class Users
        {
            public const string Users_CanCreateCitizenOrPersonCrossOrganization = "Users.CanCreateCitizenOrPersonCrossOrganization";
            public const string Users_CanEditColleagues = "Users.CanEditColleagues";
            public const string Users_CanSeeUncompletedUsers = "Users.Users_CanSeeUncompletedUsers";
            public const string Users_CanCreate = "Users.CanCreate";
        }
    }
}
