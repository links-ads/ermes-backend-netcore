using Ermes.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ermes.Social.Dto
{
    public class SocialBaseFilters
    {
        public List<SocialModuleLanguageType> Languages { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public List<int> Infotypes { get; set; }
        public List<int> Hazards { get; set; }

        [MaxLength(2), MinLength(2)]
        public List<float> SouthWest { get; set; }
        [MaxLength(2), MinLength(2)]
        public List<float> NorthEast { get; set; }
    }
}
