using Ermes.Gamification;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ermes.Gamification
{
    /// <summary>
    /// Awards are reassigned monthly and represent a way to reward most active users with additional points and temporary statuses
    /// </summary>
    public class Award: Reward
    {
        public const int MaxDescriptionLength = 1000;

        public int Points { get; set; }

        [StringLength(MaxDescriptionLength)]
        public string Description { get; set; }

    }
}
