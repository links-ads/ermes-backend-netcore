using Abp.Azure;
using Abp.BackgroundJobs;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.SensorService;
using Ermes.Configuration;
using Ermes.Notifications;
using Ermes.Resources;
using Ermes.Stations.Dto;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Ermes.Jobs
{
    [Serializable]
    public class PurgeStationImagesJobArgs
    {
    }

    public class PurgeStationImagesJob : AsyncBackgroundJob<PurgeStationImagesJobArgs>, ITransientDependency
    {
        private readonly IAzureManager _azureManager;
        private readonly SensorServiceManager _sensorServiceManager;
        private readonly NotificationManager _notificationManager;

        public PurgeStationImagesJob(IAzureManager azureManager, SensorServiceManager sensorServiceManager, NotificationManager notificationManager)
        {
            _azureManager = azureManager;
            _sensorServiceManager = sensorServiceManager;
            _notificationManager = notificationManager;
        }

        [UnitOfWork(IsDisabled = true)]
        protected override async Task ExecuteAsync(PurgeStationImagesJobArgs args)
        {
            var _azureStorageManager = _azureManager.GetStorageManager(ResourceManager.GetBasePath(ResourceManager.Cameras.ContainerName));
            var _azureThumbnailStorageManager = _azureManager.GetStorageManager(ResourceManager.GetBasePath(ResourceManager.CameraThumbnails.ContainerName));

            if (SettingManager.GetSettingValue<bool>(AppSettings.JobSettings.Station_JobEnabled))
            {
                var dayToKeeep = SettingManager.GetSettingValue<int>(AppSettings.JobSettings.Stations_DaysToBeKept);
                var refDate = DateTime.UtcNow.AddDays(-dayToKeeep);
                //Station Management
                var fullStationList = await _sensorServiceManager.GetStations();
                foreach (var station in fullStationList)
                {
                    var summary = await _sensorServiceManager.GetStationSummary(station.Id, DateTime.MinValue, refDate);
                    foreach (var sensor in summary.Sensors)
                    {
                        foreach (var measurement in sensor.Measurements)
                        {
                            var metadata = JsonConvert.DeserializeObject<MetadataDto>(JsonConvert.SerializeObject(measurement.Metadata));
                            if (!metadata.Detection.fire && !metadata.Detection.smoke)
                            {
                                Logger.InfoFormat("Removing Station {0} - Sensor {1} - Measure {2}...", station.Name, sensor.Description, measurement.Id);
                                string imageName = measurement.Measure.Split('/').LastOrDefault();
                                if (imageName != null)
                                {
                                    string filename = ResourceManager.Cameras.GetRelativeMediaPath(station.Name, sensor.Description, imageName);
                                    await _azureStorageManager.DeleteBlobAsync(filename);
                                    filename = ResourceManager.CameraThumbnails.GetRelativeMediaPath(station.Name, sensor.Description, imageName);
                                    await _azureThumbnailStorageManager.DeleteBlobAsync(filename);

                                    await _sensorServiceManager.DeleteMeasure(measurement.Id);
                                }
                            }
                        }
                    }
                }

                //Notification received management
                await _notificationManager.DeleteNotificationReceivedOlderThanDateAsync(refDate);
            }

            return;
        }
    }
}
