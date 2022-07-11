using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Gamification.Dto
{
    public class GetLeaderboardOutput
    {
        public List<GamificationBaseDto> Competitors { get; set; }
    }
}
