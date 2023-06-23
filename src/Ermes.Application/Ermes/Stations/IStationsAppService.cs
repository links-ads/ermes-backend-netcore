using Ermes.Dto.Datatable;
using Ermes.Interfaces;
using Ermes.Stations.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Ermes.Stations
{
    public interface IStationsAppService : IBackofficeApi
    {
        Task<DTResult<StationDto>> GetStations(GetStationsInput input);

        Task<GetMeasuresByStationAndSensorOutput> GetMeasuresByStationAndSensor(GetMeasuresByStationAndSensorInput input);
    }
}
