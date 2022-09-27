using Ermes.Organizations;
using Ermes.Teams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Persons.Cache
{
    public class PersonCacheDto: IPersonBase
    {
        public PersonCacheDto(Person p)
        {
            if (p != null)
            {
                // Should use automap here
                Id = p.Id;
                OrganizationId = p.OrganizationId;
                RegistrationToken = p.RegistrationToken;
                TeamId = p.TeamId;
                Username = p.Username;
                Email = p.Email;
            }
        }
        public long Id { get; private set; }
        public int? OrganizationId { get; private set; }
        public int? TeamId { get; private set; }
        public string Username { get; private set; }
        public string Email { get; private set; }
        public string RegistrationToken { get; private set; }

    }
}
