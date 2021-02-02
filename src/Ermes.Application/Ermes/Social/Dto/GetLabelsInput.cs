using Ermes.Enums;
using Ermes.Social.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static Abp.ErmesSocialNetCore.Model.Label;

namespace Ermes.Social.Dto
{
    public class GetLabelsInput
    {
        public GetLabelsInput()
        {
            Filters = new LabelFilters();
        }
        public LabelFilters Filters { get; set; }
    }
}
