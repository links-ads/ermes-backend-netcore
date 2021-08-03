using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Gamification.Dto
{
    public class TipDto
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string CrisisPhase { get; set; }
        public string EventContext { get; set; }
        public string Hazard { get; set; }
        public List<QuizDto> Quizzes { get; set; }
    }
}
