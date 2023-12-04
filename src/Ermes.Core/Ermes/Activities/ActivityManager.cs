using Abp;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Ermes.Configuration;
using Ermes.Enums;
using Ermes.Organizations;
using Ermes.Persons;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ermes.Activities
{
    public class ActivityManager : DomainService
    {
        public IQueryable<Activity> Activities { get { return ActivityRepository.GetAll().Include(a => a.Translations); } }
        protected IRepository<Activity> ActivityRepository { get; set; }
        protected IRepository<ActivityTranslation> ActivityTranslationRepository { get; set; }
        private readonly PersonManager PersonManager;
        private readonly OrganizationManager OrganizationManager;
        ///private readonly CsiManager OrganizationManager;

        public ActivityManager(
            IRepository<Activity> activityRepository, 
            IRepository<ActivityTranslation> activityTranslationRepository,
            PersonManager personManager,
            OrganizationManager organizationManager)
        {
            ActivityRepository = activityRepository;
            ActivityTranslationRepository = activityTranslationRepository;
            PersonManager = personManager;
            OrganizationManager = organizationManager;
        }

        public async Task<List<Activity>> GetAllAsync()
        {
            return await Activities.ToListAsync();
        }

        public async Task<bool> CheckIfActivityExists(int activityId)
        {
            return await Activities
                    .Where(a => a.Id == activityId)
                    .CountAsync() > 0;
        }

        public async Task<Activity> GetActivityByShortNameAsync(string shortName)
        {
            return await ActivityRepository.FirstOrDefaultAsync(a => a.ShortName == shortName);
        }

        public async Task<int> InsertActivityAsync(Activity activity)
        {
            return await ActivityRepository.InsertAndGetIdAsync(activity);
        }

        public async Task<ActivityTranslation> getActivityTranslationByCoreIdAndLangAsync(int coreId, string language)
        {
            return await ActivityTranslationRepository.FirstOrDefaultAsync(at => at.CoreId == coreId && at.Language == language);
        }

        public async Task<int> InsertActivityTranslationAsync(ActivityTranslation activityTranslation)
        {
            return await ActivityTranslationRepository.InsertOrUpdateAndGetIdAsync(activityTranslation);
        }

        public async Task<List<Activity>> GetLeafActivities()
        {
            var parentListId = await Activities.Where(a => a.ParentId.HasValue).Select(a => a.ParentId.Value).ToListAsync();
            return await Activities.Where(a => a.ParentId.HasValue || !parentListId.Contains(a.Id)).ToListAsync();
        }
    }
}
