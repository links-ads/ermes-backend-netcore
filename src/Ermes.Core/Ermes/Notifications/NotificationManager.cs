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
        protected IRepository<Notification, Guid> NotificationRepository { get; set; }

        public NotificationManager(
        IRepository<Notification, Guid> notificationRepository
    )
        {
            NotificationRepository = notificationRepository;
        }

        public async Task<Guid> CreateNotificationAsync(Notification not)
        {
            return await NotificationRepository.InsertOrUpdateAndGetIdAsync(not);
        }
    }
}
