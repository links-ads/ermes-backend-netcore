using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Auth.Dto
{
    public class JWTRefreshTokenRevokeEventDto
    {
        public UserDto User { get; set; }
        public Guid? UserId { get; set; }
        public Guid? ApplicationId { get; set; }
    }
}
