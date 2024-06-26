﻿using Abp.AutoMapper;
using Abp.SensorService.Model;
using Abp.SocialMedia.Dto;
using Abp.SocialMedia.Model;
using AutoMapper;
using Ermes.Actions.Dto;
using Ermes.Activations;
using Ermes.Activities;
using Ermes.Activities.Dto;
using Ermes.Alerts;
using Ermes.Alerts.Dto;
using Ermes.Answers;
using Ermes.Auth.Dto;
using Ermes.Categories;
using Ermes.Communications;
using Ermes.Communications.Dto;
using Ermes.Consumers.RabbitMq;
using Ermes.Dashboard.Dto;
using Ermes.Dto.Spatial;
using Ermes.EntityHistory;
using Ermes.Enums;
using Ermes.Gamification;
using Ermes.Gamification.Dto;
using Ermes.GeoJson.Dto;
using Ermes.Helpers;
using Ermes.Import.Dto;
using Ermes.Layers;
using Ermes.Layers.Dto;
using Ermes.Logging.Dto;
using Ermes.MapRequests;
using Ermes.MapRequests.Dto;
using Ermes.Missions;
using Ermes.Missions.Dto;
using Ermes.Notifications;
using Ermes.Notifications.Dto;
using Ermes.Notifiers.MessageBody;
using Ermes.Organizations;
using Ermes.Organizations.Dto;
using Ermes.Permissions;
using Ermes.Persons;
using Ermes.Preferences;
using Ermes.Profile.Dto;
using Ermes.Quizzes;
using Ermes.Reports;
using Ermes.Reports.Dto;
using Ermes.Resources;
using Ermes.Roles;
using Ermes.Roles.Dto;
using Ermes.Social.Dto;
using Ermes.Stations;
using Ermes.Stations.Dto;
using Ermes.Teams;
using Ermes.Teams.Dto;
using Ermes.Tips;
using Ermes.Users.Dto;
using io.fusionauth.domain;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NpgsqlTypes;
using System;
using System.Linq;

namespace Ermes
{
    internal static class CustomDtoMapper
    {
        public static void CreateMappings(IMapperConfigurationExpression configuration, MultiLingualMapContext context)
        {
            configuration.CreateMultiLingualMap<Activity, ActivityTranslation, ActivityDto>(context);
            configuration.CreateMultiLingualMap<Activity, ActivityTranslation, LocalizedActivityNameDto>(context);
            configuration.CreateMultiLingualMap<Category, CategoryTranslation, LocalizedCategoryValuesDto>(context);
            configuration.CreateMultiLingualMap<Tip, TipTranslation, TipDto>(context);
            configuration.CreateMultiLingualMap<Quiz, QuizTranslation, QuizDto>(context);
            configuration.CreateMultiLingualMap<Answer, AnswerTranslation, AnswerDto>(context);
            configuration.CreateMultiLingualMap<Layer, LayerTranslation, LayerDto>(context);
            configuration.CreateMap<User, UserDto>()
                            .ReverseMap()
                            .ForMember(entity => entity.passwordChangeRequired, options => options.MapFrom(dto => false))
                            .ForMember(entity => entity.active, options => options.MapFrom(dto => true));
            configuration.CreateMap<UserRegistration, UserRegistrationDto>().ReverseMap();
            configuration.CreateMap<Organization, OrganizationDto>()
                .ForMember(dto => dto.ParentId, options => options.MapFrom(entity => entity.ParentId))
                .ForMember(dto => dto.ParentName, options => options.MapFrom(entity => entity.ParentId.HasValue ? entity.Parent.Name : string.Empty))
                .ReverseMap()
                .ForMember(entity => entity.Parent, options => options.Ignore());
            configuration.CreateMap<Role, RoleDto>().ReverseMap();
            configuration.CreateMap<ErmesPermission, ErmesPermissionDto>().ReverseMap();
            configuration.CreateMap<Team, TeamDto>().ReverseMap();
            configuration.CreateMap<Team, TeamOutputDto>().ReverseMap();
            configuration.CreateMap<Person, ListUsernamesDto>().ReverseMap();
            configuration.CreateMap<SplitEntityChangeSet, ChangeAuthorDto>();
            configuration.CreateMap<SplitEntityPropertyChange, ChangeDto>();
            configuration.CreateMap<SplitEntityChange, ChangeInfoDto>();

            configuration.CreateMultiLingualMap<Category, CategoryTranslation, CategoryDto>(context)
                            .EntityMap.ForMember(dto => dto.CategoryId, options => options.MapFrom(a => a.Id));
            configuration.CreateMap<Mission, MissionDto>()
                            .ForMember(dto => dto.Centroid, options => options.MapFrom(b => new PointPosition(b.AreaOfInterest.Centroid.X, b.AreaOfInterest.Centroid.Y)))
                            .ForMember(dto => dto.Organization, options => options.MapFrom(b => b.Organization))
                            .AfterMap((src, dest) => dest.Duration.LowerBound = dest.Duration.LowerBound.ToUniversalTime())
                            .AfterMap((src, dest) => dest.Duration.UpperBound = dest.Duration.UpperBound.ToUniversalTime())
                            .ReverseMap()
                            .ForMember(entity => entity.CreatorPerson, options => options.Ignore())
                            .ForMember(entity => entity.Organization, options => options.Ignore());
            //.ForMember(entity => entity.OrganizationId, options => options.Ignore());
            configuration.CreateMap<Mission, MissionNotificationDto>()
                            .ForMember(dto => dto.Centroid, options => options.MapFrom(b => new PointPosition(b.AreaOfInterest.Centroid.X, b.AreaOfInterest.Centroid.Y)))
                            .ForMember(dto => dto.Status, options => options.MapFrom(b => b.CurrentStatus))
                            .ForMember(dto => dto.OrganizationId, options => options.MapFrom(b => b.OrganizationId))
                            .AfterMap((src, dest) => dest.Duration.LowerBound = dest.Duration.LowerBound.ToUniversalTime())
                            .AfterMap((src, dest) => dest.Duration.UpperBound = dest.Duration.UpperBound.ToUniversalTime());
            configuration.CreateMap<Mission, FullMissionDto>()
                            .ReverseMap();
            Func<string, int, MediaURIDto> MediaURIMapper = ((fname, reportid) =>
            {
                MediaURIDto m = new MediaURIDto
                {
                    MediaType = ErmesCommon.GetMediaTypeFromFilename(fname),
                    MediaURI = ResourceManager.Reports.GetMediaPath(reportid, fname)
                };
                if (m.MediaType == MediaType.Image)
                    m.ThumbnailURI = ResourceManager.Thumbnails.GetMediaPath(reportid, fname);
                return m;
            });
            configuration.CreateMap<Report, ReportDto>()
                            .ForMember(dto => dto.Location, options => options.MapFrom(a => new PointPosition(a.Location.X, a.Location.Y)))
                            .ForMember(dto => dto.Timestamp, options => options.MapFrom(a => a.Timestamp.ToUniversalTime()))
                            .ForMember(dto => dto.OrganizationName, options => options.MapFrom(a => a.Creator.OrganizationId.HasValue ? a.Creator.Organization.Name : null))
                            .ForMember(dto => dto.Username, options => options.MapFrom(a => a.CreatorUserId.HasValue ? a.Creator.Username : null))
                            .ForMember(dto => dto.Email, options => options.MapFrom(a => a.CreatorUserId.HasValue ? a.Creator.Email : null))
                            .ForMember(dto => dto.Upvotes, options => options.MapFrom(a => a.Validations != null && a.Validations.Count > 0 ? a.Validations.Count(a => a.IsValid) : 0))
                            .ForMember(dto => dto.Downvotes, options => options.MapFrom(a => a.Validations != null && a.Validations.Count > 0 ? a.Validations.Count(a => !a.IsValid) : 0))
                            .ForMember(dto => dto.MediaURIs, options => options.MapFrom(a => a.MediaURIs.Select(s => MediaURIMapper(s, a.Id)).OrderByDescending(a => a.MediaType == MediaType.Image).ToList()))
                            .ReverseMap()
                            .ForMember(entity => entity.Location, options => options.MapFrom(a => new Point(a.Location.Longitude, a.Location.Latitude)))
                            .ForMember(entity => entity.Timestamp, options => options.Ignore())
                            .ForMember(entity => entity.MediaURIs, options => options.Ignore())
                            .ForMember(entity => entity.Tags, options => options.Ignore())
                            .ForMember(entity => entity.AdultInfo, options => options.Ignore())
                            .ForMember(entity => entity.Creator, options => options.Ignore())
                            .ForMember(entity => entity.Validations, options => options.Ignore());
            configuration.CreateMap<ReportValidation, ReportValidationDto>()
                            .ForMember(dto => dto.ValidatorDisplayName, options => options.MapFrom(a => a.Person.Username != null && a.Person.Username != string.Empty ? a.Person.Username : a.Person.Email))
                            .ReverseMap();
            configuration.CreateMap<Report, ReportNotificationDto>()
                            .ForMember(dto => dto.Location, options => options.MapFrom(a => new PointPosition(a.Location.X, a.Location.Y)))
                            .ForMember(dto => dto.Timestamp, options => options.MapFrom(a => a.Timestamp.ToUniversalTime()))
                            .ForMember(dto => dto.OrganizationId, options => options.MapFrom(a => a.Creator.OrganizationId))
                            .ForMember(dto => dto.OrganizationName, options => options.MapFrom(a => a.Creator.OrganizationId.HasValue ? a.Creator.Organization.Name : null))
                            .ForMember(dto => dto.Username, options => options.MapFrom(a => a.CreatorUserId.HasValue ? a.Creator.Username : null))
                            .ForMember(dto => dto.Email, options => options.MapFrom(a => a.CreatorUserId.HasValue ? a.Creator.Email : null))
                            .AfterMap((src, dest) => dest.MediaURIs = dest.MediaURIs.Select(a => ResourceManager.Reports.GetMediaPath(dest.Id, a)).ToList());
            configuration.CreateMap<Communication, CommunicationDto>()
                            .ForMember(dto => dto.Centroid, options => options.MapFrom(b => new PointPosition(b.AreaOfInterest.Centroid.X, b.AreaOfInterest.Centroid.Y)))
                            .ForMember(dto => dto.OrganizationName, options => options.MapFrom(a => a.Creator.OrganizationId.HasValue ? a.Creator.Organization.Name : null))
                            .ForMember(dto => dto.OrganizationReceiverIds, options => options.MapFrom(a => a.CommunicationReceivers != null ? a.CommunicationReceivers.Select(b => b.OrganizationId).ToList() : null))
                            .AfterMap((src, dest) => dest.Duration.LowerBound = dest.Duration.LowerBound.ToUniversalTime())
                            .AfterMap((src, dest) => dest.Duration.UpperBound = dest.Duration.UpperBound.ToUniversalTime())
                            .ReverseMap()
                            .ForMember(entity => entity.Creator, options => options.Ignore());
            configuration.CreateMap<Communication, CommunicationNotificationDto>()
                            .ForMember(dto => dto.Centroid, options => options.MapFrom(b => new PointPosition(b.AreaOfInterest.Centroid.X, b.AreaOfInterest.Centroid.Y)))
                            .ForMember(dto => dto.OrganizationReceiverIds, options => options.MapFrom(a => a.CommunicationReceivers != null ? a.CommunicationReceivers.Select(b => b.OrganizationId).ToList() : null))
                            .AfterMap((src, dest) => dest.Duration.LowerBound = dest.Duration.LowerBound.ToUniversalTime())
                            .AfterMap((src, dest) => dest.Duration.UpperBound = dest.Duration.UpperBound.ToUniversalTime());
            configuration.CreateMap<Notification, NotificationDto>()
                            .ForMember(dto => dto.Timestamp, options => options.MapFrom(b => b.Timestamp.ToUniversalTime()))
                            .ReverseMap()
                            .ForMember(entity => entity.Creator, options => options.Ignore());
            configuration.CreateMap<Preference, PreferenceDto>()
                            .ReverseMap();
            configuration.CreateMap<ApplicationRole, Role>()
                            .ForMember(r => r.Id, options => options.Ignore())
                            .ForMember(r => r.Default, options => options.MapFrom(ar => ar.isDefault))
                            .ForMember(r => r.SuperRole, options => options.MapFrom(ar => ar.isSuperRole));
            ////////////////
            ///Social Module
            configuration.CreateMap<AnnotationFilters, PagedGenericQuery>()
                            .ForMember(r => r.Languages, options => options.MapFrom(a => a.Languages.Select(l => l.ToString()).Where(a => a != string.Empty).ToList()))
                            .ReverseMap();
            configuration.CreateMap<EventFilters, GetEventsQuery>()
                            .ForMember(r => r.Languages, options => options.MapFrom(a => a.Languages.Select(l => l.ToString()).Where(a => a != string.Empty).ToList()))
                            .ReverseMap();
            configuration.CreateMap<TweetStatFilters, GenericQuery>()
                            .ForMember(r => r.Languages, options => options.MapFrom(a => a.Languages.Select(l => l.ToString()).Where(a => a != string.Empty).ToList()))
                            .ReverseMap();
            configuration.CreateMap<SocialBaseFilters, EventStatsQuery>()
                            .ForMember(r => r.Languages, options => options.MapFrom(a => a.Languages.Select(l => l.ToString()).Where(a => a != string.Empty).ToList()))
                            .ReverseMap();
            /////////////////

            configuration.CreateMap<Person, PersonDto>()
                            .ForMember(r => r.TeamName, options => options.MapFrom(a => a.TeamId.HasValue ? a.Team.Name : string.Empty))
                            .ForMember(r => r.OrganizationName, options => options.MapFrom(a => a.OrganizationId.HasValue ? a.Organization.Name : string.Empty))
                            .ReverseMap();
            configuration.CreateMap<PersonActionSharingPosition, PersonActionDto>()
                            .ForMember(dto => dto.Type, options => options.MapFrom(a => PersonActionType.PersonActionSharingPosition))
                            .ForMember(dto => dto.Timestamp, options => options.MapFrom(a => a.Timestamp.ToUniversalTime()))
                            .ForMember(dto => dto.Latitude, options => options.MapFrom(a => a.Location != null ? (double?)a.Location.Coordinate.Y : null))
                            .ForMember(dto => dto.Longitude, options => options.MapFrom(a => a.Location != null ? (double?)a.Location.Coordinate.X : null))
                            .ReverseMap()
                            .ForMember(entity => entity.Location, options => options.MapFrom(l => l.Latitude.HasValue && l.Longitude.HasValue ? new Point(l.Longitude.Value, l.Latitude.Value) : null))
                            .ForMember(entity => entity.CurrentExtensionData, options => options.MapFrom(l => l.ExtensionData));
            configuration.CreateMap<PersonActionTracking, PersonActionDto>()
                            .ForMember(dto => dto.Type, options => options.MapFrom(a => PersonActionType.PersonActionTracking))
                            .ForMember(dto => dto.Timestamp, options => options.MapFrom(a => a.Timestamp.ToUniversalTime()))
                            .ForMember(dto => dto.Latitude, options => options.MapFrom(a => a.Location != null ? (double?)a.Location.Coordinate.Y : null))
                            .ForMember(dto => dto.Longitude, options => options.MapFrom(a => a.Location != null ? (double?)a.Location.Coordinate.X : null))
                            .ReverseMap()
                            .ForMember(entity => entity.CurrentExtensionData, options => options.MapFrom(l => l.ExtensionData))
                            .ForMember(entity => entity.Location, options => options.MapFrom(l => l.Latitude.HasValue && l.Longitude.HasValue ? new Point(l.Longitude.Value, l.Latitude.Value) : null));
            configuration.CreateMap<PersonActionStatus, PersonActionDto>()
                            .ForMember(dto => dto.Type, options => options.MapFrom(a => PersonActionType.PersonActionStatus))
                            .ForMember(dto => dto.Timestamp, options => options.MapFrom(a => a.Timestamp.ToUniversalTime()))
                            .ForMember(dto => dto.Latitude, options => options.MapFrom(a => a.Location != null ? (double?)a.Location.Coordinate.Y : null))
                            .ForMember(dto => dto.Longitude, options => options.MapFrom(a => a.Location != null ? (double?)a.Location.Coordinate.X : null))
                            .ReverseMap()
                            .ForMember(entity => entity.Location, options => options.MapFrom(l => l.Latitude.HasValue && l.Longitude.HasValue ? new Point(l.Longitude.Value, l.Latitude.Value) : null))
                            .ForMember(entity => entity.CurrentStatus, options => options.MapFrom(l => l.Status));
            configuration.CreateMap<PersonActionActivity, PersonActionDto>()
                           .ForMember(dto => dto.Type, options => options.MapFrom(a => PersonActionType.PersonActionActivity))
                           .ForMember(dto => dto.Status, options => options.MapFrom(a => a.CurrentStatus))
                           .ForMember(dto => dto.Timestamp, options => options.MapFrom(a => a.Timestamp.ToUniversalTime()))
                           .ForMember(dto => dto.Latitude, options => options.MapFrom(a => a.Location != null ? (double?)a.Location.Coordinate.Y : null))
                           .ForMember(dto => dto.Longitude, options => options.MapFrom(a => a.Location != null ? (double?)a.Location.Coordinate.X : null))
                           .ReverseMap()
                           .ForMember(entity => entity.Location, options => options.MapFrom(l => l.Latitude.HasValue && l.Longitude.HasValue ? new Point(l.Longitude.Value, l.Latitude.Value) : null))
                           .ForMember(entity => entity.CurrentActivityId, options => options.MapFrom(l => l.ActivityId));
            configuration.CreateMap<ImportResultDto, ImportUsersDto>().ReverseMap();
            configuration.CreateMap<MapRequests.MapRequest, MapRequestDto>()
                            .ForMember(dto => dto.Centroid, options => options.MapFrom(b => new PointPosition(b.AreaOfInterest.Centroid.X, b.AreaOfInterest.Centroid.Y)))
                            .ForMember(dto => dto.Organization, options => options.MapFrom(b => b.Creator.Organization))
                            .ForMember(dto => dto.Username, options => options.MapFrom(b => b.Creator.Username))
                            .ForMember(dto => dto.Email, options => options.MapFrom(b => b.Creator.Email))
                            .ForMember(dto => dto.MapRequestType, options => options.MapFrom(b => b.Type))
                            .AfterMap((src, dest) => dest.Duration.LowerBound = dest.Duration.LowerBound.ToUniversalTime())
                            .AfterMap((src, dest) => dest.Duration.UpperBound = dest.Duration.UpperBound.ToUniversalTime())
                            .ReverseMap()
                            .ForMember(entity => entity.Code, options => options.Ignore())
                            .ForMember(entity => entity.Type, options => options.MapFrom(b => b.MapRequestType));
            configuration.CreateMap<MapRequests.MapRequest, MapRequestNotificationDto>();
            configuration.CreateMap<MapRequests.MapRequestLayer, MapRequestLayerDto>()
                            .ReverseMap();
            configuration.CreateMap<MapRequests.MapRequestLayerError, MapRequestLayerErrorDto>()
                            .ReverseMap();

            configuration.CreateMap<Level, LevelDto>()
                            .ForMember(dto => dto.PreviousLevelName, options => options.MapFrom(b => b.PreviousLevelId.HasValue ? b.PreviousLevel.Name : null))
                            .ForMember(dto => dto.FollowingLevelName, options => options.MapFrom(b => b.FollowingLevelId.HasValue ? b.FollowingLevel.Name : null));

            configuration.CreateMap<Person, GamificationBaseDto>()
                .ForMember(dto => dto.LevelName, options => options.MapFrom(b => b.LevelId.HasValue ? b.Level.Name : null))
                .ForMember(dto => dto.LevelId, options => options.MapFrom(b => b.LevelId))
                .ForMember(dto => dto.Username, options => options.MapFrom(b => b.Username))
                .ForMember(dto => dto.Email, options => options.MapFrom(b => b.Email))
                .ForMember(dto => dto.Points, options => options.MapFrom(b => b.Points));

            configuration.CreateMap<MapRequestBody, MapRequestFireAndBurnedAreaBody>()
                .ForMember(dto => dto.start, options => options.MapFrom(b => b.start.ToUniversalTime()))
                .ForMember(dto => dto.end, options => options.MapFrom(b => b.end.ToUniversalTime()));
            configuration.CreateMap<MapRequestBody, MapRequestPostEventMonitoringBody>()
                .ForMember(dto => dto.start, options => options.MapFrom(b => b.start.ToUniversalTime()))
                .ForMember(dto => dto.end, options => options.MapFrom(b => b.end.ToUniversalTime()));
            configuration.CreateMap<MapRequestBody, MapRequestWildFireSimulationBody>()
                .ForMember(dto => dto.datatype_id, options => options.MapFrom(b => b.datatype_id.ToString()))
                .ForMember(dto => dto.start, options => options.MapFrom(b => b.start.ToUniversalTime()))
                .ForMember(dto => dto.end, options => options.MapFrom(b => b.end.ToUniversalTime()));
            configuration.CreateMap<BoundaryCondition, BoundaryConditionBody>()
                .ForMember(dto => dto.w_dir, options => options.MapFrom(b => b.WindDirection))
                .ForMember(dto => dto.w_speed, options => options.MapFrom(b => b.WindSpeed));

            configuration.CreateMap<Alert, AlertDto>()
                .ForMember(dto => dto.Centroid, options => options.MapFrom(b => new PointPosition(b.BoundingBox.Centroid.X, b.BoundingBox.Centroid.Y)));
            configuration.CreateMap<Alert, RabbitMqAlert>()
                .ReverseMap()
                .ForMember(a => a.Info, options => options.Ignore())
                .ForMember(a => a.BoundingBox, options => options.Ignore());
            configuration.CreateMap<CapInfo, CapInfoDto>();
            configuration.CreateMap<CapInfo, RabbitMqAlertInfo>()
                .ReverseMap();
            configuration.CreateMap<CapArea, CapAreaDto>();
            configuration.CreateMap<CapArea, RabbitMqAlertArea>().ReverseMap();

            configuration.CreateMap<Alert, Communication>()
                .ForMember(comm => comm.Duration, options => options.MapFrom(b => new NpgsqlRange<DateTime>(b.Sent, b.Info != null ? b.Info.First().Expires : b.Sent.AddDays(1))))
                .ForMember(comm => comm.Id, options => options.Ignore());

            #region GeoJsons
            configuration.CreateMap<Communication, FeatureDto<GeoJsonItem>>()
                            .ForMember(fd => fd.Geometry, options => options.MapFrom(c => new GeoJsonWriter().Write(c.AreaOfInterest)))
                            .ForPath(fd => fd.Properties.Id, options => options.MapFrom(c => c.Id))
                            .ForPath(fd => fd.Properties.StartDate, options => options.MapFrom(c => c.Duration.LowerBound))
                            .ForPath(fd => fd.Properties.EndDate, options => options.MapFrom(c => c.Duration.UpperBound))
                            .ForPath(fd => fd.Properties.Type, options => options.MapFrom(c => Enums.EntityType.Communication))
                            .ForPath(fd => fd.Properties.Details, options => options.MapFrom(c => c.Message));
            configuration.CreateMap<Mission, FeatureDto<GeoJsonItem>>()
                           .ForMember(fd => fd.Geometry, options => options.MapFrom(c => new GeoJsonWriter().Write(c.AreaOfInterest)))
                           .ForPath(fd => fd.Properties.Id, options => options.MapFrom(c => c.Id))
                           .ForPath(fd => fd.Properties.StartDate, options => options.MapFrom(c => c.Duration.LowerBound))
                           .ForPath(fd => fd.Properties.EndDate, options => options.MapFrom(c => c.Duration.UpperBound))
                           .ForPath(fd => fd.Properties.Type, options => options.MapFrom(c => Enums.EntityType.Mission))
                           .ForPath(fd => fd.Properties.Details, options => options.MapFrom(c => c.Title))
                           .ForPath(fd => fd.Properties.Status, options => options.MapFrom(c => c.CurrentStatusString));
            configuration.CreateMap<Report, FeatureDto<GeoJsonItem>>()
                           .ForPath(fd => fd.Properties.Id, options => options.MapFrom(c => c.Id))
                           .ForPath(fd => fd.Properties.StartDate, options => options.MapFrom(c => c.Timestamp))
                           .ForPath(fd => fd.Properties.EndDate, options => options.MapFrom(c => c.Timestamp))
                           .ForPath(fd => fd.Properties.Type, options => options.MapFrom(c => Enums.EntityType.Report))
                           .ForPath(fd => fd.Properties.Details, options => options.MapFrom(c => c.Description))
                           .ForPath(fd => fd.Properties.Status, options => options.MapFrom(c => c.StatusString));
            configuration.CreateMap<PersonAction, FeatureDto<GeoJsonItem>>()
                           .ForMember(fd => fd.Geometry, options => options.MapFrom(c => new GeoJsonWriter().Write(c.Location)))
                           .ForPath(fd => fd.Properties.Id, options => options.MapFrom(c => c.Id))
                           .ForPath(fd => fd.Properties.StartDate, options => options.MapFrom(c => c.Timestamp))
                           .ForPath(fd => fd.Properties.EndDate, options => options.MapFrom(c => c.Timestamp))
                           //.ForPath(fd => fd.Properties.Type, options => options.MapFrom(c => EntityType.PersonAction))
                           .ForPath(fd => fd.Properties.Status, options => options.MapFrom(c => c.CurrentStatusString));
            configuration.CreateMap<PersonAction, PersonActionDto>();
            configuration.CreateMap<Activation, ActivationDto>()
                            .ForMember(dto => dto.Y, options => options.MapFrom(c => c.Counter))
                            .ReverseMap();

            configuration.CreateMap<Station, SensorServiceStation>()
                .ForMember(a => a.Id, options => options.MapFrom(s => s.SensorServiceId))
                .ReverseMap()
                .ForMember(s => s.SensorServiceId, options => options.MapFrom(c => c.Id))
                .ForMember(s => s.Id, options => options.Ignore())
                .ForMember(s => s.Location, options => options.MapFrom(a => new Point((double)a.Location.Coordinates[1], (double)a.Location.Coordinates[0])));
            configuration.CreateMap<StationDto, SensorServiceStation>()
                .ReverseMap()
                .ForMember(dto => dto.Type, options => options.Ignore())
                .ForMember(dto => dto.StationType, options => options.MapFrom(b => b.ProductCode));
            configuration.CreateMap<SensorDto, SensorServiceSensor>().ReverseMap();

            configuration.CreateMap<MeasureDto, SensorServiceMeasure>().ReverseMap()
                .ForMember(dto => dto.Timestamp, options => options.MapFrom(b => b.DateStart));


            configuration.CreateMap<SensorServiceStationLocation, PointPosition>()
                .ForMember(pos => pos.Latitude, options => options.MapFrom(b => b.Coordinates[0]))
                .ForMember(pos => pos.Longitude, options => options.MapFrom(b => b.Coordinates[1]))
                .ReverseMap();
            #endregion


        }
    }
}
