using Abp.Application.Services.Dto;
using Abp.BackgroundJobs;
using Abp.Importer;
using Abp.Linq.Extensions;
using Abp.UI;
using AutoMapper;
using Ermes.Attributes;
using Ermes.Authorization;
using Ermes.Dto;
using Ermes.Dto.Datatable;
using Ermes.Dto.Spatial;
using Ermes.Enums;
using Ermes.EventHandlers;
using Ermes.GeoJson;
using Ermes.Helpers;
using Ermes.Linq.Extensions;
using Ermes.MapRequests.Dto;
using Ermes.Notifiers.MessageBody;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NpgsqlTypes;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.MapRequests
{
    [ErmesAuthorize]
    public class MapRequestsAppService : ErmesAppServiceBase, IMapRequestsAppService
    {
        private readonly ErmesAppSession _session;
        private readonly MapRequestManager _mapRequestManager;
        private readonly IGeoJsonBulkRepository _geoJsonBulkRepository;
        private readonly ErmesPermissionChecker _permissionChecker;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IImporterManager _importerMananger;
        public MapRequestsAppService(
            ErmesAppSession session,
            MapRequestManager mapRequestManager,
            IGeoJsonBulkRepository geoJsonBulkRepository,
            ErmesPermissionChecker permissionChecker,
            IBackgroundJobManager backgroundJobManager,
            IImporterManager importerMananger
        )
        {
            _session = session;
            _mapRequestManager = mapRequestManager;
            _geoJsonBulkRepository = geoJsonBulkRepository;
            _permissionChecker = permissionChecker;
            _backgroundJobManager = backgroundJobManager;
            _importerMananger = importerMananger;
        }

        #region Private
        private async Task<MapRequest> GetMapRequestAsync(int mapRequestId)
        {
            var mapReq = await _mapRequestManager.GetMapRequestByIdAsync(mapRequestId);
            return CheckMapRequestOwnership(mapReq);
        }
        private async Task<MapRequest> GetMapRequestAsync(string  mapRequestCode)
        {
            var mapReq = await _mapRequestManager.GetMapRequestByCodeAsync(mapRequestCode);
            return CheckMapRequestOwnership(mapReq);
        }

        private MapRequest CheckMapRequestOwnership(MapRequest mapReq)
        {
            if (mapReq == null)
                throw new UserFriendlyException(L("InvalidEntityId", "MapRequest", mapReq.Code));
            if (mapReq.Creator.OrganizationId != _session.LoggedUserPerson.OrganizationId && mapReq.Creator.Organization.ParentId.HasValue && mapReq.Creator.Organization.ParentId.Value != _session.LoggedUserPerson.OrganizationId)
                throw new UserFriendlyException(L("EntityOutsideOrganization"));
            return mapReq;
        }
        private async Task<PagedResultDto<MapRequestDto>> InternalGetMapRequests(GetMapRequestsInput input, bool filterByOrganization = true)
        {
            PagedResultDto<MapRequestDto> result = new PagedResultDto<MapRequestDto>();

            IQueryable<MapRequest> query;
            input.StartDate = input.StartDate.HasValue ? input.StartDate : DateTime.MinValue;
            input.EndDate = input.EndDate.HasValue ? input.EndDate : DateTime.MaxValue;

            if (input.NorthEastBoundary != null && input.SouthWestBoundary != null)
            {
                Geometry boundingBox = GeometryHelper.GetPolygonFromBoundaries(input.SouthWestBoundary, input.NorthEastBoundary);
                query = _geoJsonBulkRepository.GetMapRequests(input.StartDate.Value, input.EndDate.Value, boundingBox);
                query = query.Include(a => a.Creator.Organization).Include(a => a.MapRequestLayers);
            }
            else
                query = _mapRequestManager.MapRequests.Where(a => new NpgsqlRange<DateTime>(input.StartDate.Value, input.EndDate.Value).Contains(a.Duration));

            if (input.Status != null && input.Status.Count > 0)
            {
                //input.Status.Contains throw an exception
                //I need to go through the strings rather then the enum
                var list = input.Status.Select(a => a.ToString()).ToList();
                query = query.Where(a => list.Contains(a.StatusString));
            }
            else
                query = query.Where(a => a.StatusString != MapRequestStatusType.Canceled.ToString());

            if (input.Layers != null && input.Layers.Count > 0)
            {
                var list = input.Layers.Select(a => a.ToString()).ToList();
                query = query.Where(a => list.Contains(a.LayerString));
            }

            if (input.Hazards != null && input.Hazards.Count > 0)
            {
                var list = input.Hazards.Select(a => a.ToString()).ToList();
                query = query.Where(a => list.Contains(a.HazardString));
            }

            query = query.DTFilterBy(input);

            var currentUserPerson = _session.LoggedUserPerson;

            //List of Missions available only for pro users
            var hasPermission = _permissionChecker.IsGranted(_session.Roles, AppPermissions.MapRequests.MapRequest_CanSeeCrossOrganization);
            if (!hasPermission)
            {
                if (filterByOrganization && currentUserPerson.OrganizationId.HasValue)
                    query = query.DataOwnership(new List<int>() { currentUserPerson.OrganizationId.Value });
                else
                    return result;
            }

            result.TotalCount = await query.CountAsync();

            if (input?.Order != null && input.Order.Count == 0)
            {
                query = query.OrderByDescending(a => a.Duration.LowerBound);
                query = query.PageBy(input);
            }
            else
            {
                query = query.DTOrderedBy(input)
                    .PageBy(input);
            }

            var items = await query.ToListAsync();
            result.Items = ObjectMapper.Map<List<MapRequestDto>>(items);

            return result;
        }

        private async Task<MapRequest> CreateMapRequestAsync(FeatureDto<MapRequestDto> featureDto)
        {
            var newMR = ObjectMapper.Map<MapRequest>(featureDto.Properties);
            if (featureDto.FullGeometry != null && featureDto.FullGeometry.IsValid)
                newMR.AreaOfInterest = featureDto.FullGeometry;
            else
                throw new UserFriendlyException(L("InvalidAOI"));

            newMR.Id = await _mapRequestManager.CreateOrUpdateMapRequestAsync(newMR, featureDto.Properties.DataTypeIds);

            await CurrentUnitOfWork.SaveChangesAsync();

            NotificationEvent<MapRequestNotificationDto> notification = new NotificationEvent<MapRequestNotificationDto>(newMR.Id,
                _session.UserId.Value,
                ObjectMapper.Map<MapRequestNotificationDto>(newMR),
                EntityWriteAction.Create);
            await _backgroundJobManager.EnqueueEventAsync(notification);

            return newMR;
        }
        #endregion

        [OpenApiOperation("Get Map Requests",
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
                In addition to pagination parameters, there are additional properties for mission filtering:
                    - Layers: list of layer types of interest
                    - Hazards: list of hazards of interest
                    - Status: list of MapRequestStatus of interest
                    - StartDate and EndDate to define a time window of interest
                    - SouthWestBoundary: bottom-left corner of the bounding box for a spatial query. (optional) (to be filled together with NorthEast property)
                    - NorthEastBoundary: top-right corner of the bounding box for a spatial query format. (optional) (to be filled together with SouthWest property)
                Output: list of MapRequestDto elements

                N.B.: A person has visibility only on map request belonging to his organization
            "
        )]
        public virtual async Task<DTResult<MapRequestDto>> GetMapRequests(GetMapRequestsInput input)
        {
            PagedResultDto<MapRequestDto> result = await InternalGetMapRequests(input);
            return new DTResult<MapRequestDto>(input.Draw, result.TotalCount, result.Items.Count, result.Items.ToList());
        }

        [OpenApiOperation("Get MapRequest by Id",
            @"
                Input: 
                        - Id: the id of the map request to be retrived
                        - IncludeArea: if true, the response will contain the geometry of the communication
                Output: GeoJson feature, with CommunicationDto element in Properties field
                Exception: invalid id of the map request
            "
        )]
        public virtual async Task<GetEntityByIdOutput<MapRequestDto>> GetMapRequestById(GetEntityByIdInput<int> input)
        {
            var mr = await GetMapRequestAsync(input.Id);
            var hasPermission = _permissionChecker.IsGranted(_session.Roles, AppPermissions.MapRequests.MapRequest_CanSeeCrossOrganization);
            if(!hasPermission)
            {
                //Father Org can see child contents
                //false the contrary
                if (
                        mr.Creator.OrganizationId != _session.LoggedUserPerson.OrganizationId &&
                        (
                            !mr.Creator.Organization.ParentId.HasValue ||
                            (
                                mr.Creator.Organization.ParentId.HasValue &&
                                mr.Creator.Organization.ParentId.Value != _session.LoggedUserPerson.OrganizationId
                            )
                        )
                    )
                    throw new UserFriendlyException(L("EntityOutsideOrganization"));
            }
            var writer = new GeoJsonWriter();
            var res = new GetEntityByIdOutput<MapRequestDto>()
            {
                Feature = new FeatureDto<MapRequestDto>()
                {
                    Geometry = input.IncludeArea ? writer.Write(mr.AreaOfInterest) : null,
                    Properties = ObjectMapper.Map<MapRequestDto>(mr)
                }
            };
            return res;
        }

        [OpenApiOperation("Delete Map Request",
            @"
                Input: the code of the map request to be deleted (format <partner_name>.<maprequest_code>)
                Output: object coming  if the operation has been excuted successfully, false otherwise
                Exception: invalid map request code
                Note: the map request will not be canceled, but its status will be set to canceled
            "
        )]
        public virtual async Task<object> DeleteMapRequest(string mapRequestCode)
        {
            var result = await _importerMananger.DeleteMapRequest(mapRequestCode);
            if (result != null && mapRequestCode.Contains("links"))
            {
                string[] chunks = mapRequestCode.Split(".");
                if (chunks.Length > 1)
                {
                    string code = chunks.ElementAt(1);
                    var mr = await GetMapRequestAsync(code);
                    await _mapRequestManager.DeleteMapRequestAsync(mr);
                }
            }
            
            return result;
        }

        [OpenApiOperation("Create or Update a Map Request",
            @"
                Input: GeoJson feature, with MapRequestDto element in Properties field
                If the input contains an Id > 0, an update is performed, otherwise a new entity is created
                Output: the Id the map request that has been created/updated
                This operation will trigger a message on the bus
            "
        )]
        public virtual async Task<CreateOrUpdateMapRequestOutput> CreateOrUpdateMapRequest(CreateOrUpdateMapRequestInput input)
        {
            MapRequest mapRequest;
            if (input.Feature.Properties.Id == 0)
                mapRequest = await CreateMapRequestAsync(input.Feature);
            else
                throw new NotImplementedException();

            var writer = new GeoJsonWriter();
            return new CreateOrUpdateMapRequestOutput()
            {
                Feature = new FeatureDto<MapRequestDto>()
                {
                    Geometry = writer.Write(mapRequest.AreaOfInterest),
                    Properties = ObjectMapper.Map<MapRequestDto>(mapRequest)
                }
            };
        }
    }
}
