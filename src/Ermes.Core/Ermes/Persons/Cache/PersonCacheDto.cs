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
            // Should use automap here
            this.Id = p.Id;
            this.OrganizationId = p.OrganizationId;
            this.RegistrationToken = p.RegistrationToken;
            this.TeamId = p.TeamId;
            this.Username = p.Username;
        }
        public long Id { get; private set; }
        public int? OrganizationId { get; private set; }
        public int? TeamId { get; private set; }
        public string Username { get; private set; }
        public string RegistrationToken { get; private set; }

    }
}
