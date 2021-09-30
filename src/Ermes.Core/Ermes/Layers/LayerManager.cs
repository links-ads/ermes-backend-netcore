using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Layers
{
    public class LayerManager: DomainService
    {
        public IQueryable<Layer> Layers { get { return LayerRepository.GetAllIncluding(l => l.Translations); } }
        protected IRepository<Layer> LayerRepository { get; set; }
        public IQueryable<LayerTranslation> LayersTranslation { get { return LayerTranslationRepository.GetAllIncluding(ct => ct.Core); } }
        protected IRepository<LayerTranslation> LayerTranslationRepository { get; set; }

        public LayerManager(IRepository<Layer> layerRepository, IRepository<LayerTranslation> layerTranslationRepository)
        {
            LayerRepository = layerRepository;
            LayerTranslationRepository = layerTranslationRepository;
        }

        public async Task<List<Layer>> GetLayerDefinitionAsync()
        {
            return await Layers.ToListAsync();
        }

        public async Task<Layer> GetLayerByDataTypeIdAsync(int dataTypeId)
        {
            return await Layers.SingleOrDefaultAsync(l => l.DataTypeId == dataTypeId);
        }

        public async Task InsertOrUpdateLayerAsync(Layer layer)
        {
            await LayerRepository.InsertOrUpdateAsync(layer);
        }

        public async Task<LayerTranslation> GetLayerTranslationByCoreIdLanguageAsync(int coreId, string language)
        {
            return await LayersTranslation.SingleOrDefaultAsync(a => a.CoreId == coreId && a.Language == language);
        }

        public async Task InsertOrUpdateLayerTranslationAsync(LayerTranslation translation)
        {
            await LayerTranslationRepository.InsertOrUpdateAsync(translation);
        }
    }
}
