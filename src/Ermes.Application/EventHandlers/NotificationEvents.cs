using Abp.Events.Bus;
using Abp.Events.Bus.Handlers;
using Ermes.Communications.Dto;
using Ermes.Enums;
using Ermes.Notifiers;
using Ermes.Persons;
using System.Threading.Tasks;
using System.Linq;
using Ermes.Reports.Dto;
using Ermes.Missions;
using Ermes.Missions.Dto;
using Abp.Dependency;
using Abp.Domain.Uow;
using Ermes.Communications;
using Ermes.GeoJson;
using System.Collections.Generic;
using Newtonsoft.Json;
using Ermes.Actions.Dto;
using Microsoft.EntityFrameworkCore;
using Ermes.MapRequests;
using Ermes.MapRequests.Dto;
using Ermes.Gamification.Dto;
using Ermes.Teams.Dto;

namespace Ermes.EventHandlers
{
    public class NotificationEvent<T> : EventData
    {
        public NotificationEvent(int entityId, long creatorId, T content, EntityWriteAction action, bool includeCreator = false)
        {
            EntityId = entityId;
            CreatorId = creatorId;
            Content = content;
            Action = action;
            IncludeCreator = includeCreator;
        }

        public int EntityId { get; private set; }
        public long CreatorId { get; private set; }
        public T Content { get; private set; }
        public EntityWriteAction Action { get; private set; }
        public bool IncludeCreator { get; set; }
    }

    public class CommunicationNotificationEventHandler : IAsyncEventHandler<NotificationEvent<CommunicationNotificationDto>>, ITransientDependency
    {
        private readonly NotifierService _notifierService;
        private readonly PersonManager _personManager;
        private readonly CommunicationManager _communicationManager;
        private readonly IGeoJsonBulkRepository _geoJsonBulkRepository;
        public CommunicationNotificationEventHandler(NotifierService notifierService, PersonManager personManager, CommunicationManager communicationManager, IGeoJsonBulkRepository geoJsonBulkRepository)
        {
            _notifierService = notifierService;
            _personManager = personManager;
            _communicationManager = communicationManager;
            _geoJsonBulkRepository = geoJsonBulkRepository;
        }

        [UnitOfWork]
        public virtual async Task HandleEventAsync(NotificationEvent<CommunicationNotificationDto> eventData)
        {
            await _notifierService.SendBusNotification(eventData.CreatorId, eventData.EntityId, eventData.Content, eventData.Action, EntityType.Communication);

            string titleKey = (eventData.Action == EntityWriteAction.Create) ? "Notification_Communication_Create_Title" : "Notification_Communication_Update_Title";
            string[] bodyParams = new string[] { eventData.Content.Message };
            //int? orgId = (await _personManager.GetPersonByIdAsync(eventData.CreatorId)).OrganizationId;
            //if (!orgId.HasValue)
            //    return;
            List<long> personIdList;
            try
            {
                //Need to filter receivers by the Area of Interest of the communication
                var comm = await _communicationManager.GetCommunicationByIdAsync(eventData.EntityId);

                //Exclude persons in status = Off; citizens are by default in status = Ready
                var statusTypes = new List<ActionStatusType>() { ActionStatusType.Active, ActionStatusType.Moving, ActionStatusType.Ready };

                var items = _geoJsonBulkRepository.GetPersonActions(comm.Duration.LowerBound, comm.Duration.UpperBound, eventData.Content.OrganizationReceiverIds?.ToArray(), statusTypes, null, comm.AreaOfInterest, null, "en", comm.Scope, comm.Restriction);
                var actions = JsonConvert.DeserializeObject<GetActionsOutput>(items);
                actions.PersonActions ??= new List<PersonActionDto>();
                
                personIdList = actions.PersonActions.Select(a => a.PersonId).ToList();
            }
            catch
            {
                personIdList = null;
            }

            var receivers = _personManager
                                .Persons
                                .Include(p => p.Organization)
                                .Where(p => personIdList != null && personIdList.Contains(p.Id));
            await _notifierService.SendUserNotification(eventData.CreatorId, receivers, eventData.EntityId, ("Notification_Communication_Create_Body", bodyParams), (titleKey, null), eventData.Action, EntityType.Communication);
        }
    }

    public class ReportRequestNotificationEventHandler : IAsyncEventHandler<NotificationEvent<ReportRequestNotificationDto>>, ITransientDependency
    {
        private readonly NotifierService _notifierService;
        private readonly PersonManager _personManager;
        public ReportRequestNotificationEventHandler(NotifierService notifierService, PersonManager personManager)
        {
            _notifierService = notifierService;
            _personManager = personManager;
        }

        [UnitOfWork]
        public virtual async Task HandleEventAsync(NotificationEvent<ReportRequestNotificationDto> eventData)
        {
            await _notifierService.SendBusNotification(eventData.CreatorId, eventData.EntityId, eventData.Content, eventData.Action, EntityType.ReportRequest);

            string titleKey = null, bodyKey = null;
            string[] bodyParams = new string[] { eventData.Content.Title };

            switch(eventData.Action)
            {
                case EntityWriteAction.Create:
                    {
                        titleKey = "Notification_ReportRequest_Create_Title";
                        bodyKey = "Notification_ReportRequest_Create_Body";
                        break;
                    }
                case EntityWriteAction.Update:
                    {
                        titleKey = "Notification_ReportRequest_Update_Title";
                        bodyKey = "Notification_ReportRequest_Update_Body";
                        break;
                    }
                case EntityWriteAction.Delete:
                    {
                        titleKey = "Notification_ReportRequest_Delete_Title";
                        bodyKey = "Notification_ReportRequest_Delete_Body";
                        break;
                    }
            }

            int? orgId = (await _personManager.GetPersonByIdAsync(eventData.CreatorId)).OrganizationId;
            if (!orgId.HasValue)
                return;
            var receivers = _personManager.Persons.Where(p => p.OrganizationId == orgId);

            await _notifierService.SendUserNotification(eventData.CreatorId, receivers, eventData.EntityId, (bodyKey, bodyParams), (titleKey, null), eventData.Action, EntityType.ReportRequest);
        }
    }

    public class MissionNotificationEventHandler : IAsyncEventHandler<NotificationEvent<MissionNotificationDto>>, IAsyncEventHandler<NotificationEvent<MissionNotificationTestDto>>, ITransientDependency
    {
        private readonly NotifierService _notifierService;
        private readonly MissionManager _missionManager;
        public MissionNotificationEventHandler(NotifierService notifierService, MissionManager missionManager)
        {
            _notifierService = notifierService;
            _missionManager = missionManager;
        }

        [UnitOfWork]
        public virtual async Task HandleEventAsync(NotificationEvent<MissionNotificationDto> eventData)
        {
            await _notifierService.SendBusNotification(eventData.CreatorId, eventData.EntityId, eventData.Content, eventData.Action, EntityType.Mission);
            string titleKey = null, bodyKey = null;
            string[] bodyParams = null;

            switch (eventData.Action)
            {
                case EntityWriteAction.Create:
                    {
                        titleKey = "Notification_Mission_Create_Title";
                        bodyKey = "Notification_Mission_Create_Body";
                        bodyParams = new string[] { eventData.Content.Title };
                        break;
                    }
                case EntityWriteAction.Update:
                    {
                        titleKey = "Notification_Mission_Update_Title";
                        bodyKey = "Notification_Mission_Update_Body";
                        bodyParams = new string[] { eventData.Content.Title };
                        break;
                    }
                case EntityWriteAction.StatusChange:
                    {
                        titleKey = "Notification_Mission_Update_Title";
                        bodyKey = "Notification_Mission_Update_Status_Body";
                        bodyParams = new string[] { eventData.Content.Title, eventData.Content.Status.ToString() };
                        break;
                    }
            }

            var receivers = _missionManager.GetMissionCoordinators(eventData.Content.CoordinatorPersonId, eventData.Content.CoordinatorTeamId, eventData.Content.OrganizationId);
            await _notifierService.SendUserNotification(eventData.CreatorId, receivers, eventData.EntityId, (bodyKey, bodyParams), (titleKey, null), eventData.Action, EntityType.Mission);
        }

        [UnitOfWork]
        public virtual async Task HandleEventAsync(NotificationEvent<MissionNotificationTestDto> eventData)
        {
            await _notifierService.SendTestBusNotification(eventData.CreatorId, eventData.EntityId, eventData.Content, eventData.Action, EntityType.Mission, eventData.Content.TopicName);
        }
    }

    public class ReportNotificationEventHandler : IAsyncEventHandler<NotificationEvent<ReportNotificationDto>>, ITransientDependency
    {
        private readonly NotifierService _notifierService;
        public ReportNotificationEventHandler(NotifierService notifierService)
        {
            _notifierService = notifierService;
        }

        [UnitOfWork]
        public virtual async Task HandleEventAsync(NotificationEvent<ReportNotificationDto> eventData)
        {
            await _notifierService.SendBusNotification(eventData.CreatorId, eventData.EntityId, eventData.Content, eventData.Action, EntityType.Report);
        }
    }

    public class MapRequestNotificationEventHandler : IAsyncEventHandler<NotificationEvent<MapRequestNotificationDto>>, ITransientDependency
    {
        private readonly NotifierService _notifierService;
        public MapRequestNotificationEventHandler(NotifierService notifierService)
        {
            _notifierService = notifierService;
        }

        [UnitOfWork]
        public virtual async Task HandleEventAsync(NotificationEvent<MapRequestNotificationDto> eventData)
        {
            await _notifierService.SendBusNotification(eventData.CreatorId, eventData.EntityId, eventData.Content, eventData.Action, EntityType.MapRequest, true);
        }
    }

    public class GamificationNotificationEventHandler : IAsyncEventHandler<NotificationEvent<GamificationNotificationDto>>, ITransientDependency
    {
        private readonly NotifierService _notifierService;
        private readonly PersonManager _personManager;
        public GamificationNotificationEventHandler(NotifierService notifierService, PersonManager personManager)
        {
            _notifierService = notifierService;
            _personManager = personManager;
        }

        [UnitOfWork]
        public virtual async Task HandleEventAsync(NotificationEvent<GamificationNotificationDto> eventData)
        {
            string titleKey = null, bodyKey = null;
            string[] bodyParams = null;

            switch (eventData.Action)
            {
                case EntityWriteAction.LevelChangeUp:
                    {
                        titleKey = "Notification_Gamification_LevelChange_Title";
                        bodyKey = "Notification_Gamification_LevelChange_Body";
                        bodyParams = new string[] { eventData.Content.NewValue };
                        break;
                    }
                case EntityWriteAction.LevelChangeDown:
                    {
                        titleKey = "Notification_Gamification_LevelChangeDown_Title";
                        bodyKey = "Notification_Gamification_LevelChangeDown_Body";
                        bodyParams = new string[] { eventData.Content.NewValue };
                        break;
                    }
                case EntityWriteAction.MedalObtained:
                    {
                        titleKey = "Notification_Gamification_MedalObtained_Title";
                        bodyKey = "Notification_Gamification_MedalObtained_Body";
                        bodyParams = new string[] { eventData.Content.NewValue, eventData.Content.EarnedPoints.ToString() };
                        break;
                    }
                case EntityWriteAction.BadgeObtained:
                    {
                        titleKey = "Notification_Gamification_BadgeObtained_Title";
                        bodyKey = "Notification_Gamification_BadgeObtained_Body";
                        bodyParams = new string[] { eventData.Content.NewValue, eventData.Content.EarnedPoints.ToString() };
                        break;
                    }
                case EntityWriteAction.FirstLogin:
                    {
                        titleKey = "Notification_Gamification_FirstLogin_Title";
                        bodyKey = "Notification_Gamification_FirstLogin_Body";
                        bodyParams = new string[] { eventData.Content.NewValue, eventData.Content.EarnedPoints.ToString() };
                        break;
                    }
                case EntityWriteAction.CompleteWizard:
                    {
                        titleKey = "Notification_Gamification_CompleteWizard_Title";
                        bodyKey = "Notification_Gamification_CompleteWizard_Body";
                        bodyParams = new string[] { eventData.Content.NewValue, eventData.Content.EarnedPoints.ToString() };
                        break;
                    }
                case EntityWriteAction.FirstReport:
                    {
                        titleKey = "Notification_Gamification_FirstReport_Title";
                        bodyKey =  "Notification_Gamification_FirstReport_Body";
                        bodyParams = new string[] { eventData.Content.NewValue, eventData.Content.EarnedPoints.ToString() };
                        break;
                    }
                default:
                    {
                        titleKey = "";
                        bodyKey = "";
                        break;
                    }
            }
            
            var receivers = _personManager.Persons.Where(p => p.Id == eventData.Content.PersonId);

            await _notifierService.SendUserNotification(eventData.CreatorId, receivers, eventData.EntityId, (bodyKey, bodyParams), (titleKey, null), eventData.Action, EntityType.Gamification, eventData.IncludeCreator);
        }
    }

    public class TeamNotificationEventHandler : IAsyncEventHandler<NotificationEvent<TeamNotificationDto>>, ITransientDependency
    {
        private readonly NotifierService _notifierService;
        private readonly PersonManager _personManager;
        public TeamNotificationEventHandler(NotifierService notifierService, PersonManager personManager)
        {
            _notifierService = notifierService;
            _personManager = personManager;
        }

        [UnitOfWork]
        public virtual async Task HandleEventAsync(NotificationEvent<TeamNotificationDto> eventData)
        {
            string titleKey = (eventData.Action == EntityWriteAction.TeamAssociation) ? "Notification_Team_PersonAssociation_Title" : "Notification_Team_PersonDissociation_Title";
            string[] bodyParams = new string[] { eventData.Content.Name };

            var receivers = _personManager
                                .Persons
                                .Include(p => p.Organization)
                                .Where(p => eventData.Content.Guids != null && eventData.Content.Guids.Contains(p.FusionAuthUserGuid));
            await _notifierService.SendUserNotification(eventData.CreatorId, receivers, eventData.EntityId, ((eventData.Action == EntityWriteAction.TeamAssociation) ? "Notification_Team_PersonAssociation_Body" : "Notification_Team_PersonDissociation_Body", bodyParams), (titleKey, null), eventData.Action, EntityType.Team);
        }
    }
}
