using Ermes.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Actions.Dto
{
    public class GetActionsOutput
    {
        public List<PersonActionDto> PersonActions { get; set; }
    }
}
