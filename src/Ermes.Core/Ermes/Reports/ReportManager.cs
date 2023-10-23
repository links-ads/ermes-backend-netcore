using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Reports
{
    public class ReportManager : DomainService
    {
        public IQueryable<Report> Reports { get { return ReportRepository.GetAll().Include(a => a.Creator).Include(a => a.Creator.Organization).Include(a => a.Validations); } }
        public IQueryable<ReportValidation> ReportValidations { get { return ReportValidationRepository.GetAll(); } }
        protected IRepository<Report> ReportRepository { get; set; }
        protected IRepository<ReportValidation> ReportValidationRepository { get; set; }

        public ReportManager(
                IRepository<Report> reportRepository,
                IRepository<ReportValidation> reportValidationRepository
            )
        {
            ReportRepository = reportRepository;
            ReportValidationRepository = reportValidationRepository;
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
            NpgsqlRange<DateTime> range = new NpgsqlRange<DateTime>(startDate, endDate);
            return Reports
                       .Where(r => range.Contains(r.Timestamp));
        }

        public async Task<List<Report>> GetReportsByPersonAsync(long personId)
        {
            return await Reports.Where(r => r.CreatorUserId.Value == personId).ToListAsync();
        }
        public async Task DeleteReportsByPersonIdAsync(long personId)
        {
            await ReportRepository.DeleteAsync(r => r.CreatorUserId.HasValue & r.CreatorUserId.Value == personId);
        }

        public async Task<List<ReportValidation>> GetReportValidationByPersonIdAsync(long personId)
        {
            return await ReportValidations.Where(a => a.PersonId == personId).ToListAsync();
        }

        public async Task<bool> HasAlreadyValidatedReportAsync(long personId, int reportId)
        {
            return (await ReportValidations.CountAsync(a => a.ReportId == reportId && a.PersonId == personId)) > 0;
        }

        public async Task<int> InsertReportValidationAsync(ReportValidation reportValidation)
        {
            return await ReportValidationRepository.InsertAndGetIdAsync(reportValidation);
        }

    }
}
