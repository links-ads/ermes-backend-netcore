using System;
using System.Collections.Generic;
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
        /// Thumbnail image export quality
        /// </summary>
        public const int ThumbnailQuality = 75;

        /// <summary>
        /// Thumbnail image size
        /// </summary>
        public const int ThumbnailSize = 256;

        /// <summary>
        /// Default timezone if one is not specified
        /// </summary>
        public const string DefaultLanguage = "en";

    }
}
