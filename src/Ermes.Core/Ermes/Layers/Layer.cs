﻿using Abp.Domain.Entities;
using Ermes.Enums;
using Ermes.Helpers;
using Ermes.MapRequests;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ermes.Layers
{
    [Table("layers")]
    public class Layer: Entity, IPassivable, IMultiLingualEntity<LayerTranslation>
    {
        public const int MaxGroupLength = 255;
        public const int MaxPartnerNameLength = 100;
        public const int MaxUnitOfMeasureLength = 20;

        public int DataTypeId { get; set; }

        [Required]
        [StringLength(MaxGroupLength)]
        public string GroupKey { get; set; }

        [Required]
        [StringLength(MaxGroupLength)]
        public string SubGroupKey { get; set; }

        [StringLength(MaxPartnerNameLength)]
        public string PartnerName { get; set; }

        public bool CanBeVisualized { get; set; }
        [Column("Format")]
        public string FormatString
        {
            get { return Format.ToString(); }
            private set { Format = value.ParseEnum<FormatType>(); }
        }
        [NotMapped]
        public FormatType Format { get; set; }

        [Column("Frequency")]
        public string FrequencyString
        {
            get { return Frequency.ToString(); }
            private set { Frequency = value.ParseEnum<FrequencyType>(); }
        }
        [NotMapped]
        public FrequencyType Frequency { get; set; }

        public ICollection<LayerTranslation> Translations { get; set; }

        [StringLength(MaxUnitOfMeasureLength)]
        public string UnitOfMeasure { get; set; }

        public virtual ICollection<MapRequestLayer> MapRequestLayers { get; set; }

        public int Order { get; set; }

        /// <summary>
        /// Associated layer
        /// </summary>
        public virtual Layer Parent { get; set; }
        public int? ParentDataTypeId { get; set; }

        public virtual ICollection<Layer> AssociatedLayers { get; set; }
        public bool IsActive { get; set; }
    }
}
