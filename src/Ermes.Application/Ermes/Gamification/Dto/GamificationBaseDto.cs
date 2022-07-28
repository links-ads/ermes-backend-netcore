using Ermes.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Gamification.Dto
{
    public class GamificationBaseDto
    {
        public GamificationBaseDto(int points, int? levelId)
        {
            Points = points;
            LevelId = levelId;
        }
        public int Points { get; set; }
        public int? LevelId { get; set; }
        public string LevelName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string DisplayName
        {
            get
            {
                return Username ?? Email;
            }
        }
        public int Position { get; set; }
    }
}
