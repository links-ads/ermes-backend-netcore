﻿using Ermes.Enums;
using Ermes.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.Gamification
{
    /// <summary>
    /// Badge is related to a hazard category and can be earned from completing a full set of hazard specific activity providing evidence of the user’s specialization
    /// </summary>
    public class Badge: Achievement
    {
        [Column("Hazard")]
        public string HazardString
        {
            get { return Hazard.ToString(); }
            private set { Hazard = value.ParseEnum<HazardType>(); }
        }
        [NotMapped]
        public HazardType Hazard { get; set; }
    }
}
