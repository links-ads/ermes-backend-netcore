using Abp.ErmesSocialNetCore.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Ermes.Social.Dto
{
    public class GetLabelsOutput
    {
        public List<Label> Labels { get; set; }
    }
}
