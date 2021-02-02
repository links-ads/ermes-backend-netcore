using Abp.AutoMapper;
using io.fusionauth.domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Auth.Dto
{
    [AutoMap(typeof(UserRegistration))]
    public class UserRegistrationDto
    {
        [JsonIgnore]
        public Guid? ApplicationId { get; set; }
        public List<string> Roles { get; set; }
        [JsonIgnore]
        public bool? Verified { get; set; } = true;
    }
}
