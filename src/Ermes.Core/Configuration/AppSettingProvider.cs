using Abp.Configuration;
using Abp.Localization;
using Ermes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Ermes.Configuration
{
    public class AppSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                //example
                new SettingDefinition(AppSettings.General.Environment, ConfigurationManager.AppSettings[AppSettings.General.Environment] ?? ErmesConsts.Environments.Dev,null,null,null,SettingScopes.Application,true,true,null),
                new SettingDefinition(LocalizationSettingNames.DefaultLanguage, "en"),
                new SettingDefinition(AppSettings.General.HouseOrganization, "Protezione Civile Piemonte"),
                new SettingDefinition(AppSettings.JobSettings.Station_JobEnabled, "true"),
                new SettingDefinition(AppSettings.JobSettings.NotificationReceived_JobEnabled, "true"),
                new SettingDefinition(AppSettings.JobSettings.Stations_DaysToBeKept, "7"),
                new SettingDefinition(AppSettings.JobSettings.NotificationReceived_DaysToBeKept, "15")
            };
        }
    }
}
