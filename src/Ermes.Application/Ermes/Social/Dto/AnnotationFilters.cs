using Ermes.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ermes.Social.Dto
{
    public class AnnotationFilters : SocialBaseFilters, ISocialPaginationInput
    {
        public int? Page { get; set; }
        public int? Limit { get; set; }
        public bool? Informative { get; set; }
    }
}
