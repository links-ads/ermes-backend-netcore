using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Stations.Dto
{
    public class GetMeasuresByStationAndSensorOutput
    {
        public List<MeasureDto> Measurements { get; set; }
    }
}
