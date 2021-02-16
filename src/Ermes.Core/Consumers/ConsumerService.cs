using Abp.Domain.Uow;
using Abp.Threading;
using Ermes.Missions;
using Ermes.Notifiers;
using Ermes.Persons;
using Newtonsoft.Json;
using System;
using System.Transactions;

namespace Ermes.Consumers
{
    public class ConsumerService: ErmesDomainServiceBase
    {
        private readonly MissionManager _missionManager;
        private readonly PersonManager _personManager;
        private readonly NotifierService _notifierService;

        public ConsumerService(
            MissionManager missionManager, 
            NotifierService notifierService,
            PersonManager personManager)
        {
            _missionManager = missionManager;
            _notifierService = notifierService;
            _personManager = personManager;
        }

        public void ConsumeBusNotification(string message)
        {
            var eventData = JsonConvert.DeserializeObject<BusDto<object>>(message);
            if(eventData != null)
            {
                switch (eventData.EntityType)
                {
                    case Enums.EntityType.Communication:
                        break;
                    case Enums.EntityType.Mission:
                        HandleMissionMessage(eventData);
                        break;
                    case Enums.EntityType.ReportRequest:
                        break;
                    case Enums.EntityType.Report:
                        break;
                    case Enums.EntityType.Person:
                        break;
                    default:
                        break;
                }
            }

            return;
        }

        private void HandleMissionMessage(BusDto<object> eventData)
        {
            switch (eventData.EntityWriteAction)
            {
                case Enums.EntityWriteAction.Create:
                    break;
                case Enums.EntityWriteAction.Update:
                    break;
                case Enums.EntityWriteAction.Delete:
                    break;
                case Enums.EntityWriteAction.StatusChange:
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
                        case Enums.EntityWriteAction.Create:
                        case Enums.EntityWriteAction.Update:
                        case Enums.EntityWriteAction.Delete:
                            break;
                        case Enums.EntityWriteAction.StatusChange:
                            if (_missionManager.CheckNewStatus(mission.CurrentStatus, eventData.Content.Status))
                            {
                                mission.CurrentStatus = eventData.Content.Status;

                                //Need to update status before sending the notification
                                CurrentUnitOfWork.SaveChanges();
                                Logger.InfoFormat("Consumer Service is sending bus notification: {0} - {1} - {2}", eventData.EntityType, eventData.EntityWriteAction, eventData.Content.Id);
                                AsyncHelper.RunSync(() => _notifierService.SendBusNotification(person.Id, eventData.Content.Id, eventData.Content, eventData.EntityWriteAction, eventData.EntityType));
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
    }
}
