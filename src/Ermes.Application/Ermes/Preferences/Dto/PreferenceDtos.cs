using Abp.AutoMapper;
using Ermes.Enums;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Ermes.Preferences
{
    public class GetPreferenceInput
    {
        public SourceDeviceType Source { get; set; }
    }
   
    public class GetPreferenceOutput
    {
        public PreferenceDto Preference { get; set; }
    }

    public class PreferenceDto
    {
        public string Details { get; set; }
        public SourceDeviceType Source { get; set; }
    }

    public class CreateOrUpdatePreferenceInput
    {
        [Required]
        public PreferenceDto Preference { get; set; }
    }
}