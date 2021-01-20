using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Interfaces
{
    public interface IAppFolders
    {
        string TempFileDownloadFolder { get; }
        string WebLogsFolder { get; set; }
    }
}
