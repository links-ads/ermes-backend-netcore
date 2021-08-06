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
        public const int MaxCodeLength = 100;
        public const int MaxGroupCodeLength = 100;
        public const int MaxGroupKeyLength = 100;
        public const int MaxSubGroupKeyLength = 255;

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
        [StringLength(MaxCodeLength)]
        public string Code { get; set; }
        [Required]
        [StringLength(MaxGroupCodeLength)]
        public string GroupCode { get; set; }

        public ICollection<CategoryTranslation> Translations { get; set; }

        #region Numeric
        public string MinValue { get; set; }
        public string MaxValue { get; set; }
        #endregion

        public string GroupIcon { get; set; }

        [Required]
        [Column("TargetKey")]
        public string TargetKeyString
        {
            get { return TargetKey.ToString(); }
            private set { TargetKey = value.ParseEnum<TargetType>(); }
        }
        [NotMapped]
        public TargetType TargetKey { get; set; }

        [Required]
        [StringLength(MaxGroupKeyLength)]
        public string GroupKey { get; set; }

        [StringLength(MaxSubGroupKeyLength)]
        public string SubGroupKey { get; set; }

        [Column("FieldType")]
        public string FieldTypeString
        {
            get { return FieldType.ToString(); }
            private set { FieldType = value.ParseEnum<FieldType>(); }
        }

        [NotMapped]
        public FieldType FieldType { get; set; }
    }
}
