using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Ermes.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Stations
{
    public class StationManager: DomainService
    {
        public IQueryable<Station> Stations { get { return StationRepository.GetAll(); } }
        protected IRepository<Station> StationRepository { get; set; }
        public StationManager(IRepository<Station> stationRepository)
        {
            StationRepository = stationRepository;
        }

        public async Task<Station> InsertStationAsync(Station station)
        {
            return await StationRepository.InsertAsync(station);
        }

        public Station InsertStation(Station station)
        {
            return StationRepository.Insert(station);
        }

        public Station GetStationBySensorServiceId(string id)
        {
            return Stations.FirstOrDefault(s => s.SensorServiceId == id);
        }

    }
}
