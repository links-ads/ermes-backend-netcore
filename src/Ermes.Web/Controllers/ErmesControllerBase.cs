using Abp.AspNetCore.Mvc.Controllers;

namespace Ermes.Web.Controllers
{
    public abstract class ErmesControllerBase: AbpController
    {
        protected ErmesControllerBase()
        {
            LocalizationSourceName = ErmesConsts.LocalizationSourceName;
        }
    }
}