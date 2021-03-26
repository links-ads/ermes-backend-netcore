using System;
using System.Collections.Generic;

namespace Ermes.Auth.Dto
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string BirthDate { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string FullName { get; set; }
        public string ImageUrl { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string MobilePhone { get; set; }
        public List<string> PreferredLanguages { get; set; }
        public string Username { get; set; }
        //public string Password { get; set; }
        public string[] Roles { get; set; }
        public string Timezone { get; set; }
    }
}
