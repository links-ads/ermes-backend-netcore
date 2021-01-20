using Abp.AspNetCore.Mvc.Views;

namespace Ermes.Web.Views
{
    public abstract class ErmesRazorPage<TModel> : AbpRazorPage<TModel>
    {
        protected ErmesRazorPage()
        {
            LocalizationSourceName = ErmesConsts.LocalizationSourceName;
        }
    }
}
