using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Configuration
{
    /// <summary>
    /// Defines string constants for setting names in the application.
    /// See <see cref="AppSettingProvider"/> for setting definitions.
    /// </summary>
    public static class AppSettings
    {
        public static class General
        {
            public const string Environment = "App.General.Environment";
            public const string HouseOrganization = "App.General.HouseOrganization";
        }

        public static class JobSettings
        {
            public const string Station_JobEnabled = "App.JobSettings.Stations.JobEnabled";
            public const string Stations_DaysToBeKept = "App.JobSettings.Stations.DaysToBeKept";
        }
    }
}
