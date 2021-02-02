using Ermes.Dto.Datatable;
using Ermes.Notifications.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Notifications
{
    public interface INotificationsAppService
    {
        Task<DTResult<NotificationDto>> GetNotifications(GetNotificationsInput input);
    }
}
