using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Users.Dto
{
    public class ListUsernamesDto
    {
        public long Id { get; set; }
        public Guid FusionAuthUserGuid { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string DisplayName { get {
                return Username ?? Email;
            } 
        }
    }
}
