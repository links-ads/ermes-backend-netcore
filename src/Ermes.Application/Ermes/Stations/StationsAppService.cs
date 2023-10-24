using Abp.Application.Services.Dto;
using Abp.SensorService;
using Abp.UI;
using Ermes.Attributes;
using Ermes.Dto;
using Ermes.Dto.Datatable;
using Ermes.Ermes.Stations;
using Ermes.Helpers;
using Ermes.Stations.Dto;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Ermes.Stations
{
    [ErmesAuthorize]
    public class StationsAppService : ErmesAppServiceBase, IStationsAppService
    {
        private readonly SensorServiceManager _sensorServiceManager;
        private readonly StationManager _stationManager;
        private readonly ErmesAppSession _session;
        public StationsAppService(SensorServiceManager sensorServiceManager, StationManager stationManager, ErmesAppSession session)
        {
            _sensorServiceManager = sensorServiceManager;
            _stationManager = stationManager;
            _session = session;
        }

        #region Private
        private async Task<PagedResultDto<StationDto>> InternalGetStations(GetStationsInput input)
        {
            PagedResultDto<StationDto> result = new PagedResultDto<StationDto>();
            input.StartDate = input.StartDate.HasValue ? input.StartDate : DateTime.MinValue;
            input.EndDate = input.EndDate.HasValue ? input.EndDate : DateTime.MaxValue;
            try
            {
                var fullStationList = await _sensorServiceManager.GetStations();
                if (input.SouthWestBoundary != null && input.NorthEastBoundary != null)
                {
                    Geometry boundingBox = GeometryHelper.GetPolygonFromBoundaries(input.SouthWestBoundary, input.NorthEastBoundary);
                    fullStationList = fullStationList.Where(a => boundingBox.Contains(GeometryHelper.GetPointFromCoordinates(a.Location.Coordinates))).ToList();
                }
                var stations = new List<StationDto>();
                foreach (var station in fullStationList)
                {
                    var summary = await _sensorServiceManager.GetStationSummary(station.Id, input.StartDate.Value, input.EndDate.Value);
                    stations.Add(ObjectMapper.Map<StationDto>(summary));
                }
                result.TotalCount = stations.Count;
                result.Items = stations
                            .OrderBy(a => a.Name)
                            .Skip(input.SkipCount)
                            .Take(input.MaxResultCount)
                            .ToList();
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("GetStation exception: {0}", e.Message.ToString());
                result.Items = new List<StationDto>();
            }

            return result;
        }
        #endregion

        [OpenApiOperation("Get Stations",
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
                In addition to pagination parameters, there are additional properties for station filtering:
                    - StartDate and EndDate to define a time window of interest
                    - SouthWestBoundary: bottom-left corner of the bounding box for a spatial query. (optional) (to be filled together with NorthEast property)
                    - NorthEastBoundary: top-right corner of the bounding box for a spatial query format. (optional) (to be filled together with SouthWest property)
                Output: list of StationDto elements

                Exception: Sensor service module not available
            "
        )]
        public virtual async Task<DTResult<StationDto>> GetStations(GetStationsInput input)
        {
            PagedResultDto<StationDto> result = await InternalGetStations(input);
            return new DTResult<StationDto>(input.Draw, result.TotalCount, result.Items.Count, result.Items.ToList());
        }

        [OpenApiOperation("Get measures of a specific sensor",
            @"
                This is a server-side paginated API
                Input: use the following properties to filter result list:
                    - StationId: the id of the station (camera)
                    - SensorId: the id of the station (camera direction)
                    - StartDate and EndDate to define a time window of interest

                Output: list of MeasureDto elements
                Exception: Sensor service module not available
            "
        )]

        public virtual async Task<GetMeasuresByStationAndSensorOutput> GetMeasuresByStationAndSensor(GetMeasuresByStationAndSensorInput input)
        {
            input.StartDate = input.StartDate.HasValue ? input.StartDate : DateTime.MinValue;
            input.EndDate = input.EndDate.HasValue ? input.EndDate : DateTime.MaxValue;

            var sensorWithMeasures = await _sensorServiceManager.GetMeasuresOfSensor(input.StationId, input.SensorId, input.StartDate.Value, input.EndDate.Value);
            var measures = ObjectMapper.Map<List<MeasureDto>>(sensorWithMeasures.Measurements);

            return new GetMeasuresByStationAndSensorOutput()
            {
                Measurements = measures
            };
        }


        [OpenApiOperation("Delete station",
            @"
                Delete a station both from sensor service module and API Gateway
                Input:
                    - Id: station id coming from sensor service module

                Output: true if the entity has been successfully deleted, false otherwise
                Exception: Sensor service module not available
            "
        )]
        public virtual async Task<bool> DeleteStation(IdInput<string> input)
        {
            if (input == null || input.Id == "")
                throw new UserFriendlyException(L("InvalidEntityId", "Station", ""));

            try
            {
                if (await _sensorServiceManager.DeleteStation(input.Id))
                    await _stationManager.DeleteStationBySensorServiceIdAsync(input.Id);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

            return true;
        }

        [OpenApiOperation("Delete sensor",
            @"
                Delete a sensor from sensor service module and API Gateway
                Input:
                    - Id: sensor id coming from sensor service module

                Output: true if the entity has been successfully deleted, false otherwise
                Exception: Sensor service module not available
            "
        )]
        public virtual async Task<bool> DeleteSensor(IdInput<string> input)
        {
            if (input == null || input.Id == "")
                throw new UserFriendlyException(L("InvalidEntityId", "Sensor", ""));

            try
            {
                await _sensorServiceManager.DeleteSensor(input.Id);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

            return true;
        }

        [OpenApiOperation("Delete measure",
            @"
                Delete a measure from sensor service module
                Input:
                    - Id: measure id coming from sensor service module

                Output: true if the entity has been successfully deleted, false otherwise
                Exception: Sensor service module not available
            "
        )]
        public virtual async Task<bool> DeleteMeasure(IdInput<string> input)
        {
            if (input == null || input.Id == "")
                throw new UserFriendlyException(L("InvalidEntityId", "Measure", ""));

            try
            {
                await _sensorServiceManager.DeleteMeasure(input.Id);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

            return true;
        }

        [OpenApiOperation("Validate measure",
            @"
                Validate a measure by sending information on fire and smoke
                Input:
                    - MeasureId: id of the measure
                    - Fire: true if the measure contains fire
                    - Smoke: true if the measure contains smoke

                Output: full measure object, with updated metadata object
                Exception: Sensor service module not available or invalid measure id
            "
        )]
        public virtual async Task<ValidateMeasureOutput> ValidateMeasure(ValidateMeasureInput input)
        {
            if (input.MeasureId == null || input.MeasureId == string.Empty)
                throw new UserFriendlyException(L("InvalidEntityId", "Measure", ""));

            var res = new ValidateMeasureOutput();
            try
            {
                dynamic metadata = new ExpandoObject();
                if (input.Metadata != null)
                    metadata = input.Metadata;
                metadata.validation = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(new
                {
                    smoke = input.Smoke,
                    fire = input.Fire,
                    validationTime = DateTime.UtcNow,
                    validator = _session.LoggedUserPerson.Email
                }, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Include }));

                var response = await _sensorServiceManager.UpdateMetadataOfMeasure(input.MeasureId, metadata);
                res.Measure = ObjectMapper.Map<MeasureDto>(response);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

            return res;
        }
    }
}
