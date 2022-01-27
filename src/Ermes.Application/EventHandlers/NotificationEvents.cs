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

namespace Ermes.EventHandlers
{
    public class NotificationEvent<T> : EventData
    {
        public NotificationEvent(int entityId, long creatorId, T content, EntityWriteAction action)
        {
            EntityId = entityId;
            CreatorId = creatorId;
            Content = content;
            Action = action;
        }

        public int EntityId { get; private set; }
        public long CreatorId { get; private set; }
        public T Content { get; private set; }
        public EntityWriteAction Action { get; private set; }
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
            int? orgId = (await _personManager.GetPersonByIdAsync(eventData.CreatorId)).OrganizationId;
            if (!orgId.HasValue)
                return;
            List<string> personUsernameList;
            try
            {
                //Need to filter receivers by the Area of Interest of the communication
                var comm = await _communicationManager.GetCommunicationByIdAsync(eventData.EntityId);
                var statusTypes = new List<ActionStatusType>() { ActionStatusType.Active, ActionStatusType.Moving, ActionStatusType.Ready };
                var items = _geoJsonBulkRepository.GetPersonActions(comm.Duration.LowerBound, comm.Duration.UpperBound, new int[] { eventData.Content.OrganizationId.Value }, statusTypes, null, comm.AreaOfInterest, null, "en");
                var actions = JsonConvert.DeserializeObject<GetActionsOutput>(items);
                //PersonId not available, make check on username
                personUsernameList= actions.PersonActions.Select(a => a.Username).ToList();
            }
            catch
            {
                personUsernameList = null;
            }

            var receivers = _personManager
                                .Persons
                                .Include(p => p.Organization)
                                .Where(p => p.OrganizationId == orgId || (p.Organization.ParentId.HasValue && p.Organization.ParentId.Value == orgId))
                                .Where(p => personUsernameList != null && personUsernameList.Contains(p.Username));
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
}
