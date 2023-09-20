using System;

namespace Ermes.Stations.Dto
{
    public class ValidateMeasureInput
    {
        public string MeasureId { get; set; }
        public bool Fire { get; set; }
        public bool Smoke { get; set; }
        public object Metadata { get; set; }
    }
}
