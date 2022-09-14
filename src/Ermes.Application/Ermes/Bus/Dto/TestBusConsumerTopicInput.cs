using Ermes.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Bus.Dto
{
    public class TestBusConsumerTopicInput
    {
        public string TopicName { get; set; }
        public int MissionId { get; set; }
        public MissionStatusType Status { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public EntityWriteAction Action { get; set; }
        public string NewRewardName { get; set; }
    }
}
