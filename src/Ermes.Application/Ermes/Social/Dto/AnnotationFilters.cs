using Ermes.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ermes.Social.Dto
{
    public class AnnotationFilters
    {
        public int? Limit { get; set; }
        public int? Page { get; set; }
        public SocialModuleLanguageType Language { get; set; }
        public bool? Informative { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public List<string> Labels { get; set; }

        [MaxLength(2), MinLength(2)]
        public List<float> SouthWest { get; set; }
        [MaxLength(2), MinLength(2)]
        public List<float> NorthEast { get; set; }

    }
}
