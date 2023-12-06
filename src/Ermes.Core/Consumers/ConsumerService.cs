using Abp.Azure;
using Abp.Domain.Uow;
using Abp.SensorService;
using Abp.SensorService.Model;
using Abp.Threading;
using Abp.UI;
using Ermes.Alerts;
using Ermes.Authorization;
using Ermes.Communications;
using Ermes.Consumers.Kafka;
using Ermes.Consumers.RabbitMq;
using Ermes.Core.Helpers;
using Ermes.Dss;
using Ermes.Enums;
using Ermes.MapRequests;
using Ermes.Missions;
using Ermes.Notifications;
using Ermes.Notifiers;
using Ermes.Persons;
using Ermes.Resources;
using Ermes.Stations;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Transactions;

namespace Ermes.Consumers
{
    public class ConsumerService : ErmesDomainServiceBase, IConsumerService
    {
        private readonly MissionManager _missionManager;
        private readonly PersonManager _personManager;
        private readonly NotifierService _notifierService;
        private readonly MapRequestManager _mapRequestManager;
        private readonly CommunicationManager _communicationManager;
        private readonly NotificationManager _notificationManager;
        private readonly AlertManager _alertManager;
        private readonly StationManager _stationManager;
        private readonly IAzureManager _azureManager;
        private readonly SensorServiceManager _sensorServiceManager;
        private readonly IOptions<DssSettings> _dssSettings;
        private readonly CommunicationNotifier _communicationNotifier;


        public ConsumerService(
            MissionManager missionManager,
            NotifierService notifierService,
            PersonManager personManager,
            MapRequestManager mapRequestManager,
            CommunicationManager communicaitonManager,
            NotificationManager notificationManager,
            AlertManager alertManager,
            StationManager stationManager,
            IAzureManager azureManager,
            SensorServiceManager sensorServiceManager,
            IOptions<DssSettings> dssSettings,
            CommunicationNotifier communicationNotifier)
        {
            _missionManager = missionManager;
            _notifierService = notifierService;
            _personManager = personManager;
            _mapRequestManager = mapRequestManager;
            _communicationManager = communicaitonManager;
            _notificationManager = notificationManager;
            _alertManager = alertManager;
            _stationManager = stationManager;
            _azureManager = azureManager;
            _sensorServiceManager = sensorServiceManager;
            _dssSettings = dssSettings;
            _communicationNotifier = communicationNotifier;
        }

        public void ConsumeBusNotification(string message, string routingKey)
        {
            _notificationManager.InsertNotificationReceived(new NotificationReceived(routingKey, message));
            //Consume the message based on routing key prop
            //Kafka bus does not use routingKey, while for RabbitMq it is a mandatory field
            if (routingKey != "")
                ConsumeRabbitMqNotification(message, routingKey);
            else
                ConsumeKafkaNotification(message);
        }

        #region RabbitMq
        public void ConsumeRabbitMqNotification(string message, string routingKey)
        {
            try
            {
                if (routingKey.Contains("status"))
                {
                    var eventData = JsonConvert.DeserializeObject<RabbitMqResponse>(message);
                    eventData.request_code = routingKey.Split('.')[^1];
                    HandleMapRequestStatusChange(eventData);
                }
                else if (routingKey.Contains("notification.sem"))
                {
                    var eventData = JsonConvert.DeserializeObject<RabbitMqAlert>(message);
                    HandleAlertMessage(eventData);
                }
                else if (routingKey.Contains("event.camera"))
                {
                    var eventData = JsonConvert.DeserializeObject<RabbitMqCameraEvent>(message);
                    HandleCameraMessage(eventData);
                }
                else
                    Console.WriteLine("No management for messages with routing key: " + routingKey);
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("ConsumeBusNotification Exception: {0}", e.Message);
            }

            return;
        }

        [UnitOfWork(IsDisabled = true)]
        private void HandleMapRequestStatusChange(RabbitMqResponse eventData)
        {
            using (var unitOfWork = UnitOfWorkManager.Begin(new UnitOfWorkOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                IsTransactional = true,
                Timeout = TimeSpan.FromMinutes(30),
                FilterOverrides =
                        {
                            new DataFilterConfiguration(AbpDataFilters.MayHaveTenant,false),
                            new DataFilterConfiguration(AbpDataFilters.MustHaveTenant, false)
                        }
            }))
            {
                try
                {
                    var mr = _mapRequestManager.GetMapRequestByCode(eventData.request_code);
                    if (mr == null)
                    {
                        Logger.ErrorFormat("HandleMapRequestStatusChange: MapRequest with Code {0} not found", eventData.request_code);
                        return;
                    }

                    var layer = mr.MapRequestLayers.SingleOrDefault(l => l.LayerDataTypeId == eventData.datatype_id);
                    if (layer == null)
                    {
                        Logger.ErrorFormat("HandleMapRequestStatusChange: Layer with DataTypeId {0} not found", eventData.datatype_id);
                        return;
                    }

                    switch (eventData.status_code)
                    {
                        case "200":
                            mr.Status = MapRequestStatusType.ContentAvailable;
                            layer.ReceivedUpdates++;
                            if (layer.ReceivedUpdates == mr.ExpectedUpdates)
                                layer.Status = LayerImportStatusType.Completed;
                            break;
                        case "201":
                            layer.Status = LayerImportStatusType.Accepted;
                            break;
                        case "202":
                            layer.Status = LayerImportStatusType.Processing;
                            if (mr.MapRequestLayers.Select(l => l.Status == LayerImportStatusType.Processing).Count() == mr.MapRequestLayers.Count)
                                mr.Status = MapRequestStatusType.Processing;
                            break;
                        case "400":
                        case "404":
                        case "422":
                        case "500":
                            layer.ErrorMessages ??= new List<MapRequestLayerError>();
                            layer.ErrorMessages.Add(new MapRequestLayerError(eventData.message, eventData.acquisition_date != null && eventData.acquisition_date != DateTime.MinValue ? eventData.acquisition_date : DateTime.UtcNow));
                            layer.ErrorMessages.OrderBy(a => a.AcquisitionDate).ToList();
                            layer.ReceivedUpdates++;
                            if (layer.ReceivedUpdates == mr.ExpectedUpdates)
                            {
                                layer.Status = LayerImportStatusType.Completed;
                                mr.Status = MapRequestStatusType.ContentAvailable;
                            }
                            break;
                        default:
                            break;
                    }

                    CurrentUnitOfWork.SaveChanges();
                }
                catch (Exception e)
                {
                    Logger.ErrorFormat("HandleMapRequestStatusChange exception: {0}", e.Message);
                }

                unitOfWork.Complete();
            }
        }

        [UnitOfWork(IsDisabled = true)]
        private void HandleAlertMessage(RabbitMqAlert eventData)
        {
            using (var unitOfWork = UnitOfWorkManager.Begin(new UnitOfWorkOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                IsTransactional = true,
                Timeout = TimeSpan.FromMinutes(30),
                FilterOverrides =
                        {
                            new DataFilterConfiguration(AbpDataFilters.MayHaveTenant,false),
                            new DataFilterConfiguration(AbpDataFilters.MustHaveTenant, false)
                        }
            }))
            {
                try
                {
                    var alert = ObjectMapper.Map<Alert>(eventData);
                    var fullGeometry = eventData.Info.First().Area.First().FullGeometry;
                    alert.BoundingBox = fullGeometry.Envelope;
                    alert.IsARecommendation = true;
                    if (_dssSettings.Value.MustCreateAlert)
                    {
                        alert.Id = _alertManager.InsertAlertAndGetId(alert);
                        AlertAreaOfInterest alertAreaOfInterest = new AlertAreaOfInterest(alert.Id, fullGeometry);
                        alert.AlertAreaOfInterestId = _alertManager.InsertAlertAreaOfInterestAndGetId(alertAreaOfInterest);
                    }

                    CurrentUnitOfWork.SaveChanges();

                    string alertMessage = "";
                    foreach (var item in eventData.Info)
                    {
                        var info = ObjectMapper.Map<CapInfo>(item);
                        info.AlertId = alert.Id;
                        if (_dssSettings.Value.MustCreateAlert)
                            _alertManager.InsertCapInfoAndGetId(info);
                        
                        alertMessage = info.Description;
                    }

                    if (_dssSettings.Value.MustConvertAlertInCommunication)
                    {
                        Communication com = ObjectMapper.Map<Communication>(alert);
                        com.Message = alertMessage;
                        com.AreaOfInterest = fullGeometry;
                        com.CreationTime = DateTime.UtcNow;
                        var creator = _personManager.GetPersonByUsername(ErmesConsts.DEFAULT_DSS_USERNAME);
                        if (creator != null)
                        {
                            com.Creator = creator;
                            com.CreatorUserId = creator.Id;
                        }
                        else
                            throw new UserFriendlyException(L("MissingDssAccount"));

                        com.Id = _communicationManager.CreateOrUpdateCommunication(com);

                        AsyncHelper.RunSync(() => _communicationNotifier.SendCommunication(EntityWriteAction.Create, com.Id, com.CreatorUserId.Value, com.Message));
                    }

                    CurrentUnitOfWork.SaveChanges();
                }
                catch (Exception e)
                {
                    Logger.ErrorFormat("HandleAlertMessage exception: {0}", e.Message);
                }

                unitOfWork.Complete();
            }
        }

        [UnitOfWork(IsDisabled = true)]
        private void HandleCameraMessage(RabbitMqCameraEvent eventData)
        {
            using (var unitOfWork = UnitOfWorkManager.Begin(new UnitOfWorkOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                IsTransactional = true,
                Timeout = TimeSpan.FromMinutes(30),
                FilterOverrides =
                        {
                            new DataFilterConfiguration(AbpDataFilters.MayHaveTenant,false),
                            new DataFilterConfiguration(AbpDataFilters.MustHaveTenant, false)
                        }
            }))
            {
                try
                {

                    WebClient client = new WebClient();
                    byte[] fileBytes = client.DownloadData(eventData.Link);
                    var _azureCameraStorageManager = _azureManager.GetStorageManager(ResourceManager.GetBasePath(ResourceManager.Cameras.ContainerName));
                    var _azureCameraThumbnailStorageManager = _azureManager.GetStorageManager(ResourceManager.GetBasePath(ResourceManager.CameraThumbnails.ContainerName));

                    //TODO: to be generalized + implement thumbnails
                    string fileExtension = "jpg";
                    string uploadedFileName = string.Concat(Guid.NewGuid().ToString(), ".", fileExtension);
                    var fileNameWithFolder = ResourceManager.Cameras.GetRelativeMediaPath(eventData.Camera.Name, eventData.Camera.CamDirection, uploadedFileName);
                    AsyncHelper.RunSync(() => _azureCameraStorageManager.UploadFile(fileNameWithFolder, fileBytes, ErmesConsts.IMAGE_MIME_TYPE));

                    string thumbnailName = ResourceManager.CameraThumbnails.GetJpegThumbnailFilename(uploadedFileName);
                    string thumbnailPath = ResourceManager.CameraThumbnails.GetRelativeMediaPath(eventData.Camera.Name, eventData.Camera.CamDirection, thumbnailName);
                    try
                    {
                        AsyncHelper.RunSync(() => _azureCameraThumbnailStorageManager.UploadFile(thumbnailPath, ErmesCoreCommon.CreateThumbnailFromImage(fileBytes, ErmesConsts.Thumbnail.SIZE, ErmesConsts.Thumbnail.QUALITY, Logger), ErmesConsts.IMAGE_MIME_TYPE));
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e.Message);
                        Logger.WarnFormat("Upload of Thumbnail fails, try to upload original image");

                        AsyncHelper.RunSync(() => _azureCameraThumbnailStorageManager.UploadFile(thumbnailPath, fileBytes, ErmesConsts.IMAGE_MIME_TYPE));
                    }

                    var stations = AsyncHelper.RunSync(() => _sensorServiceManager.GetStations());
                    SensorServiceStation station = stations.Where(s => s.Name == eventData.Camera.Name).FirstOrDefault();
                    SensorServiceSensor sensor;
                    if (station == null) //create station and sensor if not present
                    {
                        station = AsyncHelper.RunSync(() => _sensorServiceManager.CreateStation(eventData.Camera.Name, eventData.Camera.Latitude, eventData.Camera.Longitude, eventData.Camera.Altitude, eventData.Camera.Owner, eventData.Camera.Model, eventData.Camera.Type));
                        sensor = AsyncHelper.RunSync(() => _sensorServiceManager.CreateSensor(station.Id, eventData.Camera.CamDirection, eventData.Camera.CamDirection, "degree"));
                    }
                    else
                    {
                        station = AsyncHelper.RunSync(() => _sensorServiceManager.GetStationInfo(station.Id));
                        sensor = station.Sensors.Where(s => s.Type == eventData.Camera.CamDirection).FirstOrDefault();
                        sensor ??= AsyncHelper.RunSync(() => _sensorServiceManager.CreateSensor(station.Id, eventData.Camera.CamDirection, eventData.Camera.CamDirection, "degree"));
                    }

                    //for optimization purposes during GetFeatureCollection, also check and create station locally
                    Station localStation = _stationManager.GetStationBySensorServiceId(station.Id);
                    if (localStation == null)
                    {
                        localStation = ObjectMapper.Map<Station>(station);
                        _stationManager.InsertStation(localStation);
                    }

                    object metadata = new
                    {
                        detection = new
                        {
                            not_available = eventData.Detection.NotAvailable,
                            fire = eventData.Detection.Fire,
                            smoke = eventData.Detection.Smoke,

                        },
                        class_of_fire = new
                        {
                            not_available = eventData.ClassOfFire.NotAvailable,
                            class_1 = eventData.ClassOfFire.Class1,
                            class_2 = eventData.ClassOfFire.Class2,
                            class_3 = eventData.ClassOfFire.Class3,
                        },
                        fire_location = new
                        {
                            not_available = eventData.FireLocation.NotAvailable,
                            direction = eventData.FireLocation.Direction,
                            distance = eventData.FireLocation.Distance,
                            latitude = eventData.FireLocation.Latitude,
                            longitude = eventData.FireLocation.Longitude
                        },
                        thumbnail_uri = ResourceManager.CameraThumbnails.GetMediaPath(eventData.Camera.Name, eventData.Camera.CamDirection, uploadedFileName)
                    };
                    AsyncHelper.RunSync(() => _sensorServiceManager.CreateMeasure(sensor.Id, DateTime.UtcNow, DateTime.UtcNow, ResourceManager.Cameras.GetMediaPath(eventData.Camera.Name, eventData.Camera.CamDirection, uploadedFileName), metadata));
                }
                catch (Exception e)
                {
                    Logger.ErrorFormat("HandleCameraEventMessage exception: {0}", e.Message);
                }

                unitOfWork.Complete();
            }
        }

        #endregion

        #region Kafka
        public void ConsumeKafkaNotification(string message)
        {
            try
            {
                var eventData = JsonConvert.DeserializeObject<BusDto<object>>(message);
                if (eventData != null)
                {
                    switch (eventData.EntityType)
                    {
                        case EntityType.Communication:
                            break;
                        case EntityType.Mission:
                            HandleMissionMessage(eventData);
                            break;
                        case EntityType.Report:
                            break;
                        case EntityType.Person:
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("ConsumeBusNotification Exception: {0}", e.Message);
            }

            return;
        }
        private void HandleMissionMessage(BusDto<object> eventData)
        {
            try
            {
                switch (eventData.EntityWriteAction)
                {
                    case EntityWriteAction.Create:
                        break;
                    case EntityWriteAction.Update:
                        break;
                    case EntityWriteAction.Delete:
                        break;
                    case EntityWriteAction.StatusChange:
                        var content = JsonConvert.DeserializeObject<MissionChangeStatusDto>(eventData.Content.ToString());
                        var data = new BusDto<MissionChangeStatusDto>()
                        {
                            EntityType = eventData.EntityType,
                            EntityWriteAction = eventData.EntityWriteAction,
                            Content = content
                        };

                        HandleMissionStatusChangeMessage(data);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("HandleMissionMessage Exception: {0}", e.Message);
            }

        }

        [UnitOfWork(IsDisabled = true)]
        private void HandleMissionStatusChangeMessage(BusDto<MissionChangeStatusDto> eventData)
        {
            using (var unitOfWork = UnitOfWorkManager.Begin(new UnitOfWorkOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                IsTransactional = true,
                Timeout = TimeSpan.FromMinutes(30),
                FilterOverrides =
                        {
                            new DataFilterConfiguration(AbpDataFilters.MayHaveTenant,false),
                            new DataFilterConfiguration(AbpDataFilters.MustHaveTenant, false)
                        }
            }))
            {
                try
                {
                    var mission = _missionManager.GetMissionById(eventData.Content.Id);
                    if (mission == null)
                    {
                        Logger.ErrorFormat("HandleMissionStatusChangeMessage: Mission with Id {0} not found", eventData.Content.Id);
                        return;
                    }

                    var person = _personManager.GetPersonByUsername(eventData.Content.Username);
                    if (person == null)
                    {
                        Logger.ErrorFormat("HandleMissionStatusChangeMessage: person with username {0} not found", eventData.Content.Username);
                        return;
                    }
                    switch (eventData.EntityWriteAction)
                    {
                        case EntityWriteAction.Create:
                        case EntityWriteAction.Update:
                        case EntityWriteAction.Delete:
                            break;
                        case EntityWriteAction.StatusChange:
                            if (_missionManager.CheckNewStatus(mission.CurrentStatus, eventData.Content.Status))
                            {
                                mission.CurrentStatus = eventData.Content.Status;

                                //Need to update status before sending the notification
                                CurrentUnitOfWork.SaveChanges();
                                MissionNotification content = new MissionNotification
                                {
                                    CoordinatorPersonId = mission.CoordinatorPersonId,
                                    CoordinatorTeamId = mission.CoordinatorTeamId,
                                    Description = mission.Description,
                                    Id = mission.Id,
                                    Notes = mission.Notes,
                                    OrganizationId = mission.OrganizationId,
                                    Status = mission.CurrentStatus,
                                    Title = mission.Title,
                                    Duration = mission.Duration
                                };
                                Logger.InfoFormat("Consumer Service is sending bus notification: {0} - {1} - {2}", eventData.EntityType, eventData.EntityWriteAction, eventData.Content.Id);
                                AsyncHelper.RunSync(() => _notifierService.SendBusNotification(person.Id, eventData.Content.Id, content, eventData.EntityWriteAction, eventData.EntityType));

                                //Send notification to the chatbot
                                string titleKey = "Notification_Mission_Update_Title";
                                string bodyKey = "Notification_Mission_Update_Status_Body";
                                string[] bodyParams = new string[] { mission.Title, eventData.Content.Status.ToString() };

                                var receivers = _missionManager.GetMissionCoordinators(mission.CoordinatorPersonId, mission.CoordinatorTeamId, mission.OrganizationId);
                                AsyncHelper.RunSync(() => _notifierService.SendUserNotification(mission.CreatorUserId.Value, receivers, mission.Id, (bodyKey, bodyParams), (titleKey, null), EntityWriteAction.StatusChange, EntityType.Mission));
                                CurrentUnitOfWork.SaveChanges();
                            }
                            else
                                Logger.ErrorFormat("Consumer Service: invalid new status ({0}) for mission {1}", eventData.Content.Status, eventData.Content.Id);
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception e)
                {
                    Logger.ErrorFormat("HandleMissionStatusChangeMessage exception: {0}", e.Message);
                }

                unitOfWork.Complete();
            }
        }
        #endregion
    }
}
