using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Logging.Dto
{
    public class GetLatestWebLogsInput
    {
        public int NumberOfRows { get; set; } = 1000;
    }
}
