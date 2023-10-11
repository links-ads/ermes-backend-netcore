using Abp.Authorization;
using Abp.Dependency;
using Abp.Extensions;
using Abp.Runtime.Session;
using Hangfire.Dashboard;

namespace Ermes.Web.App_Start
{
    public class AbpHangfireAuthorizationFilter: IDashboardAuthorizationFilter
    {
        public IIocResolver IocResolver { get; set; }

        public AbpHangfireAuthorizationFilter(string requiredPermissionName = null)
        {
            IocResolver = IocManager.Instance;
        }

        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }
}
