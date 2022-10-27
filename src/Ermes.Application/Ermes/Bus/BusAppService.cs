using Abp.BackgroundJobs;
using Abp.UI;
using Ermes.Attributes;
using Ermes.Bus.Dto;
using Ermes.Configuration;
using Ermes.Dto;
using Ermes.Enums;
using Ermes.EventHandlers;
using Ermes.ExternalServices.Csi;
using Ermes.Gamification.Dto;
using Ermes.Jobs;
using Ermes.Missions;
using Ermes.Missions.Dto;
using Ermes.Persons;
using Microsoft.AspNetCore.Http;
using NetTopologySuite.Index.HPRtree;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Bus
{
    //[ErmesIgnoreApi(true)]
    public class BusAppService : ErmesAppServiceBase, IBusAppService
    {
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly PersonManager _personManager;
        private readonly CsiManager _csiManager;
        public BusAppService(
             IBackgroundJobManager backgroundJobManager,
             PersonManager personManager,
             CsiManager csiManager
            )
        {
            _backgroundJobManager = backgroundJobManager;
            _personManager = personManager;
            _csiManager = csiManager;
        }

        public async Task TestBusConsumerTopic(TestBusConsumerTopicInput input)
        {
            var person = _personManager.GetPersonByUsername(input.Username);
            NotificationEvent<MissionNotificationTestDto> notification = new NotificationEvent<MissionNotificationTestDto>(input.MissionId,
                person.Id,
                new MissionNotificationTestDto{ 
                    Id = input.MissionId,
                    Status = input.Status,
                    Username = input.Username,
                    TopicName = input.TopicName
                },
                EntityWriteAction.StatusChange);
            await _backgroundJobManager.EnqueueEventAsync(notification);
        }

        public async Task TestCsiPresidiService()
        {
            bool mustSendReport = true;
            //It must be "Protezione Civile Piemonte"
            var housePartner = await SettingManager.GetSettingValueAsync(AppSettings.General.HouseOrganization);
            if (mustSendReport)
            {
                _backgroundJobManager.Enqueue<SendReportJob, SendReportJobArgs>(
                    new SendReportJobArgs
                    {
                        ReportId = 6
                    });
            }
        }

        [OpenApiOperation("Test Volter Tax Code availability",
            @"
                Check if the servive to retrieve the anonimyzed userId inside of Volter environment is up and running
                Output: true if the service is online, exception if the service is not reachable
                N.B.: the service is available only if called from inside Nivola environment
            "
        )]
        public async Task<bool> TestVolterTaxCodeService()
        {
            var person = _personManager.GetPersonByUsername("admin");
            int? legacyId = await _csiManager.SearchVolontarioAsync("BAAMMD66P02Z330V", person.Id);
            if (legacyId.HasValue && legacyId.Value >= 0)
                return true;
            else
                throw new UserFriendlyException(L("VolterServiceNotAvailable"));
        }

        public async Task TestGamificationNotification(TestBusConsumerTopicInput input)
        {
            var person = _personManager.GetPersonByUsername(input.Username);
            NotificationEvent<GamificationNotificationDto> notification = new NotificationEvent<GamificationNotificationDto>(0,
            person.Id,
            new GamificationNotificationDto()
            {
                PersonId = person.Id,
                ActionName = input.Action.ToString(),
                NewValue = input.NewRewardName
            },
            input.Action,
            true);
            await _backgroundJobManager.EnqueueEventAsync(notification);
        }
    }
}
