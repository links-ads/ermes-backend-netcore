using Abp.Domain.Services;
using Abp.Extensions;
using Ermes.Enums;
using Ermes.Notifications;
using Ermes.Notifiers;
using Ermes.Persons;
using Abp.Bus;
using Ermes.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ermes.Notifiers
{
    public class NotifierService : ErmesDomainServiceBase
    {
        private readonly INotifierBase _notifierBase;
        private readonly PersonManager _personManager;
        private readonly NotificationManager _notificationManager;

        private class BusDto<T>
        {
            public EntityType EntityType { get; set; }
            public EntityWriteAction EntityWriteAction { get; set; }
            public T Content { get; set; }
        }

        public NotifierService(
                INotifierBase notifierBase,
                PersonManager personManager,
                NotificationManager notificationManager)
        {
            _notifierBase = notifierBase;
            _personManager = personManager;
            _notificationManager = notificationManager;
        }

        public async Task SendBusNotification<T>(long creatorId, int entityId, T content, EntityWriteAction action, EntityType entityType)
        {
            BusDto<T> busPayload = new BusDto<T>
            {
                Content = content,
                EntityType = entityType,
                EntityWriteAction = action
            };
            var options = new JsonSerializerOptions();
            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

            string failureMessage = null;
            string serializedPayload = JsonSerializer.Serialize(busPayload, options);

            try
            {
                await _notifierBase.SendBusNotificationAsync(NotificationNameHelper.GetBusTopicName(entityType, action), serializedPayload);
            }
            catch (Exception ex)
            {
                failureMessage = ex.Message.Truncate(Notification.MaxFailureMessageLenght);
                Logger.ErrorFormat("Ermes: Failure to send bus message for {1} {2} due exception {3}. EntityId: {0}", entityId, action.ToString(), entityType.ToString(), ex.Message);
            }

            Notification not = new Notification()
            {
                Channel = NotificationChannelType.Bus,
                CreatorId = creatorId,
                Entity = entityType,
                EntityId = entityId,
                Title = null,
                Message = serializedPayload.Truncate(Notification.MaxMessageLength),
                Name = NotificationNameHelper.GetNotificationName(entityType, action),
                ReceiverId = null,
                Status = failureMessage == null ? NotificationStatus.Ok : NotificationStatus.Failed,
                Timestamp = DateTime.Now.ToUniversalTime(),
                FailureMessage = failureMessage
            };
            not.Id = await _notificationManager.CreateNotificationAsync(not);
        }


        public async Task SendUserNotification(long creatorId, IEnumerable<Person> receivers, int entityId, (string Key, string[] Params) body, (string Key, string[] Params) title, EntityWriteAction action, EntityType entityType)
        {
            string failureMessage = null;
            try
            {
                // Exclude creator from list of receivers
                receivers = receivers?.Where(p => p.Id != creatorId);

                //Retrieve registration token of receivers
                if (receivers != null && receivers.Count() > 0)
                {
                    //1) Send Push Notification
                    BaseNotificationData notData = new BaseNotificationData()
                    {
                        Body = body.Params != null ? L(body.Key, body.Params) : L(body.Key),
                        Title = title.Params != null ? L(title.Key, title.Params) : L(title.Key),
                        Receivers = receivers.Where(p => p.RegistrationToken != null).Select(p => p.RegistrationToken).ToList()
                    };
                    Dictionary<string, bool> pushResult;
                    try
                    {
                        pushResult = await _notifierBase.SendPushNotificationAsync(notData);
                    }
                    catch (Exception e)
                    {
                        Logger.InfoFormat("Ermes: PushNotifier not available: {0}", e.Message);
                        failureMessage = e.Message.Truncate(Notification.MaxFailureMessageLenght);
                        pushResult = null;
                    }
                    try
                    {
                        foreach (var per in receivers)
                        {
                            Notification not = new Notification()
                            {
                                Channel = NotificationChannelType.PushNotification,
                                CreatorId = creatorId,
                                Entity = entityType,
                                EntityId = entityId,
                                Title = notData.Title,
                                Message = notData.Body,
                                Name = NotificationNameHelper.GetNotificationName(entityType, action),
                                ReceiverId = per.Id,
                                Status = pushResult == null ? NotificationStatus.SystemDisabled :
                                            pushResult.Count == 0 ? NotificationStatus.UserInactive :
                                            per.RegistrationToken != null && pushResult[per.RegistrationToken] ? NotificationStatus.Ok :
                                            NotificationStatus.Failed,
                                Timestamp = DateTime.Now.ToUniversalTime(),
                                FailureMessage = failureMessage
                            };
                            not.Id = await _notificationManager.CreateNotificationAsync(not);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.ErrorFormat("Ermes: Error while storing PushNotification for {1} {2} Notification. EntityId: {0}. Error: {3}", entityId, action.ToString(), entityType.ToString(), e.Message);
                    }


                    //2) Send WebApi Notification
                    FullNotificationData webData = new FullNotificationData()
                    {
                        Body = body.Key,
                        Title = title.Key,
                        BodyParams = body.Params,
                        TitleParams = title.Params,
                        Receivers = receivers.Select(p => p.FusionAuthUserGuid.ToString()).ToList(),
                        EntityId = entityId,
                        EntityType = entityType
                    };
                    List<string> webApiResult;
                    try
                    {
                        webApiResult = await _notifierBase.SendWebApiNotificationAsync(webData);
                        failureMessage = null;
                    }
                    catch (Exception e)
                    {
                        Logger.ErrorFormat("Ermes: Chatbot not available: {0}", e.Message);
                        webApiResult = null;
                        failureMessage = e.Message.Truncate(Notification.MaxFailureMessageLenght);
                    }
                    try
                    {
                        foreach (var per in receivers)
                        {
                            Notification not = new Notification()
                            {
                                Channel = NotificationChannelType.WebApi,
                                CreatorId = creatorId,
                                Entity = entityType,
                                EntityId = entityId,
                                Title = title.Params != null ? L(title.Key, title.Params) : L(title.Key),
                                Message = body.Params != null ? L(body.Key, body.Params) : L(body.Key),
                                Name = NotificationNameHelper.GetNotificationName(entityType, action),
                                ReceiverId = per.Id,
                                Status = webApiResult == null ? NotificationStatus.SystemDisabled : webApiResult.Contains(per.FusionAuthUserGuid.ToString()) ? NotificationStatus.Ok : NotificationStatus.Failed,
                                Timestamp = DateTime.Now.ToUniversalTime(),
                                FailureMessage = failureMessage
                            };
                            not.Id = await _notificationManager.CreateNotificationAsync(not);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.ErrorFormat("Ermes: Error while storing WebAPI notification for {1} {2} Notification. EntityId: {0}. Error: {3}", entityId, action.ToString(), entityType.ToString(), e.Message);
                    }
                }
                else
                    Logger.InfoFormat("Ermes: No receiver for {1} {2} Notification, EntityId: {0}", entityId, action.ToString(), entityType.ToString());
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("Ermes: Error while sending notification for {1} {2} Notification. EntityId: {0}. Error: {3}", entityId, action.ToString(), entityType.ToString(), e.Message);
            }
            return;
        }
    }

}
