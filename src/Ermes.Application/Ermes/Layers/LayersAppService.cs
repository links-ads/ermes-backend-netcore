using Abp.Importer;
using Ermes.Attributes;
using Ermes.Authorization;
using Ermes.Layers.Dto;
using Newtonsoft.Json;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Layers
{
    [ErmesAuthorize(AppPermissions.Backoffice)]
    public class LayersAppService : ErmesAppServiceBase, ILayersAppService
    {
        private readonly IImporterManager _importerMananger;
        public LayersAppService(IImporterManager importerMananger)
        {
            _importerMananger = importerMananger;
        }

        [OpenApiOperation("Get list of layers",
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
        public virtual async Task<List<string>> GetLayers(GetLayersInput input)
        {
            input.Start = input.Start == null ? DateTime.MinValue : input.Start;
            input.End = input.End == null ? DateTime.MaxValue : input.End;
            //TODO: results need to joined with layer definition
            var res = await _importerMananger.GetLayers(input.DataTypeIds, input.Bbox, input.Start.Value, input.End.Value);
            return JsonConvert.DeserializeObject<List<string>>(res.ToString());
        }
    }
}
