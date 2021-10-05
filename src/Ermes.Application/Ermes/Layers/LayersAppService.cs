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
using System.Text;
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
        public virtual async Task<List<LayerDto>> GetLayerDefinition()
        {
            var layers = await _layerManager.GetLayerDefinitionAsync();
            return ObjectMapper.Map<List<LayerDto>>(layers);
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
                                availableLayers.Select(a => new { DataTypeId = int.Parse(a.Split("_").First().Split(":").ElementAt(1)), FullName = a }).ToList(),
                                a => a.DataTypeId,
                                b => b.DataTypeId,
                                (a, b) => new { Props = a, b.FullName }
                            )
                        .ToList();

                    result.LayerGroups =
                        joinedLayerList
                        .Select(a => new { Props = ObjectMapper.Map<LayerDto>(a.Props), a.FullName })
                        .Select(a => { a.Props.FullName = a.FullName; return a.Props; })
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
