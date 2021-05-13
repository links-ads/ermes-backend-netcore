using Abp.Application.Services;
using Ermes.Dto;
using Ermes.Dto.Datatable;
using Ermes.Profile.Dto;
using System.Threading.Tasks;

namespace Profile
{
    public interface IProfileAppService : IApplicationService
    {
        Task<GetProfileOutput> GetProfile();
        Task<GetProfileOutput> GetProfileById(IdInput<long> input);
        Task<UpdateProfileOutput> UpdateProfile(UpdateProfileInput input);
        Task<bool> UpdatePreferredLanguages(UpdatePreferredLanguagesInput input);
        Task<bool> DeleteProfile(IdInput<int> input);
        Task<bool> UpdateRegistrationToken(UpdateRegistrationTokenInput input);
        Task<DTResult<PersonDto>> GetOrganizationMembers(GetOrganizationMembersInput input);
        ///TDB
        ///GetRemoteSettings
        ///UpdateRemoteSettings
    }
}
