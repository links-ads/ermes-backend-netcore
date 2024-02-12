using Ermes.Actions.Dto;
using System.Collections.Generic;

namespace Ermes.Web.Controllers.Dto
{
    public class CreatePersonActionBatchOutput
    {
        public List<CreatePersonActionOutput> PersonActions { get; set; }
    }
}
