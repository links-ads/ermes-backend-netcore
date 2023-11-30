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

        public async Task CreateInterventionAsync(long personId, double? latitude, double? longitude, DateTime timestamp, ActionStatusType status, string activityName = "Sorveglianza")
        {
            var person = await _personManager.GetPersonByIdAsync(personId);
            /*
             * Some persons inside Protezione Civile Piemonte do not have an associated legacyId
             * but they can operate on the field. In this case it is not necessary to open/close an intervention
             */
            if (!person.OrganizationId.HasValue || !person.LegacyId.HasValue)
                return;

            //It is not necessary to create an intervention when first responder goes back to Active status without passing through Off status
            //Example: Active -> Moving -> Active -> Off
            //This must create only one Intervention
            if (person.CurrentOperationLegacyId.HasValue && status == ActionStatusType.Active)
                return;

            var refOrg = await _organizationManager.GetOrganizationByIdAsync(person.OrganizationId.Value);
            var housePartner = await SettingManager.GetSettingValueAsync(AppSettings.General.HouseOrganization);
            if (refOrg.Name == housePartner || (refOrg.ParentId.HasValue && refOrg.Parent.Name == housePartner))
            {
                var operationLegacyId = await _csiManager.InsertInterventiVolontariAsync(personId, person.LegacyId.Value, latitude, longitude, activityName, timestamp, status == ActionStatusType.Off ? AppConsts.CSI_OFFLINE : AppConsts.CSI_ACTIVITY, person.CurrentOperationLegacyId);
                if (operationLegacyId > 0)
                {
                    if (status == ActionStatusType.Off)
                        person.CurrentOperationLegacyId = null;
                    else if (status == ActionStatusType.Active)
                        person.CurrentOperationLegacyId = operationLegacyId;
                }
                else
                {
                    Logger.ErrorFormat("####CreateInterventionAsync failed for personId:{0} at Timestamp {1}", personId, timestamp);
                }
            }
        }
    }
}
