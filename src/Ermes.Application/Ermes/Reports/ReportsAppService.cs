﻿using Abp.Application.Services.Dto;
using Abp.BackgroundJobs;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.UI;
using Ermes.Attributes;
using Ermes.Authorization;
using Ermes.Categories;
using Ermes.Dto;
using Ermes.Dto.Datatable;
using Ermes.Dto.Spatial;
using Ermes.Enums;
using Ermes.EventHandlers;
using Ermes.Gamification;
using Ermes.Gamification.Dto;
using Ermes.GeoJson;
using Ermes.Helpers;
using Ermes.Linq.Extensions;
using Ermes.Persons;
using Ermes.Reports.Dto;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ermes.Reports
{
    [ErmesAuthorize]
    public class ReportsAppService : ErmesAppServiceBase, IReportsAppService
    {
        private readonly CategoryManager _categoryManager;
        private readonly ReportManager _reportManager;
        private readonly GamificationManager _gamificationManager;
        private readonly PersonManager _personManager;
        private readonly ErmesAppSession _session;
        private readonly ErmesPermissionChecker _permissionChecker;
        private readonly IGeoJsonBulkRepository _geoJsonBulkRepository;
        private readonly IBackgroundJobManager _backgroundJobManager;

        public ReportsAppService(
            CategoryManager categoryManager,
            ReportManager reportManager,
            GamificationManager gamificationManager,
            PersonManager personManager,
            IGeoJsonBulkRepository geoJsonBulkRepository,
            ErmesAppSession session,
            IBackgroundJobManager backgroundJobManager,
            ErmesPermissionChecker permissionChecker
        )
        {
            _categoryManager = categoryManager;
            _reportManager = reportManager;
            _gamificationManager = gamificationManager;
            _personManager = personManager;
            _session = session;
            _backgroundJobManager = backgroundJobManager;
            _geoJsonBulkRepository = geoJsonBulkRepository;
            _permissionChecker = permissionChecker;
        }

        #region Private
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
                query = query.Include(a => a.Creator).Include(a => a.Creator.Organization).Include(a => a.Validations);
            }
            else
                query = _reportManager.GetReports(input.StartDate.Value, input.EndDate.Value);

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

            if (input.Visibility != VisibilityType.All)
            {
                if (input.Visibility == VisibilityType.Private)
                    query = query.Where(r => !r.IsPublic);
                else
                    query = query.Where(r => r.IsPublic);
            }

            query = query.DTFilterBy(input);

            var person = _session.LoggedUserPerson;
            var hasPermission = _permissionChecker.IsGranted(_session.Roles, AppPermissions.Reports.Report_CanSeeCrossOrganization);
            if (!hasPermission)
            {
                //Citizen can only see public reports
                bool isCitizen = _session.Roles.Contains(AppRoles.CITIZEN);
                query = query.DataOwnership(person.OrganizationId.HasValue ? new List<int>() { person.OrganizationId.Value } : null, null, isCitizen ? VisibilityType.Public : input.Visibility);
            }

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
                            .Select(r =>
                            {
                                r.IsEditable = person.Id == r.CreatorId;
                                r.CanBeValidated = person.Id != r.CreatorId && r.Validations.Where(a => a.PersonId == person.Id).Count() == 0;
                                r.Upvotes = r.Validations.Count(b => b.IsValid);
                                r.Downvotes = r.Validations.Count(b => !b.IsValid);
                                return r;
                            })
                            .ToList();
            return result;
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
                    - StartDate and EndDate to define a time window of interest
                    - FilterByCreator: if true, only reports created by current logged user are fetched
                    - SouthWestBoundary: bottom-left corner of the bounding box for a spatial query. (optional) (to be filled together with NorthEast property)
                    - NorthEastBoundary: top-right corner of the bounding box for a spatial query format. (optional) (to be filled together with SouthWest property)
                    - Visibility: if Private, only reports created by professional are fetched. If Public, only reports created by citizens are fetched. If All, all reports are fetched 
                    - Contents: list of content types of interest
                Output: list of ReportDto elements

                N.B.: A person has visibility only on reports belonging to his organization
            "
        )]
        public virtual async Task<DTResult<ReportDto>> GetReports(GetReportsInput input)
        {
            PagedResultDto<ReportDto> result = await InternalGetReports(input);
            return new DTResult<ReportDto>(input.Draw, result.TotalCount, result.Items.Count, result.Items.ToList());
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
            if (!hasPermission && !report.IsPublic)
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
            if (report.CreatorUserId != _session.UserId)
            {
                properties.CanBeValidated = report.Validations.Where(a => a.PersonId == _session.LoggedUserPerson.Id).Count() == 0;
                report.Read = properties.Read = true;
            }

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
            if (report == null)
                throw new UserFriendlyException(L("InvalidReportId", input.ReportId));
            if (report.Creator.OrganizationId != _session.LoggedUserPerson.OrganizationId)
                throw new UserFriendlyException(L("EntityOutsideOrganization"));

            if (report.Status == input.Status)
                return false;

            report.Status = input.Status;

            //Need to update status before sending the notification
            await CurrentUnitOfWork.SaveChangesAsync();

            //Bus Notification
            NotificationEvent<ReportNotificationDto> notification = new NotificationEvent<ReportNotificationDto>(report.Id,
                _session.UserId.Value,
                ObjectMapper.Map<ReportNotificationDto>(report),
                EntityWriteAction.StatusChange);
            await _backgroundJobManager.EnqueueEventAsync(notification);

            //The list contains the information about the notification to be sent
            //var list = await _gamificationManager.UpdatePersonGamificationProfileAsync(_session.LoggedUserPerson.Id, ErmesConsts.GamificationActionConsts.DO_REPORT);
            //foreach (var item in list)
            //{
            //    NotificationEvent<GamificationNotificationDto> gamNotification = new NotificationEvent<GamificationNotificationDto>(0,
            //    _session.LoggedUserPerson.Id,
            //    new GamificationNotificationDto()
            //    {
            //        PersonId = _session.LoggedUserPerson.Id,
            //        ActionName = ErmesConsts.GamificationActionConsts.DO_REPORT,
            //        NewValue = item.NewValue
            //    },
            //    item.Action,
            //    true);
            //    await _backgroundJobManager.EnqueueEventAsync(gamNotification);
            //}

            return true;
        }


        [OpenApiOperation("Validate report",
            @"
                Input: 
                    - ReportId: the id of the report to be validated
                    - IsValid: true to upvote the report, false otherwise
                    - RejectionNote: rejection note inserted by user, if IsValid == false
                Output: true if the operation has been successfully executed, false otherwise
                N.B: 
                    1) a person cannot validate his own reports;
                    2) a person can validate a report only once
                Exception: invalid report id 
            "
        )]
        public virtual async Task<GamificationResponse> ValidateReport(ValidateReportInput input)
        {
            var result = new GamificationResponse();
            var report = await _reportManager.GetReportByIdAsync(input.ReportId) ?? throw new UserFriendlyException(L("InvalidReportId", input.ReportId));

            if (report.CreatorUserId == AbpSession.UserId.Value)
                throw new UserFriendlyException(L("InvalidReportVerifierOwner"));

            if (await _reportManager.HasAlreadyValidatedReportAsync(AbpSession.UserId.Value, report.Id))
                throw new UserFriendlyException(L("InvalidReportVerifierDuplicate"));

            ReportValidation rp = new ReportValidation()
            {
                ReportId = report.Id,
                PersonId = AbpSession.UserId.Value,
                IsValid = input.IsValid,
                RejectionNote = input.IsValid ? null : input.RejectionNote
            };

            await _reportManager.InsertReportValidationAsync(rp);

            if (_session.Roles.Contains(AppRoles.CITIZEN))
            {
                Person person = await _personManager.GetPersonByIdAsync(_session.LoggedUserPerson.Id);
                var action = await _gamificationManager.GetActionByNameAsync(ErmesConsts.GamificationActionConsts.VALIDATE_REPORT);
                //The list contains the information about the notification to be sent
                var list = await _gamificationManager.UpdatePersonGamificationProfileAsync(_session.LoggedUserPerson.Id, action.Name, null);

                foreach (var item in list)
                {
                    NotificationEvent<GamificationNotificationDto> gamNotification = new NotificationEvent<GamificationNotificationDto>(0,
                    _session.LoggedUserPerson.Id,
                    new GamificationNotificationDto()
                    {
                        PersonId = _session.LoggedUserPerson.Id,
                        ActionName = item.Action.ToString(),
                        NewValue = item.NewValue,
                        EarnedPoints = item.EarnedPoints
                    },
                    item.Action,
                    true);
                    await _backgroundJobManager.EnqueueEventAsync(gamNotification);
                }
                result.Gamification = new GamificationBaseDto(person.Points, person.LevelId, person.Level?.Name, action != null ? action.Points : 0);
            }
            result.Response = new ResponseBaseDto()
            {
                Success = true
            };

            return result;
        }
    }
}
