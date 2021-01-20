using Abp.Application.Services;

namespace Ermes
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class ErmesAppServiceBase : ApplicationService
    {
        protected ErmesAppServiceBase()
        {
            LocalizationSourceName = ErmesConsts.LocalizationSourceName;
        }
    }
}