using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Threading;
using Abp.UI;
using Ermes.ExternalServices.Csi;
using Ermes.Reports;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Jobs
{
    [Serializable]
    public class SendReportJobArgs
    {
        public int ReportId { get; set; }
    }

    public class SendReportJob : BackgroundJob<SendReportJobArgs>, ITransientDependency
    {
        private readonly ReportManager _reportManager;
        private readonly CsiManager _csiManager;
        public SendReportJob(ReportManager reportManager, CsiManager csiManager)
        {
            _reportManager = reportManager;
            _csiManager = csiManager;
        }

        [UnitOfWork]
        public override void Execute(SendReportJobArgs args)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                var report = _reportManager.GetReportById(args.ReportId);
                if (report == null || !report.CreatorUserId.HasValue)
                    throw new UserFriendlyException(L("InvalidReportId", args.ReportId));

                AsyncHelper.RunSync(() => _csiManager.InserisciFromFaster(report));
            }
        }
    }
}
