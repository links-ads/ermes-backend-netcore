using Abp.Domain.Repositories;
using Ermes.Communications;
using Ermes.Enums;
using Ermes.Missions;
using Ermes.Persons;
using Ermes.ReportRequests;
using Ermes.Reports;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.GeoJson
{
    public interface IGeoJsonBulkRepository : IRepository
    {
        public IQueryable<Communication> GetCommunications(DateTime startDate, DateTime endDate, Geometry boundingBox);
        public IQueryable<Mission> GetMissions(DateTime startDate, DateTime endDate, Geometry boundingBox);
        public IQueryable<Report> GetReports(DateTime startDate, DateTime endDate, Geometry boundingBox);
        public IQueryable<ReportRequest> GetReportRequests(DateTime startDate, DateTime endDate, Geometry boundingBox);
        public IQueryable<PersonActionTracking> GetPersonActionTrackings(DateTime startDate, DateTime endDate, Geometry boundingBox);
        public IQueryable<PersonActionStatus> GetPersonActionStatuses(DateTime startDate, DateTime endDate, Geometry boundingBox);
        public IQueryable<PersonActionActivity> GetPersonActionActivities(DateTime startDate, DateTime endDate, Geometry boundingBox);
        //public IQueryable<PersonAction> GetPersonActions(DateTime startDate, DateTime endDate, Geometry boundingBox);
        public string GetPersonActions(DateTime StartDate, DateTime EndDate, int[] organizationIdList, List<ActionStatusType> statusTypes, int[] activityIds, Geometry boundingBox, string search = "", string language = "it");
        public string GetGeoJsonCollection(DateTime StartDate, DateTime EndDate, Geometry BoundingBox, List<EntityType> entityTypes, int[] organizationIdList, List<ActionStatusType> statusTypes, int[] activityIds, int srid, string language = "it");

    }
}
