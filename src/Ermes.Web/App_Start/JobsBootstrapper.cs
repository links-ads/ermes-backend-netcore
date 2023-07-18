using Ermes.Jobs;
using Hangfire;
using System;

namespace Ermes.Web.App_Start
{
    public class JobsBootstrapper
    {
        public static void SetupJobs()
        {
            #if !DEBUG
                RecurringJob.AddOrUpdate<PurgeStationImagesJob>("purge-station-images-job", methodCall: job => job.Execute(new PurgeStationImagesJobArgs()), cronExpression: ErmesConsts.PurgeStationImagesJob.startingCron, timeZone: TimeZoneInfo.Utc);
                RecurringJob.AddOrUpdate<PurgeNotificationReceivedJob>("purge-notifications-received-job", methodCall: job => job.Execute(new PurgeNotificationReceivedJobArgs()), cronExpression: ErmesConsts.PurgeNotificationReceivedJob.startingCron, timeZone: TimeZoneInfo.Utc);
            #endif
        }
    }
}
