using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Ermes
{
    public class AppConsts
    {
        /// <summary>
        /// Default page size for paged requests.
        /// </summary>
        public const int DefaultPageSize = 10;

        /// <summary>
        /// Maximum allowed page size for paged requests.
        /// </summary>
        public const int MaxPageSize = 1000;

        public const int Srid = 4326;

        /// <summary>
        /// Default timezone if one is not specified
        /// </summary>
        public const string DefaultTimezone = "Europe/Rome";

        /// <summary>
        /// Default timezone if one is not specified
        /// </summary>
        public const string DefaultLanguage = "en";

        public const string Ermes_Faster = "FASTER";
        public const string Ermes_Shelter = "SHELTER";
        public const string Ermes_Safers = "SAFERS";
        public const string Ermes_House_Partner = "links";

        ///<summary>
        /// CSI service consts
        /// </summary>

        public const string CSI_OFFLINE = "Offline";
        public const string CSI_ACTIVITY = "Doing activity";

        public const string GEOMETRY_POINT = "Point";
        


    }
}
