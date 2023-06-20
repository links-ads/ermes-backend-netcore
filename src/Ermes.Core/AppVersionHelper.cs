﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Ermes
{
    /// <summary>
    /// Central point for application version.
    /// </summary>
    public class AppVersionHelper
    {
        /// <summary>
        /// Gets current version of the application.
        /// All project's assembly versions are changed when this value is changed.
        /// It's also shown in the web page.
        /// </summary>
        public const string Version = "4.10.0";

        /// <summary>
        /// Gets release (last build) date of the application.
        /// It's shown in the web page.
        /// </summary>
        public static DateTime ReleaseDate
        {
            get { return new FileInfo(typeof(AppVersionHelper).Assembly.Location).LastWriteTime; }
        }
    }
}