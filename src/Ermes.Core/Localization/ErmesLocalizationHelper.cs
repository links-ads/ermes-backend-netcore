using Abp.Dependency;
using Abp.Localization;
using Ermes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Localization
{
    public class ErmesLocalizationHelper : ISingletonDependency
    {
        ILocalizationManager _localizationManager;
        const string localizationPackage = ErmesConsts.LocalizationSourceName;
        public ErmesLocalizationHelper(ILocalizationManager localizationManager)
        {
            _localizationManager = localizationManager;
        }

        public string L(string code, params object[] parameters)
        {
            return string.Format(_localizationManager.GetString(localizationPackage, code), parameters);
        }

        public string L(string code)
        {
            return _localizationManager.GetString(localizationPackage, code);
        }
    }
}
