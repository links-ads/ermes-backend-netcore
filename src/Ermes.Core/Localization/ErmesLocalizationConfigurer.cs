using System.Reflection;
using Abp.Configuration.Startup;
using Abp.Localization;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Json;
using Abp.Reflection.Extensions;

namespace Ermes.Localization
{
    public static class ErmesLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Languages.Add(new LanguageInfo("en", "English", "famfamfam-flags england", isDefault: true));
            localizationConfiguration.Languages.Add(new LanguageInfo("tr", "Türkçe", "famfamfam-flags tr"));
            localizationConfiguration.Languages.Add(new LanguageInfo("it", "Italiano", "famfamfam-flags it"));
            localizationConfiguration.Languages.Add(new LanguageInfo("fi", "Suomalainen", "famfamfam-flags fi"));

            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(ErmesConsts.LocalizationSourceName,
                    new JsonEmbeddedFileLocalizationDictionaryProvider(
                        typeof(ErmesLocalizationConfigurer).GetAssembly(),
                        "Ermes.Localization.SourceFiles"
                    )
                )
            );
        }
    }
}