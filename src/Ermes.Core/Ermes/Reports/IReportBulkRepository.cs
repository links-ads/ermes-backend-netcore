using Abp.Domain.Repositories;

namespace Ermes.Reports
{
    public interface IReportBulkRepository : IRepository<Report, int>
    {
    }
}
