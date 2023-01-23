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

        public GamificationBaseDto(int points, int? levelId, string levelName, int earnedPoints)
        {
            Points = points;
            LevelId = levelId;
            LevelName = levelName;
            EarnedPoints = earnedPoints;
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
        public int EarnedPoints { get; set; }
    }
}
