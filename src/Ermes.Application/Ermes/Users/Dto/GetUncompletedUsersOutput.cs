using Ermes.Auth.Dto;
using Ermes.Profile.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Users.Dto
{
    public class GetUncompletedUsersOutput
    {
        public List<ProfileDto> UserList { get; set; }
    }
}
