using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Gamification.Dto
{
    public class BaseGamificationDto
    {
        public bool Success { get; set; }
        public int Points { get; set; }
        public int? LevelId { get; set; }
    }
}
