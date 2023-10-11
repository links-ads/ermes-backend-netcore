using Abp.Domain.Repositories;
using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Notifications
{
    public class NotificationManager : DomainService
    {
        public IQueryable<Notification> Notifications { get { return NotificationRepository.GetAll(); } }
        public IQueryable<NotificationReceived> NotificationsReceived { get { return NotificationReceivedRepository.GetAll(); } }
        protected IRepository<Notification, Guid> NotificationRepository { get; set; }
        protected IRepository<NotificationReceived, Guid> NotificationReceivedRepository { get; set; }

        public NotificationManager(
            IRepository<Notification, Guid> notificationRepository,
            IRepository<NotificationReceived, Guid> notificationReceivedRepository
        )
        {
            NotificationRepository = notificationRepository;
            NotificationReceivedRepository = notificationReceivedRepository;
        }

        public async Task<Guid> CreateNotificationAsync(Notification not)
        {
            return await NotificationRepository.InsertOrUpdateAndGetIdAsync(not);
        }

        public async Task DeleteNotificationsByPersonIdAsync(long personId)
        {
            await NotificationRepository.DeleteAsync(a => (a.ReceiverId.HasValue && a.ReceiverId.Value == personId) || (a.CreatorId.HasValue && a.CreatorId.Value == personId));
        }

        public Guid InsertNotificationReceived(NotificationReceived notificationReceived) 
        { 
            return NotificationReceivedRepository.InsertOrUpdateAndGetId(notificationReceived);
        }

        public async Task DeleteNotificationReceivedOlderThanDateAsync(DateTime date)
        {
            await NotificationReceivedRepository.DeleteAsync(n => n.CreationTime < date);
        }
    }
}
