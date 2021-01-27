using Abp.Domain.Entities;
using Ermes.Enums;
using Ermes.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.Categories
{
    [Table("categories")]
    public class Category: Entity, IMultiLingualEntity<CategoryTranslation>
    {    
        [Required]
        [Column("Type")]
        public string TypeString
        {
            get { return Type.ToString(); }
            private set { Type = value.ParseEnum<CategoryType>(); }
        }

        [NotMapped]
        public CategoryType Type { get; set; }

        [Required]
        [Column("Hazard")]
        public string HazardString
        {
            get { return Hazard.ToString(); }
            private set { Hazard = value.ParseEnum<HazardType>(); }
        }

        [NotMapped]
        public HazardType Hazard { get; set; }

        [Required]
        public string Code { get; set; }
        [Required]
        public string GroupCode { get; set; }

        public ICollection<CategoryTranslation> Translations { get; set; }

        #region Numeric
        public string UnitOfMeasure { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        #endregion

        #region Range
        public string[] StatusValues { get; set; }
        #endregion
    }
}
