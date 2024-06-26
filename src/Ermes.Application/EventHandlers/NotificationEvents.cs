﻿using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Abp.Events.Bus.Handlers;
using Ermes.Communications.Dto;
using Ermes.Enums;
using Ermes.Gamification.Dto;
using Ermes.MapRequests.Dto;
using Ermes.Missions;
using Ermes.Missions.Dto;
using Ermes.Notifiers;
using Ermes.Persons;
using Ermes.Reports.Dto;
using Ermes.Teams.Dto;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

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
        private readonly CommunicationNotifier _communicationNotifier;
        public CommunicationNotificationEventHandler(CommunicationNotifier communicationNotifier)
        {
            _communicationNotifier = communicationNotifier;
        }

        [UnitOfWork]
        public virtual async Task HandleEventAsync(NotificationEvent<CommunicationNotificationDto> eventData)
        {
            await _communicationNotifier.SendCommunication(eventData.Action, eventData.EntityId, eventData.CreatorId, eventData.Content.Message, eventData.Content.OrganizationReceiverIds);
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
                        bodyKey = "Notification_Gamification_FirstReport_Body";
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
