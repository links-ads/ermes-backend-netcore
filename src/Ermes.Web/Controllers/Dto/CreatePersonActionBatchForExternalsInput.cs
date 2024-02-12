using System.Collections.Generic;

namespace Ermes.Web.Controllers.Dto
{
    public class CreatePersonActionBatchForExternalsInput
    {
        public List<CreatePersonActionForExternalsInput> PersonActions { get; set; }
    }
}
