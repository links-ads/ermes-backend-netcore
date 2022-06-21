using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Ermes.Gamification;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Gamification
{
    public class GamificationManager: DomainService
    {
        protected IRepository<Level> LevelRepository{ get; set; }
        protected IRepository<GamificationAction> ActionRepository { get; set; }
        protected IRepository<GamificationActionTranslation> ActionTranslationRepository { get; set; }
        public IQueryable<Level> Levels { get { return LevelRepository.GetAll(); } }
        public IQueryable<GamificationAction> Actions { get { return ActionRepository.GetAll().Include(c => c.Translations); } }
        public IQueryable<GamificationActionTranslation> ActionTranslations { get { return ActionTranslationRepository.GetAll(); } }

        public GamificationManager(IRepository<Level> levelRepository, IRepository<GamificationAction> actionRepository, IRepository<GamificationActionTranslation> actionTranslationRepository)
        {
            LevelRepository = levelRepository;
            ActionRepository = actionRepository;
            ActionTranslationRepository = actionTranslationRepository;
        }

        public async Task<List<Level>> GetLevelsAsync()
        {
            return await Levels.ToListAsync();
        }

        public async Task<GamificationAction> GetActionByCodeAsync(string code)
        {
            return await Actions.SingleOrDefaultAsync(a => a.Code == code);
        }

        public async Task InsertOrUpdateActionAsync(GamificationAction action)
        {
            await ActionRepository.InsertOrUpdateAndGetIdAsync(action);
        }

        public async Task<GamificationActionTranslation> GetActionTranslationByCoreIdLanguageAsync(int coreId, string language)
        {
            return await ActionTranslations.SingleOrDefaultAsync(a => a.CoreId == coreId && a.Language == language);
        }

        public async Task<int> InsertOrUpdateActionTranslationAsync(GamificationActionTranslation actionTrans)
        {
            return await ActionTranslationRepository.InsertOrUpdateAndGetIdAsync(actionTrans);
        }
    }
}
