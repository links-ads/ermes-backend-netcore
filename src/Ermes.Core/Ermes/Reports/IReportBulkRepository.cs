using Abp.Domain.Repositories;
using Ermes.ReportRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ermes.Reports
{
    public interface IReportBulkRepository : IRepository<Report, int>
    {
        IQueryable<Report> GetReportsFilteredByReportRequest(ReportRequest rr, int srid);
    }
}
