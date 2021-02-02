using Abp.Application.Services.Dto;
using Abp.Linq.Extensions;
using Ermes.Attributes;
using Ermes.Dto.Datatable;
using Ermes.Helpers;
using Ermes.Linq.Extensions;
using Ermes.Notifications.Dto;
using Ermes.Persons;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Notifications
{
    [ErmesAuthorize]
    public class NotificationsAppService : ErmesAppServiceBase, INotificationsAppService
    {
        private readonly NotificationManager _notificationManager;
        private readonly ErmesAppSession _session;
        private readonly PersonManager _personManager;

        public NotificationsAppService(
                NotificationManager notificationManager,
                ErmesAppSession session,
                PersonManager personManager
            )
        {
            _notificationManager = notificationManager;
            _session = session;
            _personManager = personManager;
        }
        #region Private
        private async Task<PagedResultDto<NotificationDto>> InternalGetNotifications(GetNotificationsInput input, bool filterByOrganization = true)
        {
            PagedResultDto<NotificationDto> result = new PagedResultDto<NotificationDto>();

            IQueryable<Notification> query = _notificationManager.Notifications;

            if (input.NotificationStatus != null && input.NotificationStatus.Count > 0)
            {
                //input.Status.Contains throw an exception
                //I need to go through the strings rather then the enum
                var list = input.NotificationStatus.Select(a => a.ToString()).ToList();
                query = query.Where(a => list.Contains(a.StatusString));
            }

            if (input.EntityTypes != null && input.EntityTypes.Count > 0)
            {
                //input.Status.Contains throw an exception
                //I need to go through the strings rather then the enum
                var list = input.EntityTypes.Select(a => a.ToString()).ToList();
                query = query.Where(a => list.Contains(a.EntityString));
            }

            if (input.ChannelTypes != null && input.ChannelTypes.Count > 0)
            {
                //input.Status.Contains throw an exception
                //I need to go through the strings rather then the enum
                var list = input.ChannelTypes.Select(a => a.ToString()).ToList();
                query = query.Where(a => list.Contains(a.ChannelString));
            }

            input.StartDate = input.StartDate.HasValue ? input.StartDate : DateTime.MinValue;
            input.EndDate = input.EndDate.HasValue ? input.EndDate : DateTime.MaxValue;

            query = query.Where(a => new NpgsqlRange<DateTime>(input.StartDate.Value, input.EndDate.Value).Contains(a.Timestamp));

            query = query.DTFilterBy(input);

            query = query.Where(n => n.ReceiverId == _session.UserId);

            result.TotalCount = await query.CountAsync();

            if (input?.Order != null && input.Order.Count == 0)
            {
                query = query.OrderByDescending(a => a.Timestamp);
                query = query.PageBy(input);
            }
            else
            {
                query = query.DTOrderedBy(input)
                    .PageBy(input);
            }

            var items = await query.ToListAsync();
            result.Items = ObjectMapper.Map<List<NotificationDto>>(items);

            return result;

        }
        #endregion

        [OpenApiOperation("Get Notifications",
            @"
                This is a server-side paginated API
                Input: use the following properties to filter result list:
                    - Draw: Draw counter. This is used by DataTables to ensure that the Ajax returns from server-side processing requests are drawn in sequence by 
                        DataTables (Ajax requests are asynchronous and thus can return out of sequence)
                    - MaxResultCount: number of records that the table can display in the current draw (must be >= 0)
                    - SkipCount: paging first record indicator. This is the start point in the current data set (0 index based - i.e. 0 is the first record)
                    - Search: 
                               - value: global search value
                               - regex: true if the global filter should be treated as a regular expression for advanced searching, false otherwise
                    - Order (is a list, for multi-column sorting):
                                - column: name of the column to which sorting should be applied
                                - dir: sorting direction
                In addition to pagination parameters, there are additional properties for notification filtering:
                    - ChannelTypes: list of channels of interest (i.e. push notifications, web api notifications, etc..)
                    - NotificationStatus: list of status, in terms of message delivery
                    - EntityTypes: list of entities of interest (i.e. mission, report, communication, etc..)
                    - StartDate and EndDate to define a time window of interest
                Output: list of NotificationDto elements

                N.B.: A person has visibility only on notifications belonging to his organization
            "
        )]
        public virtual async Task<DTResult<NotificationDto>> GetNotifications(GetNotificationsInput input)
        {
            PagedResultDto<NotificationDto> result = await InternalGetNotifications(input);
            return new DTResult<NotificationDto>(input.Draw, result.TotalCount, result.Items.Count, result.Items.ToList());
        }
    }
}
