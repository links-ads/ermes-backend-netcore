using Ermes.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Gamification.Dto
{
    public  class GamificationNotificationDto
    {
        public long PersonId { get; set; }
        public string ActionName { get; set; }
        public string NewValue { get; set; }
    }
}
