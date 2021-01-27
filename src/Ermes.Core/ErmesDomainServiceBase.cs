using Abp.Application.Services;
using Abp.Domain.Services;
using Abp.UI;
using Ermes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ermes
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class ErmesDomainServiceBase : DomainService
    {
        protected ErmesDomainServiceBase()
        {
            LocalizationSourceName = ErmesConsts.LocalizationSourceName;
        }

        
    }
}