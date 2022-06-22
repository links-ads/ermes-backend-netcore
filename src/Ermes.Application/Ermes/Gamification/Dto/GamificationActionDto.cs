using Ermes.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Gamification.Dto
{
    public class GamificationActionDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public CompetenceType Competence { get; set; }
        public int Points { get; set; }
    }
}
