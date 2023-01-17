using Abp.Domain.Entities;
using Ermes.Enums;
using Ermes.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.Activities
{
    [Table("activities")]
    public class Activity : Entity, IMultiLingualEntity<ActivityTranslation>
    {
        public const int MaxShortNameLength = 8;
        /// <summary>
        /// Activity short name
        /// </summary>
        [StringLength(MaxShortNameLength)]
        public string ShortName { get; set; }
        /// <summary>
        /// Activity name translations
        /// </summary>
        public ICollection<ActivityTranslation> Translations { get; set; }
        /// <summary>
        /// Sub-activity reference to parent activity
        /// </summary>
        [ForeignKey("ParentId")]
        public virtual Activity Parent { get; set; }
        public virtual int? ParentId { get; set; }

        [Column("Hazard")]
        public string HazardString
        {
            get { return Hazard.ToString(); }
            private set { Hazard = value.ParseEnum<HazardType>(); }
        }
        [NotMapped]
        public HazardType Hazard { get; set; }

        //If false, the corresponding row must not be displayed on clients
        //There's the necessity to keep old values for foreign key integrity
        public bool IsActive { get; set; }
    }
}
