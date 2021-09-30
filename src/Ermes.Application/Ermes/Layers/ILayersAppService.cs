using Ermes.Interfaces;
using Ermes.Layers.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Layers
{
    public interface ILayersAppService: IBackofficeApi
    {
        Task<List<string>> GetLayers(GetLayersInput input);

        Task<List<LayerDto>> GetLayerDefinition();
    }
}
