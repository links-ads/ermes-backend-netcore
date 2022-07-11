using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Gamification.Dto
{
    public class GetLeaderboardOutput
    {
        public GetLeaderboardOutput()
        {
            Competitors = new List<GamificationBaseDto>();
        }
        public List<GamificationBaseDto> Competitors { get; set; }
    }
}
