using Ermes.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Consumers
{
    public class MissionChangeStatusDto
    {
        public int Id { get; set; }
        public MissionStatusType Status { get; set; }
        public string Username { get; set; } = "ar.user";
    }
}
