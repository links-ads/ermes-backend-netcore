using Ermes.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Gamification.Dto
{
    public class GamificationBaseDto
    {
        public int Points { get; set; }
        public int? LevelId { get; set; }
        public string LevelName { get; set; }
        public string Username { get; set; }
    }
}
