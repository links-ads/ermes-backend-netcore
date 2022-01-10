using Abp.Application.Services.Dto;
using Abp.Linq.Extensions;
using Abp.UI;
using Ermes.Attributes;
using Ermes.Categories;
using Ermes.Dto.Datatable;
using Ermes.Dto.Spatial;
using Ermes.Helpers;
using Ermes.Linq.Extensions;
using Ermes.ReportRequests;
using Ermes.Reports.Dto;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ermes.Dto;
using NetTopologySuite.IO;
using Microsoft.EntityFrameworkCore;
using NSwag.Annotations;
using Ermes.Enums;
using Ermes.EventHandlers;
using Abp.BackgroundJobs;
using Abp.Domain.Uow;
using NetTopologySuite.Geometries;
using Ermes.GeoJson;
using Ermes.Authorization;

namespace Ermes.Reports
{
    [ErmesAuthorize]
    public class ReportsAppService : ErmesAppServiceBase, IReportsAppService
    {
        private readonly CategoryManager _categoryManager;
        private readonly ReportManager _reportManager;
        private readonly ReportRequestManager _reportRequestManager;
        private readonly ErmesAppSession _session;
        private readonly ErmesPermissionChecker _permissionChecker;
        private readonly IGeoJsonBulkRepository _geoJsonBulkRepository;
        private readonly IBackgroundJobManager _backgroundJobManager;

        public ReportsAppService(
            CategoryManager categoryManager, 
            ReportManager reportManager, 
            ReportRequestManager reportRequestManager,
            IGeoJsonBulkRepository geoJsonBulkRepository,
            ErmesAppSession session,
            IBackgroundJobManager backgroundJobManager,
            ErmesPermissionChecker permissionChecker
        )
        {
            _categoryManager = categoryManager;
            _reportManager = reportManager;
            _session = session;
            _reportRequestManager = reportRequestManager;
            _backgroundJobManager = backgroundJobManager;
            _geoJsonBulkRepository = geoJsonBulkRepository;
            _permissionChecker = permissionChecker;
        }

        #region Private
        private async Task<ReportRequest> GetReportRequestAsync(int rrId)
        {
            var rr = await _reportRequestManager.GetReportRequestByIdAsync(rrId);
            if (rr == null)
                throw new UserFriendlyException(L("InvalidCommunicationId", rrId));

            return rr;
        }

        private async Task<PagedResultDto<ReportDto>> InternalGetReports(GetReportsInput input)
        {
            PagedResultDto<ReportDto> result = new PagedResultDto<ReportDto>();
            IQueryable<Report> query;
            input.StartDate = input.StartDate.HasValue ? input.StartDate : DateTime.MinValue;
            input.EndDate = input.EndDate.HasValue ? input.EndDate : DateTime.MaxValue;
            
            if (input.NorthEastBoundary != null && input.SouthWestBoundary != null)
            {
                Geometry boundingBox = GeometryHelper.GetPolygonFromBoundaries(input.SouthWestBoundary, input.NorthEastBoundary);
                query = _geoJsonBulkRepository.GetReports(input.StartDate.Value, input.EndDate.Value, boundingBox);
                query = query.Include(a => a.Creator).Include(a => a.Creator.Organization);
            }
            else
                query = _reportManager.Reports.Where(a => new NpgsqlRange<DateTime>(input.StartDate.Value, input.EndDate.Value).Contains(a.Timestamp));

            if (input.ReportRequestId > 0)
            {               
                //TODO: to be implemented (commented after bbox filtering)
                //var reportRequest = await _reportRequestManager.GetReportRequestByIdAsync(input.ReportRequestId);
                //if (reportRequest == null)
                //    throw new UserFriendlyException(L("InvalidReportRequestId", input.ReportRequestId));

                //query = _reportBulkRepository.GetReportsFilteredByReportRequest(reportRequest, AppConsts.Srid);
                //query = query
                //            .Where(a => reportRequest.Duration.Contains(a.Timestamp));
            }

            if (input.Status != null && input.Status.Count > 0)
            {
                var list = input.Status.Select(a => a.ToString()).ToList();
                query = query.Where(a => list.Contains(a.StatusString));
            }

            if (input.Hazards != null && input.Hazards.Count > 0)
            {
                var hazardList = input.Hazards.Select(a => a.ToString()).ToList();
                query = query.Where(a => hazardList.Contains(a.HazardString));
            }

            if (input.Contents != null && input.Contents.Count > 0)
            {
                var contentList = input.Contents.Select(a => a.ToString()).ToList();
                query = query.Where(a => contentList.Contains(a.ContentString));
            }

            query = query.DTFilterBy(input);

            var person = _session.LoggedUserPerson;
            var hasPermission = _permissionChecker.IsGranted(_session.Roles, AppPermissions.Reports.Report_CanSeeCrossOrganization);
            if(!hasPermission)
                query = query.DataOwnership(person.OrganizationId.HasValue ? new List<int>() { person.OrganizationId.Value } : null, null, input.Visibility);

            if (input.FilterByCreator)
                query = query.Where(r => r.CreatorUserId.HasValue && r.CreatorUserId.Value == person.Id);

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
            var tmp = ObjectMapper.Map<List<ReportDto>>(items);
            result.Items = tmp
                            .Select(r => { r.IsEditable = person.Id == r.CreatorId; return r; })
                            .ToList();
            return result;
        }

        private async Task<PagedResultDto<ReportRequestDto>> InternalGetReportRequests(GetReportRequestsInput input)
        {
            PagedResultDto<ReportRequestDto> result = new PagedResultDto<ReportRequestDto>();
            IQueryable<ReportRequest> query;
            input.StartDate = input.StartDate.HasValue ? input.StartDate : DateTime.MinValue;
            input.EndDate = input.EndDate.HasValue ? input.EndDate : DateTime.MaxValue;

            if (input.NorthEastBoundary != null && input.SouthWestBoundary != null)
            {
                Geometry boundingBox = GeometryHelper.GetPolygonFromBoundaries(input.SouthWestBoundary, input.NorthEastBoundary);
                query = _geoJsonBulkRepository.GetReportRequests(input.StartDate.Value, input.EndDate.Value, boundingBox);
            }
            else
                query = _reportRequestManager.ReportRequests.Where(a => new NpgsqlRange<DateTime>(input.StartDate.Value, input.EndDate.Value).Contains(a.Duration));


            query = query.DTFilterBy(input);

            var person = _session.LoggedUserPerson;
            query = query.DataOwnership(person.OrganizationId.HasValue ? new List<int>() { person.OrganizationId.Value } : null);

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
            result.Items = ObjectMapper.Map<List<ReportRequestDto>>(items);

            return result;
        }        

        private async Task<int> CreateReportRequestAsync(FeatureDto<ReportRequestDto> featureDto)
        {
            ReportRequest newReportrequest = ObjectMapper.Map<ReportRequest>(featureDto.Properties);
            if (featureDto.FullGeometry.IsValid)
                newReportrequest.AreaOfInterest = featureDto.FullGeometry;
            else
                throw new UserFriendlyException(L("InvalidAOI"));

            foreach (var categoryId in newReportrequest.SelectedCategories)
            {
                var cat = await _categoryManager.GetCategoryByIdAsync(categoryId);
                if (cat == null)
                    throw new UserFriendlyException(L("InvalidCategoryId", categoryId));
            }

            newReportrequest.Id = await _reportRequestManager.CreateOrUpdateReportRequestAsync(newReportrequest);

            await CurrentUnitOfWork.SaveChangesAsync();

            NotificationEvent<ReportRequestNotificationDto> notification = new NotificationEvent<ReportRequestNotificationDto>(newReportrequest.Id,
                _session.UserId.Value,
                ObjectMapper.Map<ReportRequestNotificationDto>(newReportrequest),
                EntityWriteAction.Create);
            await _backgroundJobManager.EnqueueEventAsync(notification);
           
            return newReportrequest.Id;
        }

        private async Task<int> UpdateReportRequestAsync(FeatureDto<ReportRequestDto> featureDto)
        {
            var repoReqDto = ObjectMapper.Map<ReportRequestDto>(featureDto.Properties);
            var rr = await _reportRequestManager.GetReportRequestByIdAsync(repoReqDto.Id);
            if (rr == null)
                throw new UserFriendlyException(L("InvalidReportRequestId", repoReqDto.Id));
            if (featureDto.FullGeometry.IsValid)
                rr.AreaOfInterest = featureDto.FullGeometry;
            else
                throw new UserFriendlyException(L("InvalidAOI"));

            foreach (var categoryId in repoReqDto.SelectedCategories)
            {
                var cat = await _categoryManager.GetCategoryByIdAsync(categoryId);
                if (cat == null)
                    throw new UserFriendlyException(L("InvalidCategoryId", categoryId));
            }

            ObjectMapper.Map(repoReqDto, rr);

            await CurrentUnitOfWork.SaveChangesAsync();

            NotificationEvent<ReportRequestNotificationDto> notification = new NotificationEvent<ReportRequestNotificationDto>(rr.Id,
                _session.UserId.Value,
                ObjectMapper.Map<ReportRequestNotificationDto>(rr),
                EntityWriteAction.Update);
            await _backgroundJobManager.EnqueueEventAsync(notification);

            return rr.Id;
        }
        #endregion

        [OpenApiOperation("Get categories",
            @"
                        Categories are divided into Group. Each group can contain more than one category
                        Every category is described by a name and set of constraints. In particular, there are two type of categories:
                            1) Range: the value of the category can be selected from a set of fixed value
                            2) Numeric: the value of the category is a numeric field that must fall between a min and a max value
                                For this case, a unit of measure must be specified
                        Input: none
                        Output: list of grouped categories
                    "
        )]
        public async Task<GetCategoriesOutput> GetCategories()
        {
            var res = new GetCategoriesOutput();
            var list = await _categoryManager.GetCategoriesAsync();

            res.Categories = list.Select(cat => ObjectMapper.Map<CategoryDto>(cat))
                                .GroupBy(cat => new { cat.Group, cat.GroupIcon, cat.GroupKey })
                                .Select(g => new CategoryGroupDto { Group = g.Key.Group, Categories = g.ToList(), GroupIcon = g.Key.GroupIcon, GroupKey = g.Key.GroupKey })
                                .ToList();

            return res;
        }

        [OpenApiOperation("Get Reports",
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
                In addition to pagination parameters, there are additional properties for report filtering:
                    - Hazards: list of hazards of interest
                    - Status: list of general status of interest
                    - ReportrequestId: reports can be filtered according the properties defined in the Reportrequest with this Id
                    - StartDate and EndDate to define a time window of interest
                    - FilterByCreator: if true, only reports created by current logged user are fetched
                    - SouthWestBoundary: bottom-left corner of the bounding box for a spatial query. (optional) (to be filled together with NorthEast property)
                    - NorthEastBoundary: top-right corner of the bounding box for a spatial query format. (optional) (to be filled together with SouthWest property)
                Output: list of ReportDto elements

                N.B.: A person has visibility only on reports belonging to his organization
            "
        )]
        public virtual async Task<DTResult<ReportDto>> GetReports(GetReportsInput input)
        {
            PagedResultDto<ReportDto> result = await InternalGetReports(input);
            return new DTResult<ReportDto>(input.Draw, result.TotalCount, result.Items.Count, result.Items.ToList());
        }

        [OpenApiOperation("Get ReportRequests",
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
                In addition to pagination parameters, there are additional properties for Report Request filtering:
                    - Status: list of general status of interest
                    - StartDate and EndDate to define a time window of interest
                    - SouthWestBoundary: bottom-left corner of the bounding box for a spatial query, in (lon, lat) format. (optional) (to be filled together with NorthEast property)
                    - NorthEastBoundary: top-right corner of the bounding box for a spatial query, in (lon, lat) format. (optional) (to be filled together with SouthWest property)
                Output: list of ReportRequestDto elements

                N.B.: A person has visibility only on report requests belonging to his organization
            "
        )]
        public virtual async Task<DTResult<ReportRequestDto>> GetReportRequests(GetReportRequestsInput input)
        {
            PagedResultDto<ReportRequestDto> result = await InternalGetReportRequests(input);
            return new DTResult<ReportRequestDto>(input.Draw, result.TotalCount, result.Items.Count, result.Items.ToList());
        }

        [OpenApiOperation("Create or Update a ReportRequest",
            @"
                Input: GeoJson feature, with ReportRequestDto element in Properties field
                If the input contains an Id > 0, an update is performed, otherwise a new report request is created
                Output: the Id the report request that has been created/updated
                This operation will trigger notifications
            "
        )]
        public virtual async Task<int> CreateOrUpdateReportRequest(CreateOrUpdateReportRequestInput input)
        {
            if (input.Feature.Properties.Id == 0)
                return await CreateReportRequestAsync(input.Feature);
            else
                return await UpdateReportRequestAsync(input.Feature);
        }

        [OpenApiOperation("Get Report by Id",
            @"
                Input: 
                        - Id: the id of the report to be retrived
                        - IncludeArea: if true, the response will contain the geometry of the report
                Output: GeoJson feature, with ReporttDto element in Properties field
                Exception: invalid id of the report
             "
        )]
        public virtual async Task<GetEntityByIdOutput<ReportDto>> GetReportById(GetEntityByIdInput<int> input)
        {
            var report = await _reportManager.GetReportByIdAsync(input.Id);
            if (report == null)
                throw new UserFriendlyException(L("InvalidReportId", input.Id));
            
            //report can be created by citizens
            //they won't have an organizationId associated
            var hasPermission = _permissionChecker.IsGranted(_session.Roles, AppPermissions.Reports.Report_CanSeeCrossOrganization);
            if (!hasPermission)
            {
                //Father Org can see child contents
                //false the contrary
                if (
                    report.Creator.OrganizationId.HasValue && 
                    (
                        report.Creator.OrganizationId.Value != _session.LoggedUserPerson.OrganizationId &&
                        (
                            !report.Creator.Organization.ParentId.HasValue ||
                            (
                                report.Creator.Organization.ParentId.HasValue && 
                                report.Creator.Organization.ParentId.Value != _session.LoggedUserPerson.OrganizationId
                            )
                        )
                    )
                )
                throw new UserFriendlyException(L("EntityOutsideOrganization"));
            }

            var properties = ObjectMapper.Map<ReportDto>(report);
            properties.IsEditable = (report.CreatorUserId == _session.UserId);
            var writer = new GeoJsonWriter();
            return new GetEntityByIdOutput<ReportDto>()
            {
                Feature = new FeatureDto<ReportDto>()
                {
                    Geometry = input.IncludeArea ? writer.Write(report.Location) : null,
                    Properties = properties
                }
            };
        }

        [OpenApiOperation("Update Report Status",
            @"
                Input: 
                        - Id: the id of the report to be be updated
                        - Status: new Status for the report
                Output: true in case of success, false otherwise
                Exception: invalid Id of the report
             "
        )]
        [UnitOfWork(false)]
        public virtual async Task<bool> UpdateReportStatus(UpdateReportStatusInput input)
        {
            Report report = await _reportManager.GetReportByIdAsync(input.ReportId);
            if(report == null)
                throw new UserFriendlyException(L("InvalidReportId", input.ReportId));
            if (report.Creator.OrganizationId != _session.LoggedUserPerson.OrganizationId)
                throw new UserFriendlyException(L("EntityOutsideOrganization"));

            if (report.Status == input.Status)
                return false;

            report.Status = input.Status;

            //Need to update status before sending the notification
            await CurrentUnitOfWork.SaveChangesAsync();

            NotificationEvent<ReportNotificationDto> notification = new NotificationEvent<ReportNotificationDto>(report.Id,
                _session.UserId.Value,
                ObjectMapper.Map<ReportNotificationDto>(report),
                EntityWriteAction.StatusChange);
            await _backgroundJobManager.EnqueueEventAsync(notification);

            return true;
        }

        [OpenApiOperation("Get ReportRequest by Id",
            @"
                Input: 
                        - Id: the id of the report request to be retrived
                        - IncludeArea: if true, the response will contain the geometry of the report request
                Output: GeoJson feature, with ReportRequestDto element in Properties field
                Exception: invalid id of the report request
             "
        )]
        public virtual async Task<GetEntityByIdOutput<ReportRequestDto>> GetReportRequestById(GetEntityByIdInput<int> input)
        {
            var reportReq = await _reportRequestManager.GetReportRequestByIdAsync(input.Id);
            if (reportReq == null)
                throw new UserFriendlyException(L("InvalidReportRequestId", input.Id));
            if (reportReq.Creator.OrganizationId != _session.LoggedUserPerson.OrganizationId)
                throw new UserFriendlyException(L("EntityOutsideOrganization"));

            var writer = new GeoJsonWriter();
            return new GetEntityByIdOutput<ReportRequestDto>()
            {
                Feature = new FeatureDto<ReportRequestDto>()
                {
                    Properties = ObjectMapper.Map<ReportRequestDto>(reportReq),
                    Geometry = input.IncludeArea ? writer.Write(reportReq.AreaOfInterest) : null,
                }
            };
        }

        [OpenApiOperation("Delete a ReportRequest",
            @"
                Input: the id of the report request to be deleted
                Output: true if the operation has been excuted successfully, false otherwise
                Exception: invalid report request Id 
            "
        )]
        public virtual async Task<bool> DeleteReportRequest(IdInput<int> input)
        {
            var rr = await GetReportRequestAsync(input.Id);
            await _reportRequestManager.DeleteReportRequestAsync(rr);

            NotificationEvent<ReportRequestNotificationDto> notification = new NotificationEvent<ReportRequestNotificationDto>(rr.Id,
                _session.UserId.Value,
                ObjectMapper.Map<ReportRequestNotificationDto>(rr),
                EntityWriteAction.Delete);
            await _backgroundJobManager.EnqueueEventAsync(notification);

            return true;
        }
    }
}
