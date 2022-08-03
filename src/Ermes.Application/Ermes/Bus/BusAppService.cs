using Abp.BackgroundJobs;
using Ermes.Attributes;
using Ermes.Bus.Dto;
using Ermes.Enums;
using Ermes.EventHandlers;
using Ermes.Jobs;
using Ermes.Missions;
using Ermes.Missions.Dto;
using Ermes.Persons;
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
        public BusAppService(
             IBackgroundJobManager backgroundJobManager,
             PersonManager personManager
            )
        {
            _backgroundJobManager = backgroundJobManager;
            _personManager = personManager;
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
            if (mustSendReport)
            {
                _backgroundJobManager.Enqueue<SendReportJob, SendReportJobArgs>(
                    new SendReportJobArgs
                    {
                        ReportId = 6
                    });
            }
        }
    }
}
