using Abp.Domain.Repositories;
using Ermes.Activations;
using Ermes.Communications;
using Ermes.Enums;
using Ermes.MapRequests;
using Ermes.Missions;
using Ermes.Persons;
using Ermes.Reports;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ermes.GeoJson
{
    public interface IGeoJsonBulkRepository : IRepository
    {
        public IQueryable<Communication> GetCommunications(DateTime startDate, DateTime endDate, Geometry boundingBox);
        public IQueryable<Mission> GetMissions(DateTime startDate, DateTime endDate, Geometry boundingBox);
        public IQueryable<Report> GetReports(DateTime startDate, DateTime endDate, Geometry boundingBox);
        public IQueryable<MapRequest> GetMapRequests(DateTime startDate, DateTime endDate, Geometry boundingBox);
        public IQueryable<PersonActionTracking> GetPersonActionTrackings(DateTime startDate, DateTime endDate, Geometry boundingBox);
        public IQueryable<PersonActionStatus> GetPersonActionStatuses(DateTime startDate, DateTime endDate, Geometry boundingBox);
        public IQueryable<PersonActionActivity> GetPersonActionActivities(DateTime startDate, DateTime endDate, Geometry boundingBox);
        //public IQueryable<PersonAction> GetPersonActions(DateTime startDate, DateTime endDate, Geometry boundingBox);
        public string GetPersonActions(
            DateTime StartDate,
            DateTime EndDate,
            int[] organizationIdList,
            List<ActionStatusType> statusTypes,
            int[] activityIds,
            int[] teamIds,
            Geometry boundingBox,
            string personName,
            string search = "",
            string language = "it"
        );

        public string GetPersonActions(
            DateTime StartDate,
            DateTime EndDate,
            int[] organizationIdList,
            List<ActionStatusType> statusTypes,
            int[] activityIds,
            Geometry boundingBox,
            string search = "",
            string language = "it",
            CommunicationScopeType scopeType = CommunicationScopeType.Restricted,
            CommunicationRestrictionType restrictionType = CommunicationRestrictionType.Organization
        );


        public string GetGeoJsonCollection(
            DateTime StartDate,
            DateTime EndDate,
            Geometry BoundingBox,
            List<EntityType> entityTypes,
            int[] organizationIdList,
            List<ActionStatusType> statusTypes,
            int[] activityIds,
            int[] teamIds,
            List<HazardType> hazardTypes,
            List<GeneralStatus> reportStatusTypes,
            List<MissionStatusType> missionStatusTypes,
            List<MapRequestStatusType> mapRequestStatusTypes,
            List<MapRequestType> mapRequestTypes,
            VisibilityType visibilityType,
            List<ReportContentType> reportContentTypes,
            List<CommunicationRestrictionType> communicationRestrictionTypes,
            int srid,
            string personName,
            int? organizationParentId,
            string language = "it"
        );

        public List<Activation> GetPersonActivations(DateTime StartDate, DateTime EndDate, ActionStatusType statusType);
    }
}
