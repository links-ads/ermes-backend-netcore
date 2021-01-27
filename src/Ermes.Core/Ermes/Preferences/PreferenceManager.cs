using Abp.Domain.Repositories;
using Abp.Domain.Services;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using Ermes.Persons;
using Ermes.Enums;
using Abp.Extensions;

namespace Ermes.Preferences
{
    public class PreferenceManager : DomainService
    {
        protected IRepository<Preference, (long, String)> PreferenceRepository;
        public PreferenceManager(IRepository<Preference, (long, String)> preferenceRepository)
        {
            PreferenceRepository = preferenceRepository;
        }

        public async Task InsertPreference(Preference preference)
        {
            await PreferenceRepository.InsertAsync(preference);
        }

        public async Task<Preference> GetPreferenceAsync(long personId, SourceDeviceType source)
        {
            return await PreferenceRepository.FirstOrDefaultAsync(p => p.PreferenceOwnerId == personId && p.SourceString == source.ToString());
        }
    }
}
