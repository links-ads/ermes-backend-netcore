﻿using Abp.EntityFrameworkCore;
using Ermes.Communications;
using Ermes.Enums;
using Ermes.Missions;
using Ermes.Persons;
using Ermes.ReportRequests;
using Ermes.Reports;
using Ermes.EntityFrameworkCore;
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

        public IQueryable<ReportRequest> GetReportRequests(DateTime startDate, DateTime endDate, Geometry boundingBox)
        {
            return _dbContextProvider.GetDbContext()
                .ReportRequests
                .FromSqlInterpolated($"SELECT * FROM reportrequests WHERE ST_INTERSECTS(\"AreaOfInterest\", {boundingBox}) and tsrange({startDate},{endDate}) && \"Duration\"");
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


        public string GetGeoJsonCollection(DateTime StartDate, DateTime EndDate, Geometry BoundingBox, List<EntityType> entityTypes, int[] organizationIdList, List<ActionStatusType> statusTypes, int[] activityIds, string Language = "it")
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
                        'Mission' as ""type"" ,
                        ST_CENTROID(m.""AreaOfInterest"") as ""location"",
                        m.""CurrentStatus""  as ""status"",
                        o.""Id"" as ""organizationId"",
                        o.""Name"" as ""organizationName"",
                        null as ""extensionData"",
	                    p.""Username"" as ""creator"",
                        null as ""statusFilter"",
                        0 as ""activityFilter""
                    from public.missions m
                    join public.organizations o on o.""Id"" = m.""OrganizationId""
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
                        null as ""extensionData"",
                        p.""Username"" as ""creator"",
                        null as ""statusFilter"",
                        0 as ""activityFilter""
                    from public.communications c
                    join public.persons p on p.""Id"" = c.""CreatorUserId""
                    join public.organizations o on o.""Id"" = p.""OrganizationId""
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
                        null as ""extensionData"",
                        p.""Username"" as ""creator"",
                        null as ""statusFilter"",
                        0 as ""activityFilter""
                    from public.reports r 
                    join public.persons p on p.""Id"" = r.""CreatorUserId""
                    join public.organizations o on o.""Id"" = p.""OrganizationId""
                    union 
                    select 
                        r2.""Id"" as ""id"", 
                        r2.""Title"" as ""details"", 
                        to_char(lower(r2.""Duration""), 'YYYY-MM-DD""T""HH24:MI:SSZ') as ""startDate"", 
                        to_char(upper(r2.""Duration""), 'YYYY-MM-DD""T""HH24:MI:SSZ') as ""endDate"", 
                        lower(r2.""Duration"") as ""startDateFilter"", 
                        upper(r2.""Duration"") as ""endDateFilter"", 
                        'ReportRequest' as ""type"", 
                        ST_CENTROID(r2.""AreaOfInterest"") as ""location"", 
                        null as ""status"",
                        o.""Id"" as ""organizationId"",
                        o.""Name"" as ""organizationName"",
                        null as ""extensionData"",
                        p.""Username"" as ""creator"",
                        null as ""statusFilter"",
                        0 as ""activityFilter""
                    from public.reportrequests r2 
                    join public.persons p on p.""Id"" = r2.""CreatorUserId""
                    join public.organizations o on o.""Id"" = p.""OrganizationId""
                    union
                    (SELECT DISTINCT ON (pa.""PersonId"")
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
	                    (
                            select pa2.""ExtensionData""
                            from public.person_actions pa2
                            where pa2.""Discriminator"" = 'PersonActionTracking' and pa2.""PersonId"" = pa.""PersonId""
                            order by pa2.""Timestamp"" desc
                            limit 1
	                    )::text as ""extensionData"",
	                    p.""Username"" as ""creator"",
                        pa.""CurrentStatus"" as ""statusFilter"",
                        coalesce(a.""ParentId"", a.""Id"") as ""activityFilter""
                    from public.person_actions pa
                    join public.persons p on p.""Id"" = pa.""PersonId""
                    join public.organizations o on o.""Id"" = p.""OrganizationId""
                    left join public.activities a on a.""Id"" = pa.""ActivityId""
                    left join public.activity_translations at2 on at2.""CoreId"" = a.""Id""
                    where(at2.""Language"" = @language or at2.""Language"" is null) and pa.""Discriminator"" != 'PersonActionTracking'
                    order by pa.""PersonId"", pa.""Timestamp"" desc)
                ) tmp 
                where 
                    ST_INTERSECTS(tmp.""location"", @boundingBox) and 
                    tsrange(@startDate, @endDate, '[]') &&
                    tsrange(tmp.""startDateFilter"", tmp.""endDateFilter"", '[]')";


                command.CommandType = CommandType.Text;
                command.Parameters.Add(new NpgsqlParameter("@boundingBox", BoundingBox));
                command.Parameters.Add(new NpgsqlParameter("@startDate", StartDate));
                command.Parameters.Add(new NpgsqlParameter("@endDate", EndDate));
                command.Parameters.Add(new NpgsqlParameter("@language", Language));
                if (entityTypes != null)
                {
                    command.CommandText += @" and tmp.""type"" = any(array[@entities])";
                    var p = new NpgsqlParameter("@entities", NpgsqlDbType.Array | NpgsqlDbType.Text)
                    {
                        Value = entityTypes.Select(a => a.ToString()).ToArray()
                    };
                    command.Parameters.Add(p);
                }

                if (organizationIdList != null)
                {
                    command.CommandText += @" and tmp.""organizationId"" = any(array[@organizations])";
                    var p = new NpgsqlParameter("@organizations", NpgsqlDbType.Array | NpgsqlDbType.Integer)
                    {
                        Value = organizationIdList
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

                using (var result = command.ExecuteReader())
                {
                    if (result.Read())
                        return result.GetString("geojsoncollection");
                    else
                        return null;
                }

            }

        }

        public string GetPersonActions(DateTime startDate, DateTime endDate, int[] organizationIdList, List<ActionStatusType> statusTypes, int[] activityIds, Geometry boundingBox, string search = "", string language = "it")
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
                    SELECT DISTINCT ON (pa.""PersonId"")
                        pa.""Id"" as ""Id"",
                        pa.""DeviceId"",
                        pa.""DeviceName"",
                        ST_Y((pa.""Location"")::geometry) as ""Latitude"",
                        ST_X((pa.""Location"")::geometry) as ""Longitude"",
                        pa.""Location"",
                        pa.""ActivityId"" as ""ActivityId"",
                        at2.""Name"" as ""ActivityName"",
	                    pa.""Timestamp"", 
	                    pa.""CurrentStatus"" as ""Status"", 
	                    o.""Id"" as ""OrganizationId"",
	                    o.""Name"" as ""OrganizationName"",
	                    (
                            select pa2.""ExtensionData""
                            from public.person_actions pa2
                            where pa2.""Discriminator"" = 'PersonActionTracking' and pa2.""PersonId"" = pa.""PersonId""
                            order by pa2.""Timestamp"" desc
                            limit 1
	                    )::text as ""ExtensionData"",
	                    p.""Username"",
                        null as ""type"",
                        coalesce(a.""ParentId"", a.""Id"") as ""activityFilter""
                    from public.person_actions pa
                    join public.persons p on p.""Id"" = pa.""PersonId""
                    join public.organizations o on o.""Id"" = p.""OrganizationId""
                    left join public.activities a on a.""Id"" = pa.""ActivityId""
                    left join public.activity_translations at2 on at2.""CoreId"" = a.""Id""
                    where
                        o.""Id"" = any(array[@organizations]) and
                        (at2.""Language"" = @language or at2.""Language"" is null) and
                        pa.""Discriminator"" != 'PersonActionTracking' and
                        (tsrange(@startDate, @endDate, '[]') &&
                            tsrange(pa.""Timestamp"", pa.""Timestamp"", '[]'))
                    order by pa.""PersonId"", pa.""Timestamp"" desc
                ) as ""tmp""
                where 1=1
                ";

                command.CommandType = CommandType.Text;
                command.Parameters.Add(new NpgsqlParameter("@startDate", startDate));
                command.Parameters.Add(new NpgsqlParameter("@endDate", endDate));
                command.Parameters.Add(new NpgsqlParameter("@language", language));
                command.Parameters.Add(new NpgsqlParameter("@organizations", NpgsqlDbType.Array | NpgsqlDbType.Integer)
                {
                    Value = organizationIdList
                });

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
    }

}
