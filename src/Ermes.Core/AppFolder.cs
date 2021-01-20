using Abp.Dependency;
using Ermes.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes
{
    public class AppFolders : IAppFolders, ISingletonDependency
    {
        public string TempFileDownloadFolder { get; set; }
        public string WebLogsFolder { get; set; }
    }
}
