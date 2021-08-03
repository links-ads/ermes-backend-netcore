using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Gamification.Dto
{
    public class QuizDto
    {
        public string Text { get; set; }
        public string CrisisPhase { get; set; }
        public string EventContext { get; set; }
        public string Hazard { get; set; }
        public string Difficulty { get; set; }
        public TipDto Tip { get; set; }
    }
}
