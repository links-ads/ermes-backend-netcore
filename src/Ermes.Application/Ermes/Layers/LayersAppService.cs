﻿using Abp.Importer;
using Abp.UI;
using Ermes.Attributes;
using Ermes.Authorization;
using Ermes.Layers.Dto;
using Newtonsoft.Json;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ermes.Layers
{
    [ErmesAuthorize(AppPermissions.Backoffice)]
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
            var layers = await _layerManager.GetLayerDefinitionAsync();
            result.LayerGroups =
                        layers
                        .Select(a => ObjectMapper.Map<LayerDto>(a))
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
                var res = await _importerMananger.GetLayers(input.DataTypeIds, input.Bbox, input.Start.Value, input.End.Value);
                var availableLayers = JsonConvert.DeserializeObject<List<string>>(res.ToString());
                var layerDefinition = await _layerManager.GetLayerDefinitionAsync();

                try
                {
                    var joinedLayerList = layerDefinition.Join(
                                availableLayers.Select(a => new { DataTypeId = int.Parse(a.Split("_").First().Split(":").ElementAt(1)), FullName = a }).GroupBy(l => l.DataTypeId).ToList(),
                                a => a.DataTypeId,
                                b => b.Key,
                                (a, b) => new { Layer = a, LayerGroup = b }
                            )
                        .ToList();

                    result.LayerGroups =
                        joinedLayerList
                        .Select(a => new { LayerDto = ObjectMapper.Map<LayerDto>(a.Layer), a.LayerGroup })
                        .Select(a => { a.LayerDto.Tiles = a.LayerGroup.Select(p => p.FullName).ToList(); return a.LayerDto; })
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
                catch(Exception e)
                {
                    throw new UserFriendlyException("MalformedLayerName");
                }
            }
            catch(Exception e)
            {
                Logger.ErrorFormat("#####Importer module not available: {0}", e.Message);
            }

            return result;
        }
    }
}