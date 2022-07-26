using Ermes.Dto;
using Ermes.Dto.Spatial;
using Ermes.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Missions.Dto
{
    public class MissionNotificationTestDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public int Id { get; set; }
        public MissionStatusType Status { get; set; }
        public string TopicName { get; set; }
    }
}
