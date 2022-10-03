using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Persons
{
    public interface IPersonBase
    {
        public long Id { get; }
        public int? OrganizationId { get; }
        public int? TeamId { get; }
        public string Username { get; }
        public string Email { get; }
        public string RegistrationToken { get; }
    }
}
