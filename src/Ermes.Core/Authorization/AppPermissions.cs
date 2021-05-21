using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Authorization
{
    public static class AppPermissions
    {
        public const string App = "App";
        public const string Backoffice = "Backoffice";
        public const string CompetenceArea = "CompetenceArea";

        public static class Imports
        {
            public const string Import = "Import";
            public const string Import_Activities = "Import.Activities";
            public const string Import_CompetenceArea = "Import.CompetenceArea";
            public const string Import_Categories = "Import.Categories";
            public const string Import_Users = "Import.Users";
        }

        public static class Organizations
        {
            public const string Organization = "Organizations";
            public const string Organization_CanViewAll = "Organizations.CanViewAll";
            public const string Organization_CanCreate = "Organizations.CanCreate";
            public const string Organization_CanUpdate = "Organizations.CanUpdate";
            public const string Organization_CanUpdateAll = "Organizations.CanUpdateAll";
            public const string Organization_CanAssignPersonCrossOrganization = "Organizations.CanAssignPersonCrossOrganization";
        }

        public const string Permissions = "Permissions";

        public static class Missions
        {
            public const string Mission = "Missions";
            public const string Mission_Management = "Missions.Management";
        }

        public static class Profiles
        {
            public const string Profile = "Profile";
            public const string Profile_Create = "Profile.Create";
            public const string Profile_Update = "Profile.Update";
        }

        public static class Actions
        {
            public const string Action = "Actions";
            public const string Action_ShowAll = "Actions.ShowAll";
            public const string Action_ShowOrganization = "Actions.ShowOrganization";
        }

        public static class Teams
        {
            public const string Team_CanCreateTeamCrossOrganization = "Team.CanCreateTeamCrossOrganization";
            public const string Team_CanViewAll = "Team.CanViewAll";
        }

        public static class Users
        {
            public const string Users_CanCreateCitizenOrPersonCrossOrganization = "Users.CanCreateCitizenOrPersonCrossOrganization";
            public const string Users_CanEditColleagues = "Users.CanEditColleagues";
        }
    }
}
