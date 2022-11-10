using System;
using System.Collections.Generic;

namespace Ermes.Teams.Dto
{
    public class TeamNotificationDto
    {
        public string Name { get; set; }
        public List<Guid> Guids { get; set; }
    }
}
