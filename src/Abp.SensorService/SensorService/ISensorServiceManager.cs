using Abp.SensorService.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Abp.SensorService
{
    public interface ISensorServiceManager
    {
        Task<List<SensorServiceStation>> GetStations();
        Task<SensorServiceStation> GetStationInfo(string stationId);
        Task<SensorServiceStation> CreateStation(string name, decimal latitude, decimal longitude, decimal altitude, string address = "address");
        Task<SensorServiceSensor> CreateSensor(string stationId, string type, string desciption, string unit = "degree");
        Task<SensorServiceMeasure> CreateMeasure(string sensorId, DateTime dateStart, DateTime dateEnd, string measure, object metadata, string unit= "degree");
        Task GetMeasuresOfSensor(string stationId, string sensorId);
    }
}
