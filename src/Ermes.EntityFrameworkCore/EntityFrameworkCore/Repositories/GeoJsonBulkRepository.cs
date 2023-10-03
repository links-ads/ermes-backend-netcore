using Abp.EntityFrameworkCore;
using Ermes.Activations;
using Ermes.Alerts;
using Ermes.Communications;
using Ermes.EntityFrameworkCore;
using Ermes.Enums;
using Ermes.MapRequests;
using Ermes.Missions;
using Ermes.Persons;
using Ermes.Reports;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Ermes.GeoJson
{
    public class GeoJsonBulkRepository : IGeoJsonBulkRepository
    {
        IDbContextProvider<ErmesDbContext> _dbContextProvider;
        public GeoJsonBulkRepository(IDbContextProvider<ErmesDbContext> dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }

        public IQueryable<Communication> GetCommunications(DateTime startDate, DateTime endDate, Geometry boundingBox)
        {
            return _dbContextProvider.GetDbContext()
                .Communications
                .FromSqlInterpolated($"SELECT * FROM communications WHERE ST_INTERSECTS(\"AreaOfInterest\", {boundingBox}) and tsrange({startDate},{endDate}) && \"Duration\"");
        }

        public IQueryable<Mission> GetMissions(DateTime startDate, DateTime endDate, Geometry boundingBox)
        {
            return _dbContextProvider.GetDbContext()
                .Missions
                .FromSqlInterpolated($"SELECT * FROM missions WHERE ST_INTERSECTS(\"AreaOfInterest\", {boundingBox}) and tsrange({startDate},{endDate}) && \"Duration\"");
        }

        public IQueryable<Report> GetReports(DateTime startDate, DateTime endDate, Geometry boundingBox)
        {
            return _dbContextProvider.GetDbContext()
                .Reports
                .FromSqlInterpolated($"SELECT * FROM reports WHERE ST_INTERSECTS(\"Location\", {boundingBox}) and {startDate} < \"Timestamp\" and {endDate} > \"Timestamp\"");
        }

        public IQueryable<MapRequest> GetMapRequests(DateTime startDate, DateTime endDate, Geometry boundingBox)
        {
            return _dbContextProvider.GetDbContext()
                .MapRequests
                .FromSqlInterpolated($"SELECT * FROM map_requests WHERE ST_INTERSECTS(\"AreaOfInterest\", {boundingBox}) and tsrange({startDate},{endDate}) && \"Duration\"");
        }

        public IQueryable<PersonActionTracking> GetPersonActionTrackings(DateTime startDate, DateTime endDate, Geometry boundingBox)
        {
            return _dbContextProvider.GetDbContext()
                .PersonActions
                .FromSqlInterpolated($"SELECT * FROM person_actions WHERE ST_INTERSECTS(\"Location\", {boundingBox}) and {startDate} < \"Timestamp\" and {endDate} > \"Timestamp\"")
                .OfType<PersonActionTracking>();
        }

        public IQueryable<PersonActionStatus> GetPersonActionStatuses(DateTime startDate, DateTime endDate, Geometry boundingBox)
        {
            return _dbContextProvider.GetDbContext()
                .PersonActions
                .FromSqlInterpolated($"SELECT * FROM person_actions WHERE ST_INTERSECTS(\"Location\", {boundingBox}) and {startDate} < \"Timestamp\" and {endDate} > \"Timestamp\"")
                .OfType<PersonActionStatus>();
        }

        public IQueryable<PersonActionActivity> GetPersonActionActivities(DateTime startDate, DateTime endDate, Geometry boundingBox)
        {
            return _dbContextProvider.GetDbContext()
                .PersonActions
                .FromSqlInterpolated($"SELECT * FROM person_actions WHERE ST_INTERSECTS(\"Location\", {boundingBox}) and {startDate} < \"Timestamp\" and {endDate} > \"Timestamp\"")
                .OfType<PersonActionActivity>();
        }
        //public IQueryable<PersonAction> GetPersonActions(DateTime startDate, DateTime endDate, Geometry boundingBox)
        //{
        //    return _dbContextProvider.GetDbContext()
        //        .PersonActions
        //        .FromSqlInterpolated($"SELECT * FROM person_actions WHERE ST_INTERSECTS(\"Location\", {boundingBox}) and {startDate} < \"Timestamp\" and {endDate} > \"Timestamp\"")
        //        .OfType<PersonActionActivity>();
        //}


        //The visibility of the different entities is based on the organization of the creator of the entity,
        //made exception for the Communication, where the visibility is defined by the Restriction property
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
            List<CommunicationScopeType> communicationScopeTypes,
            List<string> alertRestrictionTypes,
            int srid,
            string personName,
            int? parentId,
            string Language = "it"
            )
        {
            ErmesDbContext context = _dbContextProvider.GetDbContext();
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = @"
                select json_build_object(
                    'type', 'FeatureCollection',
                    'features', json_agg(ST_AsGeoJSON(tmp.*)::json)
                ) as geojsoncollection
                from (
                    select 
                        m.""Id"" as ""id"" ,
                        m.""Description"" as ""details"",
                        to_char(lower(m.""Duration""), 'YYYY-MM-DD""T""HH24:MI:SSZ') as ""startDate"", 
                        to_char(upper(m.""Duration""), 'YYYY-MM-DD""T""HH24:MI:SSZ') as ""endDate"", 
                        lower(m.""Duration"") as ""startDateFilter"", 
                        upper(m.""Duration"") as ""endDateFilter"", 
                        'Mission' as ""type"",
                        ST_CENTROID(m.""AreaOfInterest"") as ""location"",
                        m.""CurrentStatus""  as ""status"",
                        o.""Id"" as ""organizationId"",
                        o.""Name"" as ""organizationName"",
                        o.""ParentId"" as ""organizationParentId"",
                        null as ""extensionData"",
	                    coalesce(p.""Username"", p.""Email"") as ""creator"",
                        null as ""statusFilter"",
                        0 as ""activityFilter"",
                        null as ""hazardFilter"",
                        null as ""reportStatusFilter"",
                        m.""CurrentStatus"" as ""missionStatusFilter"",
                        null as ""mapRequestStatusFilter"",
                        null as ""mapRequestTypeFilter"",
                        null as ""reportContentTypeFilter"",
                        null as ""reportIsPublicFilter"",
                        null as ""communicationRestrictionFilter"",
                        null as ""communicationScopeFilter"",
                        null as ""alertRestrictionFilter"",
                        0 as ""teamFilter"",
                        null as ""receivers""
                    from public.missions m
                    left join public.organizations o on o.""Id"" = m.""OrganizationId""
                    join public.persons p on p.""Id"" = m.""CreatorUserId""
                    union
                    select 
                        c.""Id"" as ""id"",
                        c.""Message"" as ""details"", 
                        to_char(lower(c.""Duration""), 'YYYY-MM-DD""T""HH24:MI:SSZ') as ""startDate"", 
                        to_char(upper(c.""Duration""), 'YYYY-MM-DD""T""HH24:MI:SSZ') as ""endDate"", 
                        lower(c.""Duration"") as ""startDateFilter"", 
                        upper(c.""Duration"") as ""endDateFilter"", 
                        'Communication' as ""type"", 
                        ST_CENTROID(c.""AreaOfInterest"") as ""location"", 
                        null as ""status"",
                        o.""Id"" as ""organizationId"",
                        o.""Name"" as ""organizationName"",
                        o.""ParentId"" as ""organizationParentId"",
                        null as ""extensionData"",
                        coalesce(p.""Username"", p.""Email"") as ""creator"",
                        null as ""statusFilter"",
                        0 as ""activityFilter"",
                        null as ""hazardFilter"",
                        null as ""reportStatusFilter"",
                        null as ""missionStatusFilter"",
                        null as ""mapRequestStatusFilter"",
                        null as ""mapRequestTypeFilter"",
                        null as ""reportContentTypeFilter"",
                        null as ""reportIsPublicFilter"",
                        c.""Restriction"" as ""communicationRestrictionFilter"",
                        c.""Scope"" as ""communicationScopeFilter"",
                        null as ""alertRestrictionFilter"",
                        0 as ""teamFilter"",
                        (
    	                    select array_agg(cr.""OrganizationId"")
    	                    from public.communication_receivers cr 
    	                    where cr.""CommunicationId"" = c.""Id"" 
                        ) as ""receivers""
                    from public.communications c
                    join public.persons p on p.""Id"" = c.""CreatorUserId""
                    left join public.organizations o on o.""Id"" = p.""OrganizationId""
                    union
                    select 
                        r.""Id"" as ""id"", 
                        r.""Description"" as ""details"", 
                        to_char(r.""Timestamp"", 'YYYY-MM-DD""T""HH24:MI:SSZ') as ""startDate"", 
                        to_char(r.""Timestamp"", 'YYYY-MM-DD""T""HH24:MI:SSZ') as ""endDate"", 
                        r.""Timestamp"" as ""startDateFilter"", 
                        r.""Timestamp"" as ""endDateFilter"",
                        'Report' as ""type"", 
                        r.""Location"" as ""location"",
                        r.""Status"" as ""status"",
                        o.""Id"" as ""organizationId"",
                        o.""Name"" as ""organizationName"",
                        o.""ParentId"" as ""organizationParentId"",
                        null as ""extensionData"",
                        coalesce(p.""Username"", p.""Email"") as ""creator"",
                        null as ""statusFilter"",
                        0 as ""activityFilter"",
                        r.""Hazard"" as ""hazardFilter"",
                        r.""Status"" as ""reportStatusFilter"",
                        null as ""missionStatusFilter"",
                        null as ""mapRequestStatusFilter"",
                        null as ""mapRequestTypeFilter"",
                        r.""ContentType"" as ""reportContentTypeFilter"",
                        r.""IsPublic""::text as ""reportIsPublicFilter"",
                        null as ""communicationRestrictionFilter"",
                        null as ""communicationScopeFilter"",
                        null as ""alertRestrictionFilter"",
                        0 as ""teamFilter"",
                        null as ""receivers""
                    from public.reports r 
                    join public.persons p on p.""Id"" = r.""CreatorUserId""
                    left join public.organizations o on o.""Id"" = p.""OrganizationId""
                    union
                    select 
                        mr.""Id"" as ""id"",
                        mr.""Code"" as ""details"", 
                        to_char(lower(mr.""Duration""), 'YYYY-MM-DD""T""HH24:MI:SSZ') as ""startDate"", 
                        to_char(upper(mr.""Duration""), 'YYYY-MM-DD""T""HH24:MI:SSZ') as ""endDate"", 
                        lower(mr.""Duration"") as ""startDateFilter"", 
                        upper(mr.""Duration"") as ""endDateFilter"", 
                        'MapRequest' as ""type"", 
                        ST_CENTROID(mr.""AreaOfInterest"") as ""location"", 
                        mr.""Status"" as ""status"",
                        o.""Id"" as ""organizationId"",
                        o.""Name"" as ""organizationName"",
                        o.""ParentId"" as ""organizationParentId"",
                        null as ""extensionData"",
                        coalesce(p.""Username"", p.""Email"") as ""creator"",
                        null as ""statusFilter"",
                        0 as ""activityFilter"",
                        null as ""hazardFilter"",
                        null as ""reportStatusFilter"",
                        null as ""missionStatusFilter"",
                        mr.""Status"" as ""mapRequestStatusFilter"",
                        mr.""Type"" as ""mapRequestTypeFilter"",
                        null as ""reportContentTypeFilter"",
                        null as ""reportIsPublicFilter"",
                        null as ""communicationRestrictionFilter"",
                        null as ""communicationScopeFilter"",
                        null as ""alertRestrictionFilter"",
                        0 as ""teamFilter"",
                        null as ""receivers""
                    from public.map_requests mr
                    join public.persons p on p.""Id"" = mr.""CreatorUserId""
                    left join public.organizations o on o.""Id"" = p.""OrganizationId""
                    union
                    SELECT 
                        pa.""Id"" as ""id"",
                        at2.""Name"" as ""details"",
                        to_char(pa.""Timestamp"", 'YYYY-MM-DD""T""HH24:MI:SSZ') as ""startDate"", 
                        to_char(pa.""Timestamp"", 'YYYY-MM-DD""T""HH24:MI:SSZ') as ""endDate"", 
	                    pa.""Timestamp"" as ""startDateFilter"", 
	                    pa.""Timestamp"" as ""endDateFilter"", 
	                    'Person' as ""type"", 
                        pa.""Location"" as ""location"", 
	                    pa.""CurrentStatus"" as ""status"", 
	                    o.""Id"" as ""organizationId"",
	                    o.""Name"" as ""organizationName"",
                        o.""ParentId"" as ""organizationParentId"",
                        pa.""CurrentExtensionData""::text as ""extensionData"",
	                    coalesce(p.""Username"", p.""Email"") as ""creator"",
                        pa.""CurrentStatus"" as ""statusFilter"",
                        coalesce(a.""ParentId"", a.""Id"") as ""activityFilter"",
                        null as ""hazardFilter"",
                        null as ""reportStatusFilter"",
                        null as ""missionStatusFilter"",
                        null as ""mapRequestStatusFilter"",
                        null as ""mapRequestTypeFilter"",
                        null as ""reportContentTypeFilter"",
                        null as ""reportIsPublicFilter"",
                        null as ""communicationRestrictionFilter"",
                        null as ""communicationScopeFilter"",
                        null as ""alertRestrictionFilter"",
                        t.""Id"" as ""TeamId"",
                        null as ""receivers""
                        FROM (
	                        SELECT pa2.""PersonId"", MAX(pa2.""Timestamp"") as ""MaxTime""
                            FROM person_actions pa2
                            GROUP BY pa2.""PersonId""
                        ) r
                    INNER JOIN person_actions pa ON pa.""PersonId"" = r.""PersonId"" and r.""MaxTime"" = pa.""Timestamp""
                    join public.persons p on p.""Id"" = pa.""PersonId""
                    left join public.organizations o on o.""Id"" = p.""OrganizationId""
                    left join public.teams t on t.""Id"" = p.""TeamId""
                    left join public.activities a on a.""Id"" = pa.""CurrentActivityId""
                    left join public.activity_translations at2 on at2.""CoreId"" = pa.""CurrentActivityId""
                    where (at2.""Language"" = @language or at2.""Language"" is null)
                    and (pa.""Location"" is not null and not ST_Equals(pa.""Location""::geometry, st_geomfromtext('POINT(0 0)', @srid)))
                    union
                    select 
                        a.""Id"" as ""id"", 
                        c.""Description"" as ""details"", 
                        to_char(a.""Sent"", 'YYYY-MM-DD""T""HH24:MI:SSZ') as ""startDate"", 
                        to_char(a.""Sent"", 'YYYY-MM-DD""T""HH24:MI:SSZ') as ""endDate"", 
                        a.""Sent"" as ""startDateFilter"", 
                        a.""Sent"" as ""endDateFilter"",
                        'Alert' as ""type"", 
                        ST_CENTROID(a.""BoundingBox"") as ""location"", 
                        a.""Status"" as ""status"",
                        null as ""organizationId"",
                        null as ""organizationName"",
                        null as ""organizationParentId"",
                        null as ""extensionData"",
                        a.""Sender"" as ""creator"",
                        null as ""statusFilter"",
                        0 as ""activityFilter"",
                        null as ""hazardFilter"",
                        null as ""reportStatusFilter"",
                        null as ""missionStatusFilter"",
                        null as ""mapRequestStatusFilter"",
                        null as ""mapRequestTypeFilter"",
                        null as ""reportContentTypeFilter"",
                        null as ""reportIsPublicFilter"",
                        null as ""communicationRestrictionFilter"",
                        null as ""communicationScopeFilter"",
                        a.""Restriction"" as ""alertRestrictionFilter"",
                        0 as ""teamFilter"",
                        null as ""receivers""
                    from public.alerts a 
                    join public.alerts_cap_info c on a.""Id"" = c.""AlertId""
                    union
                        select 
                            s.""Id"" as ""id"", 
                            s.""SensorServiceId"" as ""details"", 
                            to_char(to_timestamp(0), 'YYYY-MM-DD""T""HH24:MI:SSZ') as ""startDate"", 
                            to_char(to_timestamp(10000000000), 'YYYY-MM-DD""T""HH24:MI:SSZ') as ""endDate"", 
                            null as ""startDateFilter"", 
                            null as ""endDateFilter"",
                            'Station' as ""type"", 
                            ST_CENTROID(s.""Location"") as ""location"", 
                            null as ""status"",
                            null as ""organizationId"",
                            null as ""organizationName"",
                            null as ""organizationParentId"",
                            null as ""extensionData"",
                            s.""Owner"" as ""creator"",
                            null as ""statusFilter"",
                            0 as ""activityFilter"",
                            null as ""hazardFilter"",
                            null as ""reportStatusFilter"",
                            null as ""missionStatusFilter"",
                            null as ""mapRequestStatusFilter"",
                            null as ""mapRequestTypeFilter"",
                            null as ""reportContentTypeFilter"",
                            null as ""reportIsPublicFilter"",
                            null as ""communicationRestrictionFilter"",
                            null as ""communicationScopeFilter"",
                            null as ""alertRestrictionFilter"",
                            0 as ""teamFilter"",
                            null as ""receivers""
                        from public.stations s 
                ) tmp 
                where 
                    tsrange(@startDate, @endDate, '[]') &&
                    tsrange(tmp.""startDateFilter"", tmp.""endDateFilter"", '[]')";


                command.CommandType = CommandType.Text;
                //command.Parameters.Add(new NpgsqlParameter("@boundingBox", BoundingBox));
                command.Parameters.Add(new NpgsqlParameter("@startDate", StartDate));
                command.Parameters.Add(new NpgsqlParameter("@endDate", EndDate));
                command.Parameters.Add(new NpgsqlParameter("@language", Language));
                command.Parameters.Add(new NpgsqlParameter("@srid", srid));

                if (BoundingBox != null)
                {
                    command.CommandText += @" and ST_INTERSECTS(tmp.""location"", @boundingBox)";
                    var p = new NpgsqlParameter("@boundingBox", NpgsqlDbType.Geography)
                    {
                        Value = BoundingBox
                    };
                    command.Parameters.Add(p);
                }

                if (entityTypes != null)
                {
                    command.CommandText += @" and tmp.""type"" = any(array[@entities])";
                    var p = new NpgsqlParameter("@entities", NpgsqlDbType.Array | NpgsqlDbType.Text)
                    {
                        Value = entityTypes.Select(a => a.ToString()).ToArray()
                    };
                    command.Parameters.Add(p);
                }

                //Professional cannot see citizens' position
                if (organizationIdList != null)
                {
                    command.CommandText += @" 
                        and (((tmp.""organizationId"" = any(array[@organizations]) or tmp.""organizationParentId"" = any(array[@organizations]) or tmp.""organizationId"" is null) and tmp.""type"" in ('Mission', 'MapRequest', 'Alert', 'Station')) or 
                        ((tmp.""organizationId"" = any(array[@organizations]) or tmp.""organizationParentId"" = any(array[@organizations]) or tmp.""reportIsPublicFilter"" = 'true') and tmp.""type"" = 'Report') or
                        ((tmp.""organizationId"" = any(array[@organizations]) or tmp.""organizationParentId"" = any(array[@organizations])) and tmp.""type"" = 'Person') or
                        (tmp.""type"" = 'Communication' and tmp.""communicationRestrictionFilter"" = any(array[@restrictionTypes]) and (tmp.""communicationRestrictionFilter"" != 'Organization' or array[@organizations] && tmp.""receivers"" ";

                    //(tmp.""type"" = 'Communication' and tmp.""communicationRestrictionFilter"" = any(array[@restrictionTypes]) and (tmp.""communicationRestrictionFilter"" != 'Organization' or tmp.""organizationId"" = any(array[@organizations]) or tmp.""organizationParentId"" = any(array[@organizations])";

                    var p = new NpgsqlParameter("@organizations", NpgsqlDbType.Array | NpgsqlDbType.Integer)
                    {
                        Value = organizationIdList
                    };

                    command.Parameters.Add(p);
                    if (parentId.HasValue)
                    {
                        command.CommandText += @" or tmp.""organizationId"" = @organizationParentId)))";
                        var p2 = new NpgsqlParameter("@organizationParentId", NpgsqlDbType.Integer)
                        {
                            Value = parentId
                        };

                        command.Parameters.Add(p2);
                    }
                    else
                        command.CommandText += ")))";
                }
                else //a citizen can see: his position, public reports and public public Communications
                {
                    command.CommandText += @"
                        and (
                                (tmp.""creator"" = @personName and tmp.""type"" = 'Person') or 
                                (tmp.""reportIsPublicFilter"" = 'true' and tmp.""type"" = 'Report') or
                                (tmp.""type"" = 'Communication' and tmp.""communicationRestrictionFilter"" = any(array[@restrictionTypes]))
                            )";
                    var p = new NpgsqlParameter("@personName", NpgsqlDbType.Text)
                    {
                        Value = personName
                    };
                    command.Parameters.Add(p);
                }

                var parameter = new NpgsqlParameter("@restrictionTypes", NpgsqlDbType.Array | NpgsqlDbType.Text)
                {
                    Value = communicationRestrictionTypes.Select(a => a.ToString()).ToArray()
                };
                command.Parameters.Add(parameter);

                if (communicationScopeTypes != null && communicationScopeTypes.Count > 0)
                {
                    command.CommandText += @" and (tmp.""communicationScopeFilter"" is null or tmp.""communicationScopeFilter"" = any(array[@communicationScopeFilter]))";
                    var p = new NpgsqlParameter("@communicationScopeFilter", NpgsqlDbType.Array | NpgsqlDbType.Text)
                    {
                        Value = communicationScopeTypes.Select(a => a.ToString()).ToArray()
                    };
                    command.Parameters.Add(p);
                }

                if (statusTypes != null && statusTypes.Count > 0)
                {
                    command.CommandText += @" and (tmp.""statusFilter"" is null or tmp.""statusFilter"" = any(array[@statusTypes]))";
                    var p = new NpgsqlParameter("@statusTypes", NpgsqlDbType.Array | NpgsqlDbType.Text)
                    {
                        Value = statusTypes.Select(a => a.ToString()).ToArray()
                    };
                    command.Parameters.Add(p);
                }

                if (activityIds != null && activityIds.Length > 0)
                {
                    command.CommandText += @" and ((tmp.""statusFilter"" is null or tmp.""statusFilter"" != 'Active') or (tmp.""statusFilter"" = 'Active' and (tmp.""activityFilter""= 0 or tmp.""activityFilter"" = any(array[@activityIds]))))";
                    var p = new NpgsqlParameter("@activityIds", NpgsqlDbType.Array | NpgsqlDbType.Integer)
                    {
                        Value = activityIds
                    };
                    command.Parameters.Add(p);
                }

                if (teamIds != null && teamIds.Length > 0)
                {
                    command.CommandText += @" and (tmp.""teamFilter"" = 0 or tmp.""teamFilter"" = any(array[@teamIds]))";
                    var p = new NpgsqlParameter("@teamIds", NpgsqlDbType.Array | NpgsqlDbType.Integer)
                    {
                        Value = teamIds
                    };
                    command.Parameters.Add(p);
                }


                if (hazardTypes != null && hazardTypes.Count > 0)
                {
                    command.CommandText += @" and (tmp.""hazardFilter"" is null or tmp.""hazardFilter"" = any(array[@hazardTypes]))";
                    var p = new NpgsqlParameter("@hazardTypes", NpgsqlDbType.Array | NpgsqlDbType.Text)
                    {
                        Value = hazardTypes.Select(a => a.ToString()).ToArray()
                    };
                    command.Parameters.Add(p);
                }

                if (reportStatusTypes != null && reportStatusTypes.Count > 0)
                {
                    command.CommandText += @" and (tmp.""reportStatusFilter"" is null or tmp.""reportStatusFilter"" = any(array[@reportStatusTypes]))";
                    var p = new NpgsqlParameter("@reportStatusTypes", NpgsqlDbType.Array | NpgsqlDbType.Text)
                    {
                        Value = reportStatusTypes.Select(a => a.ToString()).ToArray()
                    };
                    command.Parameters.Add(p);
                }

                if (missionStatusTypes != null && missionStatusTypes.Count > 0)
                {
                    command.CommandText += @" and (tmp.""missionStatusFilter"" is null or tmp.""missionStatusFilter"" = any(array[@missionStatusTypes]))";
                    var p = new NpgsqlParameter("@missionStatusTypes", NpgsqlDbType.Array | NpgsqlDbType.Text)
                    {
                        Value = missionStatusTypes.Select(a => a.ToString()).ToArray()
                    };
                    command.Parameters.Add(p);
                }

                if (mapRequestTypes != null && mapRequestTypes.Count > 0)
                {
                    command.CommandText += @" and (tmp.""mapRequestTypeFilter"" is null or tmp.""mapRequestTypeFilter"" = any(array[@mapRequestTypes]))";
                    var p = new NpgsqlParameter("@mapRequestTypes", NpgsqlDbType.Array | NpgsqlDbType.Text)
                    {
                        Value = mapRequestTypes.Select(a => a.ToString()).ToArray()
                    };
                    command.Parameters.Add(p);
                }

                if (mapRequestStatusTypes != null && mapRequestStatusTypes.Count > 0)
                {
                    command.CommandText += @" and (tmp.""mapRequestStatusFilter"" is null or tmp.""mapRequestStatusFilter"" = any(array[@mapRequestStatusTypes]))";
                    var p = new NpgsqlParameter("@mapRequestStatusTypes", NpgsqlDbType.Array | NpgsqlDbType.Text)
                    {
                        Value = mapRequestStatusTypes.Select(a => a.ToString()).ToArray()
                    };
                    command.Parameters.Add(p);
                }

                if (reportContentTypes != null && reportContentTypes.Count > 0)
                {
                    command.CommandText += @" and (tmp.""reportContentTypeFilter"" is null or tmp.""reportContentTypeFilter"" = any(array[@reportContentTypes]))";
                    var p = new NpgsqlParameter("@reportContentTypes", NpgsqlDbType.Array | NpgsqlDbType.Text)
                    {
                        Value = reportContentTypes.Select(a => a.ToString()).ToArray()
                    };
                    command.Parameters.Add(p);
                }

                if (alertRestrictionTypes != null && alertRestrictionTypes.Count > 0)
                {
                    command.CommandText += @" and (tmp.""alertRestrictionFilter"" is null or tmp.""alertRestrictionFilter"" = any(array[@alertRestrictionTypes]))";
                    var p = new NpgsqlParameter("@alertRestrictionTypes", NpgsqlDbType.Array | NpgsqlDbType.Text)
                    {
                        Value = alertRestrictionTypes.ToArray()
                    };
                    command.Parameters.Add(p);
                }


                //Postgre throws error if we try to use the param as a boolean.
                //The explicit cast to text here is needed
                if (visibilityType != VisibilityType.All)
                {
                    var param = (visibilityType == VisibilityType.Public).ToString().ToLower();
                    command.CommandText += @" and (tmp.""reportIsPublicFilter"" is null or tmp.""reportIsPublicFilter"" = @reportVisibility)";
                    var p = new NpgsqlParameter("@reportVisibility", NpgsqlDbType.Text)
                    {
                        Value = param
                    };
                    command.Parameters.Add(p);
                }

                using (var result = command.ExecuteReader())
                {
                    if (result.Read())
                        return result.GetString("geojsoncollection");
                    else
                        return null;
                }
            }
        }

        public string GetPersonActions(
            DateTime startDate,
            DateTime endDate,
            int[] organizationIdList,
            List<ActionStatusType> statusTypes,
            int[] activityIds,
            int[] teamIds,
            Geometry boundingBox,
            string personName,
            string search = "",
            string language = "it"
        )
        {
            var result = new List<object>();
            ErmesDbContext context = _dbContextProvider.GetDbContext();
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = @"
                select json_build_object(
                    'PersonActions', json_agg(tmp.*)
                ) as collection
                from (
                    SELECT pa.""Id"",
                    pa.""DeviceId"",
                    pa.""DeviceName"",
                    ST_Y((pa.""Location"")::geometry) as ""Latitude"",
                    ST_X((pa.""Location"")::geometry) as ""Longitude"",
                    pa.""CurrentExtensionData""::text as ""ExtensionData"",
                    pa.""Location"",
                    pa.""CurrentActivityId"" as ""ActivityId"",
                    at2.""Name"" as ""ActivityName"",
	                pa.""Timestamp"", 
	                pa.""CurrentStatus"" as ""Status"",
	                o.""Id"" as ""OrganizationId"",
	                o.""Name"" as ""OrganizationName"",
                    o.""ParentId"" as ""OrganizationParentId"",
                    t.""Id"" as ""TeamId"",
                    t.""Name"" as ""TeamName"",
	                coalesce(p.""Username"", p.""Email"") as ""Username"",
                    null as ""Type"",
                    coalesce(a.""ParentId"", a.""Id"") as ""activityFilter""
                    FROM (
	                    SELECT pa2.""PersonId"", MAX(pa2.""Timestamp"") as ""MaxTime""
                        FROM person_actions pa2
                        GROUP BY pa2.""PersonId""
                    ) r
                    INNER JOIN person_actions pa ON pa.""PersonId"" = r.""PersonId"" and r.""MaxTime"" = pa.""Timestamp""
                    join public.persons p on p.""Id"" = pa.""PersonId""
                    left join public.organizations o on o.""Id"" = p.""OrganizationId""
                    left join public.teams t on t.""Id"" = p.""TeamId""
                    left join public.activities a on a.""Id"" = pa.""CurrentActivityId""
                    left join public.activity_translations at2 on at2.""CoreId"" = pa.""CurrentActivityId""
                    where (at2.""Language"" = @language or at2.""Language"" is null)
                ) as ""tmp""
                where (tmp.""Timestamp"" >= @startDate and tmp.""Timestamp"" <= @endDate)
                ";

                command.CommandType = CommandType.Text;
                command.Parameters.Add(new NpgsqlParameter("@startDate", startDate));
                command.Parameters.Add(new NpgsqlParameter("@endDate", endDate));
                command.Parameters.Add(new NpgsqlParameter("@language", language));

                //Professional cannot see citizens' position
                if (organizationIdList != null)
                {
                    command.CommandText += @" and (tmp.""OrganizationId"" = any(array[@organizations]) or tmp.""OrganizationParentId"" = any(array[@organizations]))";
                    var p = new NpgsqlParameter("@organizations", NpgsqlDbType.Array | NpgsqlDbType.Integer)
                    {
                        Value = organizationIdList
                    };
                    command.Parameters.Add(p);
                }
                else //citizen can see himself
                {
                    command.CommandText += @" and tmp.""Username"" = @personName";
                    var p = new NpgsqlParameter("@personName", NpgsqlDbType.Text)
                    {
                        Value = personName
                    };
                    command.Parameters.Add(p);
                }

                if (boundingBox != null)
                {
                    command.CommandText += @" and ST_INTERSECTS(tmp.""Location"", @boundingBox)";
                    var p = new NpgsqlParameter("@boundingBox", NpgsqlDbType.Geography)
                    {
                        Value = boundingBox
                    };
                    command.Parameters.Add(p);
                }

                if (statusTypes != null && statusTypes.Count > 0)
                {
                    command.CommandText += @" and (tmp.""Status"" is null or tmp.""Status"" = any(array[@statusTypes]))";
                    var p = new NpgsqlParameter("@statusTypes", NpgsqlDbType.Array | NpgsqlDbType.Text)
                    {
                        Value = statusTypes.Select(a => a.ToString()).ToArray()
                    };
                    command.Parameters.Add(p);
                }

                if (activityIds != null && activityIds.Length > 0)
                {
                    command.CommandText += @" and ((tmp.""Status"" is null or tmp.""Status"" != 'Active') or (tmp.""Status"" = 'Active' and (tmp.""activityFilter""= 0 or tmp.""activityFilter"" = any(array[@activityIds]))))";
                    var p = new NpgsqlParameter("@activityIds", NpgsqlDbType.Array | NpgsqlDbType.Integer)
                    {
                        Value = activityIds
                    };
                    command.Parameters.Add(p);
                }

                if (teamIds != null && teamIds.Length > 0)
                {
                    command.CommandText += @" and tmp.""TeamId"" = any(array[@teamIds])";
                    var p = new NpgsqlParameter("@teamIds", NpgsqlDbType.Array | NpgsqlDbType.Integer)
                    {
                        Value = teamIds
                    };
                    command.Parameters.Add(p);
                }

                if (search != null && search != "")
                {
                    search = "%" + search + "%";
                    command.CommandText += @" and tmp.""Username"" like @search ";
                    var p = new NpgsqlParameter("@search", NpgsqlDbType.Text)
                    {
                        Value = search
                    };
                    command.Parameters.Add(p);
                }

                using (var rows = command.ExecuteReader())
                {
                    if (rows.Read())
                    {
                        var res = rows.GetString("collection");
                        return res;
                    }
                    else
                        return null;


                }
            }
        }

        //Extended version to retrieve the receivers of a communication
        public string GetPersonActions(
            DateTime startDate,
            DateTime endDate,
            int[] organizationIdList,
            List<ActionStatusType> statusTypes,
            int[] activityIds,
            Geometry boundingBox,
            string search = "",
            string language = "it",
            CommunicationScopeType scopeType = CommunicationScopeType.Restricted,
            CommunicationRestrictionType restrictionType = CommunicationRestrictionType.Organization
        )
        {
            var result = new List<object>();
            ErmesDbContext context = _dbContextProvider.GetDbContext();
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = @"
                select json_build_object(
                    'PersonActions', json_agg(tmp.*)
                ) as collection
                from (
                    SELECT pa.""Id"",
                    pa.""DeviceId"",
                    pa.""DeviceName"",
                    ST_Y((pa.""Location"")::geometry) as ""Latitude"",
                    ST_X((pa.""Location"")::geometry) as ""Longitude"",
                    pa.""CurrentExtensionData""::text as ""ExtensionData"",
                    pa.""Location"",
                    pa.""CurrentActivityId"" as ""ActivityId"",
                    at2.""Name"" as ""ActivityName"",
	                pa.""Timestamp"", 
	                pa.""CurrentStatus"" as ""Status"",
	                o.""Id"" as ""OrganizationId"",
	                o.""Name"" as ""OrganizationName"",
                    o.""ParentId"" as ""OrganizationParentId"",
	                p.""Username"",
                    p.""Id"" as ""PersonId"",
                    p.""TeamId"",
                    null as ""Type"",
                    coalesce(a.""ParentId"", a.""Id"") as ""activityFilter"",
                    p.""Email""
                    FROM (
	                    SELECT pa2.""PersonId"", MAX(pa2.""Timestamp"") as ""MaxTime""
                        FROM person_actions pa2
                        GROUP BY pa2.""PersonId""
                    ) r
                    INNER JOIN person_actions pa ON pa.""PersonId"" = r.""PersonId"" and r.""MaxTime"" = pa.""Timestamp""
                    join public.persons p on p.""Id"" = pa.""PersonId""
                    left join public.organizations o on o.""Id"" = p.""OrganizationId""
                    left join public.activities a on a.""Id"" = pa.""CurrentActivityId""
                    left join public.activity_translations at2 on at2.""CoreId"" = pa.""CurrentActivityId""
                    where (at2.""Language"" = @language or at2.""Language"" is null)
                ) as ""tmp""
                where (tmp.""Timestamp"" >= @startDate and tmp.""Timestamp"" <= @endDate)
                ";

                command.CommandType = CommandType.Text;
                command.Parameters.Add(new NpgsqlParameter("@startDate", startDate));
                command.Parameters.Add(new NpgsqlParameter("@endDate", endDate));
                command.Parameters.Add(new NpgsqlParameter("@language", language));
                if (scopeType == CommunicationScopeType.Restricted)
                {
                    switch (restrictionType)
                    {
                        case CommunicationRestrictionType.Professional:
                            command.CommandText += @" and tmp.""OrganizationId"" is not null";
                            break;
                        case CommunicationRestrictionType.Citizen:
                            command.CommandText += @" and tmp.""OrganizationId"" is null";
                            break;
                        case CommunicationRestrictionType.Organization:
                        default:
                            command.CommandText += @" and (tmp.""OrganizationId"" = any(array[@organizations]) or tmp.""OrganizationParentId"" = any(array[@organizations]))";
                            var p = new NpgsqlParameter("@organizations", NpgsqlDbType.Array | NpgsqlDbType.Integer)
                            {
                                Value = organizationIdList
                            };
                            command.Parameters.Add(p);
                            break;
                    }
                }

                if (boundingBox != null)
                {
                    command.CommandText += @" and ST_INTERSECTS(tmp.""Location"", @boundingBox)";
                    var p = new NpgsqlParameter("@boundingBox", NpgsqlDbType.Geography)
                    {
                        Value = boundingBox
                    };
                    command.Parameters.Add(p);
                }

                if (statusTypes != null && statusTypes.Count > 0)
                {
                    command.CommandText += @" and (tmp.""Status"" is null or tmp.""Status"" = any(array[@statusTypes]))";
                    var p = new NpgsqlParameter("@statusTypes", NpgsqlDbType.Array | NpgsqlDbType.Text)
                    {
                        Value = statusTypes.Select(a => a.ToString()).ToArray()
                    };
                    command.Parameters.Add(p);
                }

                if (activityIds != null && activityIds.Length > 0)
                {
                    command.CommandText += @" and ((tmp.""Status"" is null or tmp.""Status"" != 'Active') or (tmp.""Status"" = 'Active' and (tmp.""activityFilter""= 0 or tmp.""activityFilter"" = any(array[@activityIds]))))";
                    var p = new NpgsqlParameter("@activityIds", NpgsqlDbType.Array | NpgsqlDbType.Integer)
                    {
                        Value = activityIds
                    };
                    command.Parameters.Add(p);
                }

                if (search != null && search != "")
                {
                    search = "%" + search + "%";
                    command.CommandText += @" and tmp.""Username"" like @search ";
                    var p = new NpgsqlParameter("@search", NpgsqlDbType.Text)
                    {
                        Value = search
                    };
                    command.Parameters.Add(p);
                }

                using (var rows = command.ExecuteReader())
                {
                    if (rows.Read())
                    {
                        var res = rows.GetString("collection");
                        return res;
                    }
                    else
                        return null;


                }
            }
        }

        public List<Activation> GetPersonActivations(DateTime startDate, DateTime endDate, ActionStatusType statusType)
        {
            var result = new List<Activation>();
            ErmesDbContext context = _dbContextProvider.GetDbContext();
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = @"
                select ""Timestamp""::date, array_agg(distinct ""PersonId"") as ""PersonIdList"", count(distinct(""PersonId"")) as ""Count""
                from public.person_actions
                where ""CurrentStatus"" = @statusType and ""Timestamp"" >= @startDate and ""Timestamp"" <= @endDate
                GROUP BY ""Timestamp""::date
                ORDER BY ""Timestamp""::date
                ";

                command.CommandType = CommandType.Text;
                command.Parameters.Add(new NpgsqlParameter("@startDate", startDate));
                command.Parameters.Add(new NpgsqlParameter("@endDate", endDate));
                command.Parameters.Add(new NpgsqlParameter("@statusType", statusType.ToString()));
                DateTime refDate = startDate.Date;
                using (var reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        var timestamp = reader.GetDateTime("Timestamp");
                        var count = reader.GetInt32("Count");
                        var personIdList = reader.GetFieldValue<long[]>("PersonIdList");


                        while (refDate != timestamp)
                        {
                            result.Add(new Activation
                            {
                                Timestamp = refDate
                            });
                            refDate = refDate.AddDays(1);
                        }


                        result.Add(new Activation
                        {
                            Timestamp = refDate,
                            Counter = count,
                            PersonIdList = personIdList
                        });
                        refDate = refDate.AddDays(1);
                    }

                    while (refDate < endDate)
                    {
                        result.Add(new Activation
                        {
                            Timestamp = refDate
                        });
                        refDate = refDate.AddDays(1);
                    }
                }

                return result;
            }
        }

        public IQueryable<Alert> GetAlerts(DateTime startDate, DateTime endDate, Geometry boundingBox)
        {
            return _dbContextProvider.GetDbContext()
                .Alerts
                .FromSqlInterpolated($"SELECT * FROM alerts WHERE ST_INTERSECTS(\"BoundingBox\", {boundingBox}) and {startDate} < \"Sent\" and {endDate} > \"Sent\"");
        }
    }

}