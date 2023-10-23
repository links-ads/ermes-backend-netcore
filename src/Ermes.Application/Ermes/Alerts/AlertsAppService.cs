using Abp.Application.Services.Dto;
using Abp.Linq.Extensions;
using Abp.UI;
using Ermes.Alerts.Dto;
using Ermes.Dto;
using Ermes.Dto.Datatable;
using Ermes.Dto.Spatial;
using Ermes.Ermes.Alerts;
using Ermes.GeoJson;
using Ermes.Helpers;
using Ermes.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NpgsqlTypes;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ermes.Alerts
{
    public class AlertsAppService : ErmesAppServiceBase, IAlertsAppService
    {
        private readonly IGeoJsonBulkRepository _geoJsonBulkRepository;
        private readonly AlertManager _alertManager;
        public AlertsAppService(AlertManager alertManager, IGeoJsonBulkRepository geoJsonBulkRepository)
        {
            _alertManager = alertManager;
            _geoJsonBulkRepository = geoJsonBulkRepository;
        }

        #region Private
        private async Task<PagedResultDto<AlertDto>> InternalGetAlerts(GetAlertsInput input)
        {
            PagedResultDto<AlertDto> result = new PagedResultDto<AlertDto>();
            IQueryable<Alert> query;
            input.StartDate = input.StartDate.HasValue ? input.StartDate : DateTime.MinValue;
            input.EndDate = input.EndDate.HasValue ? input.EndDate : DateTime.MaxValue;

            if (input.NorthEastBoundary != null && input.SouthWestBoundary != null)
            {
                Geometry boundingBox = GeometryHelper.GetPolygonFromBoundaries(input.SouthWestBoundary, input.NorthEastBoundary);
                query = _geoJsonBulkRepository.GetAlerts(input.StartDate.Value, input.EndDate.Value, boundingBox);
                query = query.Include(a => a.Info);
            }
            else
                query = _alertManager.GetAlerts(input.StartDate.Value, input.EndDate.Value);

            if (input.Restrictions != null && input.Restrictions.Count > 0)
                query = query.Where(a => input.Restrictions.Contains(a.Restriction));

            query = query.DTFilterBy(input);

            result.TotalCount = await query.CountAsync();

            if (input?.Order != null && input.Order.Count == 0)
            {
                query = query.OrderByDescending(a => a.Sent);
                query = query.PageBy(input);
            }
            else
            {
                query = query.DTOrderedBy(input)
                    .PageBy(input);
            }

            var items = await query.ToListAsync();
            result.Items = ObjectMapper.Map<List<AlertDto>>(items);
            return result;
        }

        #endregion

        [OpenApiOperation("Get Alerts",
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
                In addition to pagination parameters, there are additional properties for communication filtering:
                    - StartDate and EndDate to define a time window of interest
                    - SouthWestBoundary: bottom-left corner of the bounding box for a spatial query. (optional) (to be filled together with NorthEast property)
                    - NorthEastBoundary: top-right corner of the bounding box for a spatial query format. (optional) (to be filled together with SouthWest property)
                    - Restriction: filter result by Restriction (Citizen/Professional)
                Output: list of AlertDto elements
            "
        )]
        public virtual async Task<DTResult<AlertDto>> GetAlerts(GetAlertsInput input)
        {
            PagedResultDto<AlertDto> result = await InternalGetAlerts(input);
            return new DTResult<AlertDto>(input.Draw, result.TotalCount, result.Items.Count, result.Items.ToList());
        }

        [OpenApiOperation("Get Alert by Id",
            @"
                Input: 
                        - Id: the id of the alert to be retrived
                        - IncludeArea: if true, the response will contain the geometry of the alert
                Output: GeoJson feature, with AlertDto element in Properties field
                Exception: invalid id of the alert
             "
        )]
        public virtual async Task<GetEntityByIdOutput<AlertDto>> GetAlertById(GetEntityByIdInput<int> input)
        {
            var alert = await _alertManager.GetAlertByIdAsync(input.Id);
            if (alert == null)
                throw new UserFriendlyException(L("InvalidEntityId", "Alert", input.Id));

            var writer = new GeoJsonWriter();
            var res = new GetEntityByIdOutput<AlertDto>()
            {
                Feature = new FeatureDto<AlertDto>()
                {
                    Geometry = input.IncludeArea ? writer.Write(alert.AlertAreaOfInterest.AreaOfInterest) : null,
                    Properties = ObjectMapper.Map<AlertDto>(alert)
                }
            };
            return res;
        }
    }
}
