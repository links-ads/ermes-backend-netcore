using Abp.Application.Services;
using Ermes.Dto;
using Ermes.Dto.Datatable;
using Ermes.Organizations.Dto;
using Ermes.Profile.Dto;
using System;
using System.Threading.Tasks;

namespace Profile
{
    public interface IProfileAppService : IApplicationService
    {
        Task<GetProfileOutput> GetProfile();
        Task<GetProfileOutput> GetProfileById(IdInput<long> input);
        Task<UpdateProfileOutput> UpdateProfile(UpdateProfileInput input);
        //Task<bool> UpdatePreferredLanguages(UpdatePreferredLanguagesInput input);
        Task<bool> DeleteProfile(IdInput<Guid> input);
        Task<bool> UpdateRegistrationToken(UpdateRegistrationTokenInput input);
        Task<DTResult<PersonDto>> GetOrganizationMembers(GetOrganizationMembersInput input);
        Task<DTResult<OrganizationDto>> GetOrganizations(GetOrganizationsInput input);
        Task<GetProfileOutput> ChangeOrganization(ChangeOrganizationInput input);
        ///TDB
        ///GetRemoteSettings
        ///UpdateRemoteSettings
    }
}
