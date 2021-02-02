using Abp.UI;
using Ermes.Attributes;
using Ermes.Helpers;
using Ermes.Persons;
using Newtonsoft.Json;
using NSwag.Annotations;
using System;
using System.Threading.Tasks;

namespace Ermes.Preferences
{
    [ErmesAuthorize]
    public class PreferencesAppService: ErmesAppServiceBase, IPreferencesAppService
    {
        private readonly PreferenceManager _preferenceManager;
        private readonly PersonManager _personManager;
        private readonly ErmesAppSession _session;
        public PreferencesAppService(PreferenceManager preferenceManager, PersonManager personManager, ErmesAppSession session)
        {
            _preferenceManager = preferenceManager;
            _personManager = personManager;
            _session = session;
        }
        [OpenApiOperation("Get Preferences",
            @"
                Get the preference data for the current user and the chosen device type.
                Input: 
                    - Source: device type string/enum
                Output: GetPreferenceOutput object containing PreferenceDto object
            "
        )]
        public virtual async Task<GetPreferenceOutput> GetPreference(GetPreferenceInput request)
        {
            Preference preference = await _preferenceManager.GetPreferenceAsync(_session.UserId.Value, request.Source);
            return new GetPreferenceOutput()
            {
                Preference = ObjectMapper.Map<PreferenceDto>(preference)
            };
        }
        [OpenApiOperation("Save Preferences",
            @"
                Save the preference data for the current user and the chiosen device type. Overwrites the current data if already exists an entry for the same user and device type.
                Input:
                    - Source: device type string/enum
                    - Details: jsonb string with preference data (null-valued if an entry for the user and the device type does not exist)
            "
        )]
        public virtual async Task<bool> CreateOrUpdatePreference(CreateOrUpdatePreferenceInput input)
        {
            try
            {
                JsonConvert.DeserializeObject(input.Preference.Details);
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(L("InvalidJson", e.Message));
            }
            Preference stored_preference = await _preferenceManager.GetPreferenceAsync(_session.UserId.Value, input.Preference.Source);
            if(stored_preference == null)
            {
                stored_preference = ObjectMapper.Map<Preference>(input.Preference);
                stored_preference.PreferenceOwnerId = _session.UserId.Value;
                await _preferenceManager.InsertPreference(stored_preference);
            }
            else
            {
                ObjectMapper.Map(input.Preference, stored_preference); // Implicit update. The modified object is saved on db at function exit
            }

            return true;
        }
    }
}
