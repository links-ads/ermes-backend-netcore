using Ermes.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.Social.Dto
{
    public class LabelFilters
    {
        public bool? Operational { get; set; }
        public SocialModuleTaskType Task { get; set; }
    }
}
