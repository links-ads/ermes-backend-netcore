using Ermes.Dashboard.Dto;
using Ermes.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Dashboard
{
    public interface IDashboardAppService : IBackofficeApi
    {
        Task<GetStatisticsOutput> GetStatistics(GetStatisticsInput input);
    }
}
