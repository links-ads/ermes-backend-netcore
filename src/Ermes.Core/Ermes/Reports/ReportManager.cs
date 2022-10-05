using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Reports
{
    public class ReportManager : DomainService
    {
        public IQueryable<Report> Reports { get { return ReportRepository.GetAll().Include(a => a.Creator).Include(a => a.Creator.Organization); } }
        protected IRepository<Report> ReportRepository { get; set; }

        public ReportManager(
                IRepository<Report> reportRepository
            )
        {
            ReportRepository = reportRepository;
        }

        public async Task<Report> GetReportByIdAsync(int reportId)
        {
            return await Reports.SingleOrDefaultAsync(r => r.Id == reportId);
        }

        public Report GetReportById(int reportId)
        {
            return Reports.SingleOrDefault(r => r.Id == reportId);
        }

        public async Task<int> InsertReportAsync(Report report)
        {
            return await ReportRepository.InsertAndGetIdAsync(report);
        }

        public IQueryable<Report> GetReports(DateTime startDate, DateTime endDate)
        {
            return Reports
                       .Where(r => r.Timestamp >= startDate && r.Timestamp <= endDate);
                       
        }

        public async Task<List<Report>> GetReportsByPersonAsync(long personId)
        {
            return await Reports.Where(r => r.CreatorUserId.Value == personId).ToListAsync();
        }
        public async Task DeleteReportsByPersonIdAsync(long personId)
        {
            await ReportRepository.DeleteAsync(r => r.CreatorUserId.HasValue & r.CreatorUserId.Value == personId);
        }

    }
}
