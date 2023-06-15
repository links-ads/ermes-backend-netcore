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
        public IQueryable<Layer> Layers { get { return LayerRepository.GetAllIncluding(l => l.Translations).Where(l => l.IsActive); } }
        public IQueryable<Layer> AllLayers { get { return LayerRepository.GetAllIncluding(l => l.Translations); } }
        protected IRepository<Layer> LayerRepository { get; set; }
        public IQueryable<LayerTranslation> LayersTranslation { get { return LayerTranslationRepository.GetAllIncluding(ct => ct.Core); } }
        protected IRepository<LayerTranslation> LayerTranslationRepository { get; set; }

        public LayerManager(IRepository<Layer> layerRepository, IRepository<LayerTranslation> layerTranslationRepository)
        {
            LayerRepository = layerRepository;
            LayerTranslationRepository = layerTranslationRepository;
        }

        public async Task<List<Layer>> GetLayerDefinitionAsync(bool? canBeVisualized)
        {
            var query = Layers;
            if (canBeVisualized.HasValue)
                query = query.Where(l => l.CanBeVisualized == canBeVisualized.Value);
            
            return await query.ToListAsync();
        }

        public async Task<Layer> GetLayerByDataTypeIdAsync(int dataTypeId, bool includeInactive = false)
        {
            var query = AllLayers;
            if (includeInactive)
                query.Where(l => l.IsActive);
            return await query.SingleOrDefaultAsync(l => l.DataTypeId == dataTypeId);
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
