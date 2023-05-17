using Abp.EntityFrameworkCore;
using Ermes.Reports;

namespace Ermes.EntityFrameworkCore.Repositories
{
    public class ReportBulkRepository : ErmesRepositoryBase<Report, int>, IReportBulkRepository
    {
        public ReportBulkRepository(IDbContextProvider<ErmesDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}
