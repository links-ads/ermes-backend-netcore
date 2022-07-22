using Abp.Application.Services.Dto;
using Abp.Linq.Extensions;
using Abp.UI;
using Ermes.Communications.Dto;
using Ermes.Dto;
using Ermes.Dto.Datatable;
using Ermes.Dto.Spatial;
using Ermes.Enums;
using Ermes.Helpers;
using Ermes.Linq.Extensions;
using Ermes.Persons;
using Ermes.Notifiers;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.IO;
using NpgsqlTypes;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ermes.Attributes;
using Abp.BackgroundJobs;
using Ermes.EventHandlers;
using Ermes.GeoJson;
using NetTopologySuite.Geometries;
using Ermes.Authorization;

namespace Ermes.Communications
{
    [ErmesAuthorize]
    public class CommunicationsAppService : ErmesAppServiceBase, ICommunicationsAppService
    {
        private readonly ErmesAppSession _session;
        private readonly CommunicationManager _communicationManager;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IGeoJsonBulkRepository _geoJsonBulkRepository;
        private readonly ErmesPermissionChecker _permissionChecker;

        public CommunicationsAppService(
                ErmesAppSession session,
                CommunicationManager communicationManager,
                IBackgroundJobManager backgroundJobManager,
                IGeoJsonBulkRepository geoJsonBulkRepository,
                ErmesPermissionChecker permissionChecker
            )
        {
            _session = session;
            _communicationManager = communicationManager;
            _backgroundJobManager = backgroundJobManager;
            _geoJsonBulkRepository = geoJsonBulkRepository;
            _permissionChecker = permissionChecker;
        }

        #region Private
        private async Task<Communication> GetCommunicationAsync(int commId)
        {
            var comm = await _communicationManager.GetCommunicationByIdAsync(commId);
            if (comm == null)
                throw new UserFriendlyException(L("InvalidCommunicationId", commId));

            if (comm.Creator.OrganizationId != _session.LoggedUserPerson.OrganizationId && comm.Creator.Organization.ParentId.HasValue && comm.Creator.Organization.ParentId.Value != _session.LoggedUserPerson.OrganizationId)
                throw new UserFriendlyException(L("EntityOutsideOrganization"));

            return comm;
        }

        private async Task<PagedResultDto<CommunicationDto>> InternalGetCommunications(GetCommunicationsInput input)
        {
            PagedResultDto<CommunicationDto> result = new PagedResultDto<CommunicationDto>();

            IQueryable<Communication> query;
            input.StartDate = input.StartDate.HasValue ? input.StartDate : DateTime.MinValue;
            input.EndDate = input.EndDate.HasValue ? input.EndDate : DateTime.MaxValue;

            if (input.NorthEastBoundary != null && input.SouthWestBoundary != null)
            {
                Geometry boundingBox = GeometryHelper.GetPolygonFromBoundaries(input.SouthWestBoundary, input.NorthEastBoundary);
                query = _geoJsonBulkRepository.GetCommunications(input.StartDate.Value, input.EndDate.Value, boundingBox);
                query = query.Include(a => a.Creator).Include(a => a.Creator.Organization);
            }
            else

                query = _communicationManager.Communications.Where(a => new NpgsqlRange<DateTime>(input.StartDate.Value, input.EndDate.Value).Contains(a.Duration));

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
            result.Items = ObjectMapper.Map<List<CommunicationDto>>(items);

            return result;
        }       

        private async Task<Communication> CreateCommunicationAsync(FeatureDto<CommunicationDto> featureDto)
        {
            var newCommunication = ObjectMapper.Map<Communication>(featureDto.Properties);
            if (featureDto.FullGeometry != null && featureDto.FullGeometry.IsValid)
                newCommunication.AreaOfInterest = featureDto.FullGeometry;
            else
                throw new UserFriendlyException(L("InvalidAOI"));

            newCommunication.Receiver = null;
            newCommunication.Id = await _communicationManager.CreateOrUpdateCommunicationAsync(newCommunication);

            await CurrentUnitOfWork.SaveChangesAsync();
            
            NotificationEvent<CommunicationNotificationDto> notification = new NotificationEvent<CommunicationNotificationDto>(newCommunication.Id,
                _session.UserId.Value,
                ObjectMapper.Map<CommunicationNotificationDto>(newCommunication),
                EntityWriteAction.Create);
            await _backgroundJobManager.EnqueueEventAsync(notification);

            return newCommunication;
        }

        private async Task<Communication> UpdateCommunicationAsync(FeatureDto<CommunicationDto> featureDto)
        {
            var communicationDto = ObjectMapper.Map<CommunicationDto>(featureDto.Properties);
            var comm = await GetCommunicationAsync(communicationDto.Id);
            if (comm == null)
                throw new UserFriendlyException(L("InvalidCommunicationId", communicationDto.Id));
            if (featureDto.FullGeometry != null && featureDto.FullGeometry.IsValid)
                comm.AreaOfInterest = featureDto.FullGeometry;
            else
                throw new UserFriendlyException(L("InvalidAOI"));

            ObjectMapper.Map(communicationDto, comm);

            await CurrentUnitOfWork.SaveChangesAsync();

            NotificationEvent<CommunicationNotificationDto> notification = new NotificationEvent<CommunicationNotificationDto>(comm.Id,
                _session.UserId.Value,
                ObjectMapper.Map<CommunicationNotificationDto>(comm),
                EntityWriteAction.Update);
            await _backgroundJobManager.EnqueueEventAsync(notification);

            return comm;
        }
        #endregion

        [OpenApiOperation("Get Communications",
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
                Output: list of CommunicationDto elements

                N.B.: A person has visibility only on communications belonging to his organization
            "
        )]
        public virtual async Task<DTResult<CommunicationDto>> GetCommunications(GetCommunicationsInput input)
        {
            PagedResultDto<CommunicationDto> result = await InternalGetCommunications(input);
            return new DTResult<CommunicationDto>(input.Draw, result.TotalCount, result.Items.Count, result.Items.ToList());
        }


        [OpenApiOperation("Create or Update a Communication",
            @"
                Input: GeoJson feature, with CommunicationDto element in Properties field
                If the input contains an Id > 0, an update is performed, otherwise a new entity is created
                Output: the Id the communication that has been created/updated
                This operation will trigger notifications. The body of the notification contains the message of the communication
            "
        )]
        public virtual async Task<CreateOrUpdateCommunicationOutput> CreateOrUpdateCommunication(CreateOrUpdateCommunicationInput input)
        {
            Communication communication;
            if (input.Feature.Properties.Id == 0)
                communication = await CreateCommunicationAsync(input.Feature);
            else
                communication = await UpdateCommunicationAsync(input.Feature);

            var writer = new GeoJsonWriter();
            return new CreateOrUpdateCommunicationOutput()
            {
                Feature = new FeatureDto<CommunicationDto>()
                {
                    Geometry = writer.Write(communication.AreaOfInterest),
                    Properties = ObjectMapper.Map<CommunicationDto>(communication)
                }
            };
        }

        [OpenApiOperation("Get Communication by Id",
             @"
                Input: 
                        - Id: the id of the communication to be retrived
                        - IncludeArea: if true, the response will contain the geometry of the communication
                Output: GeoJson feature, with CommunicationDto element in Properties field
                Exception: invalid id of the communication
             "
        )]
        public virtual async Task<GetEntityByIdOutput<CommunicationDto>> GetCommunicationById(GetEntityByIdInput<int> input)
        {
            var comm = await GetCommunicationAsync(input.Id);
            var hasPermission = _permissionChecker.IsGranted(_session.Roles, AppPermissions.Communications.Communication_CanSeeCrossOrganization);
            if (!hasPermission)
            {
                //Father Org can see child contents
                //false the contrary
                if (
                        comm.Creator.OrganizationId != _session.LoggedUserPerson.OrganizationId &&
                        (
                            !comm.Creator.Organization.ParentId.HasValue ||
                            (
                                comm.Creator.Organization.ParentId.HasValue &&
                                comm.Creator.Organization.ParentId.Value != _session.LoggedUserPerson.OrganizationId
                            )
                        )
                )
                    throw new UserFriendlyException(L("EntityOutsideOrganization"));
            }


            var writer = new GeoJsonWriter();
            var res = new GetEntityByIdOutput<CommunicationDto>()
            {
                Feature = new FeatureDto<CommunicationDto>()
                {
                    Geometry = input.IncludeArea ? writer.Write(comm.AreaOfInterest) : null,
                    Properties = ObjectMapper.Map<CommunicationDto>(comm)
                }
            };
            return res;
        }

        [OpenApiOperation("Delete a Communication",
            @"
                Input: the id of the communication to be deleted
                Output: true if the operation has been excuted successfully, false otherwise
                Exception: invalid communication Id 
            "
        )]
        public virtual async Task<bool> DeleteCommunication(IdInput<int> input)
        {
            var comm = await GetCommunicationAsync(input.Id);
            await _communicationManager.DeleteCommunicationAsync(comm);

            return true;
        }
    }
}
