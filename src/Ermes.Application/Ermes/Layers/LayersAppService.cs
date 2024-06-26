﻿using Abp.Importer;
using Abp.UI;
using Ermes.Attributes;
using Ermes.Layers.Dto;
using Newtonsoft.Json;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ermes.Layers
{
    [ErmesAuthorize()]
    public class LayersAppService : ErmesAppServiceBase, ILayersAppService
    {
        private readonly IImporterManager _importerMananger;
        private readonly LayerManager _layerManager;
        public LayersAppService(IImporterManager importerMananger, LayerManager layerManager)
        {
            _importerMananger = importerMananger;
            _layerManager = layerManager;
        }

        [OpenApiOperation("Get static definition of layer list")]
        public virtual async Task<GetLayersOutput> GetLayerDefinition()
        {
            var result = new GetLayersOutput();
            var layers = await _layerManager.GetLayerDefinitionAsync(true);
            result.LayerGroups =
                        layers
                        .Select(a => ObjectMapper.Map<LayerDto>(a))
                        .OrderBy(l => l.Order)
                        .GroupBy(a => new { a.GroupKey, a.Group })
                        .Select(a => new LayerGroupDto()
                        {
                            GroupKey = a.Key.GroupKey,
                            Group = a.Key.Group,
                            SubGroups = a.ToList().GroupBy(b => new { b.SubGroupKey, b.SubGroup }).Select(c => new LayerSubGroupDto()
                            {
                                SubGroupKey = c.Key.SubGroupKey,
                                SubGroup = c.Key.SubGroup,
                                Layers = c.ToList()
                            }).ToList()
                        })
                        .ToList();

            return result;
        }

        [OpenApiOperation("Get list of available layers on Importer Module",
            @"
                Input: use the following properties to filter result list:
                    - Start: start date
                    - End: end date
                    - Bbox: bounding box of the area of interest
                    - DataTypeIds: list of dataTypeIds to be considered
                    - IncludeMapRequests: if true, the result will contain information about map requests
                    - MapRequestCode: the result will contain information only about the map request corresponding to the inserted code. The format for this field is: <partner>.<mapRequestCode>
                Output: list of available layers
                Exception: Importer service not available
            "
        )]
        public virtual async Task<GetLayersOutput> GetLayers(GetLayersInput input)
        {
            var result = new GetLayersOutput();
            input.Start = input.Start == null ? DateTime.MinValue : input.Start;
            input.End = input.End == null ? DateTime.MaxValue : input.End;
            try
            {
                //1) Get available list of layers from Importer Module
                //2) Join this list with layer definition
                //3) Group the result by GroupKey and SubGroupKey

                var res = await _importerMananger.GetLayers(input.DataTypeIds, input.Bbox, input.Start.Value, input.End.Value, input.MapRequestCodes, input.IncludeMapRequests);

                #region Example
                //Old static example
                //var res = @"
                //    {
                //        'items': [
                //          {
                //          'datatype_id': 33101,
                //          'details': [
                //          {   
                //              'name': 'ermes:33101_a7d22538-c992-4cab-9c1d-77d3f02abace',
                //              'timestamps': [
                //                '2021-11-04T07:00:00.000Z','2021-11-04T08:00:00.000Z','2021-11-04T09:00:00.000Z','2021-11-04T10:00:00.000Z','2021-11-04T11:00:00.000Z','2021-11-04T12:00:00.000Z','2021-11-04T13:00:00.000Z','2021-11-04T14:00:00.000Z','2021-11-04T15:00:00.000Z','2021-11-04T16:00:00.000Z','2021-11-04T17:00:00.000Z','2021-11-04T18:00:00.000Z','2021-11-04T19:00:00.000Z','2021-11-04T20:00:00.000Z','2021-11-04T21:00:00.000Z','2021-11-04T22:00:00.000Z','2021-11-04T23:00:00.000Z','2021-11-05T00:00:00.000Z','2021-11-05T01:00:00.000Z','2021-11-05T02:00:00.000Z','2021-11-05T03:00:00.000Z','2021-11-05T04:00:00.000Z','2021-11-05T05:00:00.000Z','2021-11-05T06:00:00.000Z','2021-11-05T07:00:00.000Z','2021-11-05T08:00:00.000Z','2021-11-05T09:00:00.000Z','2021-11-05T10:00:00.000Z','2021-11-05T11:00:00.000Z','2021-11-05T12:00:00.000Z','2021-11-05T13:00:00.000Z','2021-11-05T14:00:00.000Z','2021-11-05T15:00:00.000Z','2021-11-05T16:00:00.000Z','2021-11-05T17:00:00.000Z','2021-11-05T18:00:00.000Z','2021-11-05T19:00:00.000Z','2021-11-05T20:00:00.000Z','2021-11-05T21:00:00.000Z','2021-11-05T22:00:00.000Z','2021-11-05T23:00:00.000Z','2021-11-06T00:00:00.000Z','2021-11-06T01:00:00.000Z','2021-11-06T02:00:00.000Z','2021-11-06T03:00:00.000Z','2021-11-06T04:00:00.000Z','2021-11-06T05:00:00.000Z','2021-11-06T06:00:00.000Z','2021-11-06T07:00:00.000Z','2021-11-06T08:00:00.000Z','2021-11-06T09:00:00.000Z','2021-11-06T10:00:00.000Z','2021-11-06T11:00:00.000Z','2021-11-06T12:00:00.000Z','2021-11-06T13:00:00.000Z','2021-11-06T14:00:00.000Z','2021-11-06T15:00:00.000Z','2021-11-06T16:00:00.000Z','2021-11-06T17:00:00.000Z','2021-11-06T18:00:00.000Z','2021-11-06T19:00:00.000Z','2021-11-06T20:00:00.000Z','2021-11-06T21:00:00.000Z','2021-11-06T22:00:00.000Z','2021-11-06T23:00:00.000Z','2021-11-07T00:00:00.000Z','2021-11-07T01:00:00.000Z','2021-11-07T02:00:00.000Z','2021-11-07T03:00:00.000Z','2021-11-07T04:00:00.000Z','2021-11-07T05:00:00.000Z','2021-11-07T06:00:00.000Z'
                //                ]
                //            },
                //          {   
                //              'name': 'ermes:33101_eb6d94b4-1f18-45a1-b05c-5c23b6fd2b8c',
                //              'timestamps': [
                //                '2021-10-31T01:00:00.000Z','2021-10-31T02:00:00.000Z','2021-10-31T03:00:00.000Z','2021-10-31T04:00:00.000Z','2021-10-31T05:00:00.000Z','2021-10-31T06:00:00.000Z','2021-10-31T07:00:00.000Z','2021-10-31T08:00:00.000Z','2021-10-31T09:00:00.000Z','2021-10-31T10:00:00.000Z','2021-10-31T11:00:00.000Z','2021-10-31T12:00:00.000Z','2021-10-31T13:00:00.000Z','2021-10-31T14:00:00.000Z','2021-10-31T15:00:00.000Z','2021-10-31T16:00:00.000Z','2021-10-31T17:00:00.000Z','2021-10-31T18:00:00.000Z','2021-10-31T19:00:00.000Z','2021-10-31T20:00:00.000Z','2021-10-31T21:00:00.000Z','2021-10-31T22:00:00.000Z','2021-10-31T23:00:00.000Z','2021-11-01T00:00:00.000Z','2021-11-01T01:00:00.000Z','2021-11-01T02:00:00.000Z','2021-11-01T03:00:00.000Z','2021-11-01T04:00:00.000Z','2021-11-01T05:00:00.000Z','2021-11-01T06:00:00.000Z','2021-11-01T07:00:00.000Z','2021-11-01T08:00:00.000Z','2021-11-01T09:00:00.000Z','2021-11-01T10:00:00.000Z','2021-11-01T11:00:00.000Z','2021-11-01T12:00:00.000Z','2021-11-01T13:00:00.000Z','2021-11-01T14:00:00.000Z','2021-11-01T15:00:00.000Z','2021-11-01T16:00:00.000Z','2021-11-01T17:00:00.000Z','2021-11-01T18:00:00.000Z','2021-11-01T19:00:00.000Z','2021-11-01T20:00:00.000Z','2021-11-01T21:00:00.000Z','2021-11-01T22:00:00.000Z','2021-11-01T23:00:00.000Z','2021-11-02T00:00:00.000Z','2021-11-02T01:00:00.000Z','2021-11-02T02:00:00.000Z','2021-11-02T03:00:00.000Z','2021-11-02T04:00:00.000Z','2021-11-02T05:00:00.000Z','2021-11-02T06:00:00.000Z','2021-11-02T07:00:00.000Z','2021-11-02T08:00:00.000Z','2021-11-02T09:00:00.000Z','2021-11-02T10:00:00.000Z','2021-11-02T11:00:00.000Z','2021-11-02T12:00:00.000Z','2021-11-02T13:00:00.000Z','2021-11-02T14:00:00.000Z','2021-11-02T15:00:00.000Z','2021-11-02T16:00:00.000Z','2021-11-02T17:00:00.000Z','2021-11-02T18:00:00.000Z','2021-11-02T19:00:00.000Z','2021-11-02T20:00:00.000Z','2021-11-02T21:00:00.000Z','2021-11-02T22:00:00.000Z','2021-11-02T23:00:00.000Z','2021-11-03T00:00:00.000Z'
                //                ]
                //            }
                //          ]
                //        },
                //          {
                //          'datatype_id': 33103,
                //          'details': [
                //          {   
                //              'name': 'ermes:33103_a7d22538-c992-4cab-9c1d-77d3f02abace',
                //              'timestamps': [
                //                '2021-11-04T07:00:00.000Z','2021-11-04T08:00:00.000Z','2021-11-04T09:00:00.000Z','2021-11-04T10:00:00.000Z','2021-11-04T11:00:00.000Z','2021-11-04T12:00:00.000Z','2021-11-04T13:00:00.000Z','2021-11-04T14:00:00.000Z','2021-11-04T15:00:00.000Z','2021-11-04T16:00:00.000Z','2021-11-04T17:00:00.000Z','2021-11-04T18:00:00.000Z','2021-11-04T19:00:00.000Z','2021-11-04T20:00:00.000Z','2021-11-04T21:00:00.000Z','2021-11-04T22:00:00.000Z','2021-11-04T23:00:00.000Z','2021-11-05T00:00:00.000Z','2021-11-05T01:00:00.000Z','2021-11-05T02:00:00.000Z','2021-11-05T03:00:00.000Z','2021-11-05T04:00:00.000Z','2021-11-05T05:00:00.000Z','2021-11-05T06:00:00.000Z','2021-11-05T07:00:00.000Z','2021-11-05T08:00:00.000Z','2021-11-05T09:00:00.000Z','2021-11-05T10:00:00.000Z','2021-11-05T11:00:00.000Z','2021-11-05T12:00:00.000Z','2021-11-05T13:00:00.000Z','2021-11-05T14:00:00.000Z','2021-11-05T15:00:00.000Z','2021-11-05T16:00:00.000Z','2021-11-05T17:00:00.000Z','2021-11-05T18:00:00.000Z','2021-11-05T19:00:00.000Z','2021-11-05T20:00:00.000Z','2021-11-05T21:00:00.000Z','2021-11-05T22:00:00.000Z','2021-11-05T23:00:00.000Z','2021-11-06T00:00:00.000Z','2021-11-06T01:00:00.000Z','2021-11-06T02:00:00.000Z','2021-11-06T03:00:00.000Z','2021-11-06T04:00:00.000Z','2021-11-06T05:00:00.000Z','2021-11-06T06:00:00.000Z','2021-11-06T07:00:00.000Z','2021-11-06T08:00:00.000Z','2021-11-06T09:00:00.000Z','2021-11-06T10:00:00.000Z','2021-11-06T11:00:00.000Z','2021-11-06T12:00:00.000Z','2021-11-06T13:00:00.000Z','2021-11-06T14:00:00.000Z','2021-11-06T15:00:00.000Z','2021-11-06T16:00:00.000Z','2021-11-06T17:00:00.000Z','2021-11-06T18:00:00.000Z','2021-11-06T19:00:00.000Z','2021-11-06T20:00:00.000Z','2021-11-06T21:00:00.000Z','2021-11-06T22:00:00.000Z','2021-11-06T23:00:00.000Z','2021-11-07T00:00:00.000Z','2021-11-07T01:00:00.000Z','2021-11-07T02:00:00.000Z','2021-11-07T03:00:00.000Z','2021-11-07T04:00:00.000Z','2021-11-07T05:00:00.000Z','2021-11-07T06:00:00.000Z'
                //                ]
                //            },
                //          {   
                //              'name': 'ermes:33103_eb6d94b4-1f18-45a1-b05c-5c23b6fd2b8c',
                //              'timestamps': [
                //                '2021-10-31T01:00:00.000Z','2021-10-31T02:00:00.000Z','2021-10-31T03:00:00.000Z','2021-10-31T04:00:00.000Z','2021-10-31T05:00:00.000Z','2021-10-31T06:00:00.000Z','2021-10-31T07:00:00.000Z','2021-10-31T08:00:00.000Z','2021-10-31T09:00:00.000Z','2021-10-31T10:00:00.000Z','2021-10-31T11:00:00.000Z','2021-10-31T12:00:00.000Z','2021-10-31T13:00:00.000Z','2021-10-31T14:00:00.000Z','2021-10-31T15:00:00.000Z','2021-10-31T16:00:00.000Z','2021-10-31T17:00:00.000Z','2021-10-31T18:00:00.000Z','2021-10-31T19:00:00.000Z','2021-10-31T20:00:00.000Z','2021-10-31T21:00:00.000Z','2021-10-31T22:00:00.000Z','2021-10-31T23:00:00.000Z','2021-11-01T00:00:00.000Z','2021-11-01T01:00:00.000Z','2021-11-01T02:00:00.000Z','2021-11-01T03:00:00.000Z','2021-11-01T04:00:00.000Z','2021-11-01T05:00:00.000Z','2021-11-01T06:00:00.000Z','2021-11-01T07:00:00.000Z','2021-11-01T08:00:00.000Z','2021-11-01T09:00:00.000Z','2021-11-01T10:00:00.000Z','2021-11-01T11:00:00.000Z','2021-11-01T12:00:00.000Z','2021-11-01T13:00:00.000Z','2021-11-01T14:00:00.000Z','2021-11-01T15:00:00.000Z','2021-11-01T16:00:00.000Z','2021-11-01T17:00:00.000Z','2021-11-01T18:00:00.000Z','2021-11-01T19:00:00.000Z','2021-11-01T20:00:00.000Z','2021-11-01T21:00:00.000Z','2021-11-01T22:00:00.000Z','2021-11-01T23:00:00.000Z','2021-11-02T00:00:00.000Z','2021-11-02T01:00:00.000Z','2021-11-02T02:00:00.000Z','2021-11-02T03:00:00.000Z','2021-11-02T04:00:00.000Z','2021-11-02T05:00:00.000Z','2021-11-02T06:00:00.000Z','2021-11-02T07:00:00.000Z','2021-11-02T08:00:00.000Z','2021-11-02T09:00:00.000Z','2021-11-02T10:00:00.000Z','2021-11-02T11:00:00.000Z','2021-11-02T12:00:00.000Z','2021-11-02T13:00:00.000Z','2021-11-02T14:00:00.000Z','2021-11-02T15:00:00.000Z','2021-11-02T16:00:00.000Z','2021-11-02T17:00:00.000Z','2021-11-02T18:00:00.000Z','2021-11-02T19:00:00.000Z','2021-11-02T20:00:00.000Z','2021-11-02T21:00:00.000Z','2021-11-02T22:00:00.000Z','2021-11-02T23:00:00.000Z','2021-11-03T00:00:00.000Z'
                //                ]
                //            }
                //          ]
                //        }
                //      ]
                //    }";
                #endregion

                var availableLayers = JsonConvert.DeserializeObject<ImporterLayerList>(res.ToString());
                var layerDefinition = await _layerManager.GetLayerDefinitionAsync(true);

                try
                {
                    var joinedLayerList = layerDefinition.Join(
                                availableLayers.Items.Where(a => a.Details.Where(b => b.Timestamps != null && b.Timestamps.Count > 0).Count() > 0).Select(a => new { DataTypeId = a.DataType_Id, a.Details }).ToList(),
                                a => a.DataTypeId,
                                b => b.DataTypeId,
                                (a, b) => new { Layer = a, b.Details }
                            )
                        .ToList();

                    result.LayerGroups =
                        joinedLayerList
                        .Select(a => new { LayerDto = ObjectMapper.Map<LayerDto>(a.Layer), a.Details })
                        .Select(a => { a.LayerDto.Details = a.Details; return a.LayerDto; })
                        .OrderBy(l => l.Order)
                        .GroupBy(a => new { a.GroupKey, a.Group })
                        .Select(a => new LayerGroupDto()
                        {
                            GroupKey = a.Key.GroupKey,
                            Group = a.Key.Group,
                            SubGroups = a.ToList().GroupBy(b => new { b.SubGroupKey, b.SubGroup }).Select(c => new LayerSubGroupDto()
                            {
                                SubGroupKey = c.Key.SubGroupKey,
                                SubGroup = c.Key.SubGroup,
                                Layers = c.ToList()
                            }).ToList()
                        })
                        .ToList();
                }
                catch (Exception e)
                {
                    throw new UserFriendlyException(string.Format("Exception while joining layer lists: {0}", e.Message));
                }

                //Associated layers management
                layerDefinition = await _layerManager.GetLayerDefinitionAsync(false);
                try
                {
                    var joinedLayerList = layerDefinition.Join(
                            availableLayers.Items.Where(a => a.Details.Where(b => b.Timestamps != null && b.Timestamps.Count > 0).Count() > 0).Select(a => new { DataTypeId = a.DataType_Id, a.Details }).ToList(),
                            a => a.DataTypeId,
                            b => b.DataTypeId,
                            (a, b) => new { Layer = a, b.Details }
                        )
                    .ToList();

                    result.AssociatedLayers =
                        joinedLayerList
                        .Select(a => new { LayerDto = ObjectMapper.Map<LayerDto>(a.Layer), a.Details })
                        .Select(a => { a.LayerDto.Details = a.Details; return a.LayerDto; })
                        .OrderBy(l => l.Order)
                        .ToList();
                }
                catch (Exception e)
                {
                    throw new UserFriendlyException(string.Format("Exception while joining layer lists for associated layers: {0}", e.Message));
                }
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("#####Importer module not available: {0}", e.Message);
            }

            return result;
        }

        [OpenApiOperation("Gets the layer metadata given the metadata_id.",
            @"
                Input: use the following properties to filter result list:
                    - MetadataId: metadata id of the layer. It's returned by GetLayers API
                    
                Exception: Importer service not available
            "
        )]
        public virtual async Task<object> GetMetadata(GetMetadataInput input)
        {
            try
            {
                return await _importerMananger.GetMetadata(input.MetadataId);
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        [OpenApiOperation("Retrieves the time series of the requested attribute for layers denoted by the specified datatype_id, at the \"point\" position",
            @"
                Input: use the following properties to filter result list:
                    - DatatypeId: datatype_id of the layers to retrieve the attribute time series (required)
                    - Point: point string in WKT format. Example: 'POINT(7 45)' (required)
                    - Crs: coordinate reference system. Example: 'EPSG:4326' (required)
                    - RequestCode: request code related to the map request of interest
                    - LayerName: the name of the layer
                    - StartDate: start date, format YYYY-MM-DDTHH:MM:SS.000Z
                    - EndDate: end date, format YYYY-MM-DDTHH:MM:SS.000Z
                    
                Exception: Importer service not available
            "
        )]
        public virtual async Task<GetTimeSeriesOutput> GetTimeSeries(GetTimeSeriesInput input)
        {
            var result = new GetTimeSeriesOutput();
            input.StartDate = input.StartDate == null ? DateTime.MinValue : input.StartDate;
            input.EndDate = input.EndDate == null ? DateTime.MaxValue : input.EndDate;
            input.RequestCode = input.RequestCode == null || input.RequestCode == string.Empty || input.RequestCode.Contains(AppConsts.Ermes_House_Partner) ? input.RequestCode : string.Format("{0}.{1}", AppConsts.Ermes_House_Partner, input.RequestCode);
            try
            {
                var response = await _importerMananger.GetTimeSeries(input.DatatypeId, input.Point, input.Crs, input.RequestCode, input.LayerName, input.StartDate, input.EndDate);
                if (response != null)
                    result.Variables = JsonConvert.DeserializeObject<List<TimeSeriesVariableDto>>(response.ToString());
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
            return result;
        }
        [OpenApiOperation("Gets the resource file given the layer name or the resource id",
            @"
                Input:
                    - LayerName: name of the layer
                    - ResourceId: id of the resource
                Output: filename of the file to be downloaded.
                        To download the file, it's necessary to do, from the client:
                            location.href = {{base_url}}/download?filename={fileName}', where {{base_url}} refers to Importer module
                Exception: Importer service not available
            "
        )]
        public virtual async Task<GetFilenameOutput> GetFilename(GetFilenameInput input)
        {
            GetFilenameOutput result = new GetFilenameOutput();
            try
            {
                var response = await _importerMananger.GetFilename(input.LayerName, input.ResourceId);
                if (response != null)
                    result = JsonConvert.DeserializeObject<GetFilenameOutput>(response.ToString());
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
            return result;
        }
    }
}
