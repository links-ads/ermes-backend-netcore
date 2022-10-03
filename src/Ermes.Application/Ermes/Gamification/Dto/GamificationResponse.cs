using Ermes.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Gamification.Dto
{
    public class GamificationResponse
    {
        public GamificationBaseDto Gamification { get; set; }
        public ResponseBaseDto Response { get; set; }
    }
}
