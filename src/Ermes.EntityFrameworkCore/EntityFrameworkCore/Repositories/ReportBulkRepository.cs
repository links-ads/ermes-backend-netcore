using Abp.EntityFrameworkCore;
using Ermes.ReportRequests;
using Ermes.Reports;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Ermes.EntityFrameworkCore.Repositories
{
    public class ReportBulkRepository : ErmesRepositoryBase<Report, int>, IReportBulkRepository
    {
        public ReportBulkRepository(IDbContextProvider<ErmesDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public IQueryable<Report> GetReportsFilteredByReportRequest(ReportRequest rr, int srid)
        {
            return Context
                    .Reports
                    .FromSqlInterpolated($"SELECT * FROM public.reports r where ST_CONTAINS(ST_GEOMFROMTEXT({rr.AreaOfInterest.ToText()}, {srid}), \"Location\"::geometry) and array(select jsonb_array_elements(r.\"ExtensionData\" ) ->> 'CategoryId')::int[] && {rr.SelectedCategories}");
        }
    }
}
