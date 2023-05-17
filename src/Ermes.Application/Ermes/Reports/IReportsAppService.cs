using Ermes.Dto;
using Ermes.Dto.Datatable;
using Ermes.Gamification.Dto;
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
        Task<GamificationResponse> ValidateReport(ValidateReportInput input);
    }
}
