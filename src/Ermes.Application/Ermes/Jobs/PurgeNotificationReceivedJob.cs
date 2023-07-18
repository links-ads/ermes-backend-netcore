using Abp.BackgroundJobs;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Uow;
using Ermes.Configuration;
using Ermes.Notifications;
using System;
using System.Threading.Tasks;

namespace Ermes.Jobs
{
    [Serializable]
    public class PurgeNotificationReceivedArgs
    {
    }

    public class PurgeNotificationReceivedJob : AsyncBackgroundJob<PurgeNotificationReceivedArgs>, ITransientDependency
    {
        private readonly NotificationManager _notificationManager;

        public PurgeNotificationReceivedJob(NotificationManager notificationManager)
        {
            _notificationManager = notificationManager;
        }

        [UnitOfWork(IsDisabled = true)]
        protected override async Task ExecuteAsync(PurgeNotificationReceivedArgs args)
        {

            if (SettingManager.GetSettingValue<bool>(AppSettings.JobSettings.NotificationReceived_JobEnabled))
            {
                var dayToKeeep = SettingManager.GetSettingValue<int>(AppSettings.JobSettings.NotificationReceived_DaysToBeKept);
                var refDate = DateTime.UtcNow.AddDays(-dayToKeeep);

                try
                {
                    //Notification received management
                    await _notificationManager.DeleteNotificationReceivedOlderThanDateAsync(refDate);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                }
            }

            return;
        }
    }
}
