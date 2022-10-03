using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Gamification.Dto
{
    public class LevelDto
    {
        public string Name { get; set; }
        public int LowerBound { get; set; }
        public int UpperBound { get; set; }
        public string PreviousLevelName { get; set; }
        public string FollowingLevelName { get; set; }
    }
}
