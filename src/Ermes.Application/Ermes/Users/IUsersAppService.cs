using Ermes.Dto.Datatable;
using Ermes.Interfaces;
using Ermes.Profile.Dto;
using Ermes.Users.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Users
{
    public interface IUsersAppService : IBackofficeApi
    {
        Task<DTResult<ProfileDto>> GetUsers(GetUsersInput input);
        Task<CreateOrUpdateUserOutput> CreateOrUpdateUser(UpdateProfileInput input);
        Task RegisterUser(RegisterUserEventInput input);
    }
}
