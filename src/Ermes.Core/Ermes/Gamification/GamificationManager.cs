﻿using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Ermes.Gamification;
using Ermes.Persons;
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
        private readonly PersonManager PersonManager;

        public GamificationManager(
                IRepository<Level> levelRepository, 
                IRepository<GamificationAction> actionRepository, 
                IRepository<GamificationActionTranslation> actionTranslationRepository,
                PersonManager personManager)
        {
            LevelRepository = levelRepository;
            ActionRepository = actionRepository;
            ActionTranslationRepository = actionTranslationRepository;
            PersonManager = personManager;
        }

        public async Task<List<Level>> GetLevelsAsync()
        {
            return await Levels.ToListAsync();
        }

        public async Task<Level> GetLevelByPointsAsync(int points)
        {
            return await Levels.Where(l => l.LowerBound <= points && l.UpperBound >= points).SingleAsync();
        }

        public async Task<GamificationAction> GetActionByCodeAsync(string code)
        {
            return await Actions.SingleOrDefaultAsync(a => a.Code == code);
        }

        public async Task<GamificationAction> GetActionByNameAsync(string name)
        {
            return await Actions.SingleOrDefaultAsync(a => a.Name == name);
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

        //True --> send notification
        public async Task<(bool sendNotification, string newValue)> UpdatePersonGamificationProfileAsync(long personId, string actionName)
        {
            var person = await PersonManager.GetPersonByIdAsync(personId);
            var action = await GetActionByNameAsync(actionName);

            if (person == null || action == null)
            {
                Logger.ErrorFormat("Error in UpdatePersonGamificationProfileAsync for person with id {0} and action {1}", personId, actionName);
                return (false,string.Empty);
            }

            person.Points += action.Points;

            var level = await GetLevelByPointsAsync(person.Points);
            (bool, string) result = (level.Id != person.LevelId, level.Name);
            person.LevelId = level.Id;

            return result;
        }
    }
}
