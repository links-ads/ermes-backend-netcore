using Ermes.Dto;
using Ermes.Dto.Datatable;
using Ermes.Reports.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Reports
{
    public interface IReportsAppService
    {
        Task<GetCategoriesOutput> GetCategories();
        Task<DTResult<ReportDto>> GetReports(GetReportsInput input);
        Task<GetEntityByIdOutput<ReportDto>> GetReportById(GetEntityByIdInput<int> input);
        Task<bool> UpdateReportStatus(UpdateReportStatusInput input);

        Task<DTResult<ReportRequestDto>> GetReportRequests(GetReportRequestsInput input);
        Task<int> CreateOrUpdateReportRequest(CreateOrUpdateReportRequestInput input);
        Task<GetEntityByIdOutput<ReportRequestDto>> GetReportRequestById(GetEntityByIdInput<int> input);
        Task<bool> DeleteReportRequest(IdInput<int> input);

    }
}
