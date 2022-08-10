using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Ermes.Authorization;
using Ermes.Enums;
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
        protected IRepository<Reward> RewardRepository { get; set; }
        protected IRepository<GamificationAction> ActionRepository { get; set; }
        protected IRepository<GamificationAudit> AuditRepository { get; set; }
        public IQueryable<Level> Levels { get { return LevelRepository.GetAll(); } }
        public IQueryable<Medal> Medals { get { return RewardRepository.GetAll().OfType<Medal>(); } }
        public IQueryable<Badge> Badges { get { return RewardRepository.GetAll().OfType<Badge>(); } }
        public IQueryable<Award> Awards { get { return RewardRepository.GetAll().OfType<Award>(); } }
        public IQueryable<GamificationAction> Actions { get { return ActionRepository.GetAll().Include(c => c.Achievements); } }
        public IQueryable<GamificationAudit> GamificationAudits { get { return AuditRepository.GetAll(); } }
        private readonly PersonManager PersonManager;

        public GamificationManager(
                IRepository<Level> levelRepository, 
                IRepository<GamificationAction> actionRepository, 
                IRepository<Reward> rewardRepository,
                IRepository<GamificationAudit> auditRepository, 
                PersonManager personManager)
        {
            LevelRepository = levelRepository;
            ActionRepository = actionRepository;
            PersonManager = personManager;
            AuditRepository = auditRepository;
            RewardRepository = rewardRepository;
        }

        public async Task<List<Level>> GetLevelsAsync()
        {
            return await Levels.ToListAsync();
        }

        public async Task<List<Medal>> GetMedalsAsync()
        {
            return await Medals.ToListAsync();
        }
        public async Task<List<Badge>> GetBadgesAsync()
        {
            return await Badges.ToListAsync();
        }
        public async Task<List<Award>> GetAwardsAsync()
        {
            return await Awards.ToListAsync();
        }

        public async Task<Level> GetLevelByPointsAsync(int points)
        {
            return await Levels.Include(l => l.Barriers).Where(l => l.LowerBound <= points && l.UpperBound >= points).SingleAsync();
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

        public async Task InsertAudit(long personId, int? actionId, int? rewardId, int? levelId)
        {
            var item = new GamificationAudit()
            {
                GamificationActionId = actionId,
                PersonId = personId,
                RewardId = rewardId,
                LevelId = levelId
            };
            
            await AuditRepository.InsertAndGetIdAsync(item);
        }

        
        public async Task<List<(EntityWriteAction Action, string NewValue)>> UpdatePersonGamificationProfileAsync(long personId, string actionName, Func<long, Task<List<(EntityWriteAction, string newValue)>>> assignRewards)
        {
            var person = await PersonManager.GetPersonByIdAsync(personId);
            var action = await GetActionByNameAsync(actionName);
            List<(EntityWriteAction, string newValue)> result = new List<(EntityWriteAction, string newValue)>();

            if (person == null || action == null)
            {
                Logger.ErrorFormat("Error in UpdatePersonGamificationProfileAsync for person with id {0} and action {1}", personId, actionName);
                return result;
            }

            var personRole = await PersonManager.GetPersonRolesAsync(personId);
            if(!personRole.Any(r => r.Role.Name == AppRoles.CITIZEN))
                return result;

            person.Points += action.Points;
            await InsertAudit(person.Id, action.Id, null, null);

            if(assignRewards != null)
                result.AddRange(await assignRewards(person.Id));

            var level = await GetLevelByPointsAsync(person.Points);
            if (level.Id != person.LevelId)
            {
                bool canChangeLevel = await CheckBarriersAsync(person, level);
                if (canChangeLevel)
                {
                    result.Add((action.Points > 0 ? EntityWriteAction.LevelChangeUp : EntityWriteAction.LevelChangeDown, level.Name));
                    person.LevelId = level.Id;
                    await InsertAudit(person.Id, null, null, person.LevelId);
                }
                else
                    Logger.InfoFormat("User {0} cannot improve his level because he's missing some barriers", person.Email);
            }
            
            return result;
        }

        public async Task<List<Medal>> GetPersonMedalsAsync(long personId)
        {
            return await GamificationAudits
                            .Where(a => a.PersonId == personId && a.RewardId.HasValue)
                            .Include(a => a.Reward)
                            .Select(a => a.Reward)
                            .OfType<Medal>()
                            .ToListAsync();
        }

        public async Task<List<Badge>> GetPersonBadgesAsync(long personId)
        {
            return await GamificationAudits
                            .Where(a => a.PersonId == personId && a.RewardId.HasValue)
                            .Include(a => a.Reward)
                            .Select(a => a.Reward)
                            .OfType<Badge>()
                            .ToListAsync();
        }

        public async Task<Level> GetDefaultLevel()
        {
            return await Levels.SingleOrDefaultAsync(l => !l.PreviousLevelId.HasValue);
        }

        public async Task<bool> CheckBarriersAsync(Person person, Level newLevel)
        {
            var barriers = newLevel.Barriers.Where(b => b.Level.Id == newLevel.Id).Select(b => b.RewardName).ToList();
            return await GamificationAudits.Where(ga => ga.PersonId == person.Id).AnyAsync(ga => barriers.Contains(ga.Reward.Name));
        }
    }
}
