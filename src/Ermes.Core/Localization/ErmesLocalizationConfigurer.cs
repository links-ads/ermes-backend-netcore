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
            localizationConfiguration.Languages.Add(new LanguageInfo("es", "Español", "famfamfam-flags es"));
            localizationConfiguration.Languages.Add(new LanguageInfo("fr", "Français", "famfamfam-flags fr"));
            localizationConfiguration.Languages.Add(new LanguageInfo("el", "Ελληνικά", "famfamfam-flags gr"));

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