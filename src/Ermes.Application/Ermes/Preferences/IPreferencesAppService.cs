using Abp.Application.Services;
using System.Threading.Tasks;

namespace Ermes.Preferences
{
    public interface IPreferencesAppService:IApplicationService
    {
        Task<GetPreferenceOutput> GetPreference(GetPreferenceInput input);
        Task<bool> CreateOrUpdatePreference(CreateOrUpdatePreferenceInput preference);
    }
}
