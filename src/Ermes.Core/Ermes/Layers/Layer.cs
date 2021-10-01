using Abp.Domain.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ermes.Layers
{
    [Table("layers")]
    public class Layer: Entity, IMultiLingualEntity<LayerTranslation>
    {
        public const int MaxGroupLength = 255;
        public const int MaxPartnerNameLength = 100;

        public int DataTypeId { get; set; }

        [Required]
        [StringLength(MaxGroupLength)]
        public string GroupKey { get; set; }

        [Required]
        [StringLength(MaxGroupLength)]
        public string SubGroupKey { get; set; }

        [StringLength(MaxPartnerNameLength)]
        public string PartnerName { get; set; }

        public ICollection<LayerTranslation> Translations { get; set; }

    }
}
