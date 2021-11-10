using Abp.Importer;
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
                //var res = await _importerMananger.GetLayers(input.DataTypeIds, input.Bbox, input.Start.Value, input.End.Value);

                var res = @"
                    {
                        'items': 
                            [
                              {
                               'dataType_Id': 33106,
                               'details': [
                               {   
                                  'name': 'ermes:33106_df5a9ca2-beb3-450e-aee5-8eecdfc224f3',
                                  'timestamps': ['2021-10-01T00:00:00.0Z', '2021-10-01T01:00:00.0Z','2021-10-01T02:00:00.0Z', '2021-10-01T06:00:00.0Z'] 
                                },
                               {
                                  'name': 'ermes:33106_fdg453v3-hj89-j98jh-joi-hoiuh9h89u90',
                                  'timestamps': ['2021-10-02T00:00:00.0Z','2021-10-02T01:00:00.0Z','2021-10-02T02:00:00.0Z'] 
                                }
                              ]
                            },
                              {
                               'dataType_Id': 33102,
                               'details': [
                               {   
                                  'name': 'ermes:33106_df5a9ca2-beb3-450e-aee5-8eecdfc224f3',
                                  'timestamps': ['2021-10-01T00:00:00.0Z','2021-10-01T01:00:00.0Z','2021-10-01T02:00:00.0Z']
                                },
                               {
                                  'name': 'ermes:33106_fdg453v3-hj89-j98jh-joi-hoiuh9h89u90',
                                  'timestamps': ['2021-10-02T00:00:00.0Z','2021-10-02T01:00:00.0Z','2021-10-02T02:00:00.0Z', '2021-10-02T05:00:00.0Z']
                                }
                              ]
                             }
                           ]
                    }";


                //var availableLayers = JsonConvert.DeserializeObject<List<string>>(res.ToString());
                var availableLayers = JsonConvert.DeserializeObject<ImporterLayerList>(res.ToString());
                var layerDefinition = await _layerManager.GetLayerDefinitionAsync();

                try
                {
                    //var joinedLayerList = layerDefinition.Join(
                    //            availableLayers.Select(a => new { DataTypeId = int.Parse(a.Split("_").First().Split(":").ElementAt(1)), FullName = a }).GroupBy(l => l.DataTypeId).ToList(),
                    //            a => a.DataTypeId,
                    //            b => b.Key,
                    //            (a, b) => new { Layer = a, LayerGroup = b }
                    //        )
                    //    .ToList();
                    var joinedLayerList = layerDefinition.Join(
                                availableLayers.Items.Select(a => new { DataTypeId = a.DataType_Id, a.Details }).ToList(),
                                a => a.DataTypeId,
                                b => b.DataTypeId,
                                (a, b) => new { Layer = a, b.Details }
                            )
                        .ToList();

                    //result.LayerGroups =
                    //    joinedLayerList
                    //    .Select(a => new { LayerDto = ObjectMapper.Map<LayerDto>(a.Layer), a.LayerGroup })
                    //    .Select(a => { a.LayerDto.Tiles = a.LayerGroup.Select(p => p.FullName).ToList(); return a.LayerDto; })
                    //    .GroupBy(a => new { a.GroupKey, a.Group })
                    //    .Select(a => new LayerGroupDto()
                    //    {
                    //        GroupKey = a.Key.GroupKey,
                    //        Group = a.Key.Group,
                    //        SubGroups = a.ToList().GroupBy(b => new { b.SubGroupKey, b.SubGroup }).Select(c => new LayerSubGroupDto()
                    //        {
                    //            SubGroupKey = c.Key.SubGroupKey,
                    //            SubGroup = c.Key.SubGroup,
                    //            Layers = c.ToList()
                    //        }).ToList()
                    //    })
                    //    .ToList();

                    result.LayerGroups =
                        joinedLayerList
                        .Select(a => new { LayerDto = ObjectMapper.Map<LayerDto>(a.Layer), a.Details })
                        .Select(a => { a.LayerDto.Details = a.Details; return a.LayerDto; })
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
