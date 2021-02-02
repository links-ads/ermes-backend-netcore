using Ermes.Dto.Datatable;
using Ermes.Enums;
using Ermes.Filters;
using Ermes.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Notifications.Dto
{
    public class GetNotificationsInput: DTPagedSortedAndFilteredInputDto, IDateRangeFilter
    {
        public List<EntityType> EntityTypes { get; set; }
        public List<NotificationStatus> NotificationStatus { get; set; }
        public List<NotificationChannelType> ChannelTypes { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
