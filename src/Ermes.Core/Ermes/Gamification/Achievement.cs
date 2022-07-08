using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.Gamification
{
    /// <summary>
    /// Achievements are a virtual or physical representation of having accomplished something, and they are based on incremental and quantitative criteria. 
    /// Achievements are a way to give players visibility about what they have accomplished as well as to add challenges to the overall experience. 
    /// In ERMES there are two types of achievements: Medals and Badges.
    /// They are used to define barriers that controls the progressive access from one level to the next one
    /// </summary>
    public class Achievement: Reward
    {
        [Column(TypeName = "jsonb")]
        public GamificationDetail Detail { get; set; }

        [ForeignKey("GamificationActionCode")]
        public virtual GamificationAction GamificationAction { get; set; }
        public string GamificationActionCode { get; set; }
    }
}
