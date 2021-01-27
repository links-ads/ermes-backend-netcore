using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.ReportRequests
{
    public class ReportRequestManager : DomainService
    {
        public IQueryable<ReportRequest> ReportRequests { get { return ReportRequestRepository.GetAll().Include(a => a.Creator.Organization); } }
        protected IRepository<ReportRequest> ReportRequestRepository { get; set; }

        public ReportRequestManager(
                IRepository<ReportRequest> reportRequestRepository
            )
        {
            ReportRequestRepository = reportRequestRepository;
        }

        public async Task<ReportRequest> GetReportRequestByIdAsync(int reportRequestId)
        {
            return await ReportRequests.SingleOrDefaultAsync(a => a.Id == reportRequestId);
        }

        public async Task<int> CreateOrUpdateReportRequestAsync(ReportRequest rr)
        {
            return await ReportRequestRepository.InsertOrUpdateAndGetIdAsync(rr);
        }

        public async Task DeleteReportRequestAsync(ReportRequest rr)
        {
            await ReportRequestRepository.DeleteAsync(rr);
        }
    }
}
