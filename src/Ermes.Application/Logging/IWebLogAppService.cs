using Abp.Application.Services;
using Ermes.Interfaces;
using Ermes.Logging.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Logging
{
    public interface IWebLogAppService : IBackofficeApi
    {
        GetLatestWebLogsOutput GetLatestWebLogs(GetLatestWebLogsInput input);
    }
}
