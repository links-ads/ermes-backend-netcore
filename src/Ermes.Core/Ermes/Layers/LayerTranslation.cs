using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.Layers
{
    [Table("layer_translations")]
    public class LayerTranslation : Entity, IEntityTranslation<Layer>
    {
        public const int MaxNameLength = 255;
        public const int MaxGroupLength = 255;
        public const int MaxSubGroupLength = 255;

        public Layer Core { get; set; }
        public int CoreId { get; set; }
        public string Language { get; set; }

        [Required]
        [StringLength(MaxGroupLength)]
        public string Group { get; set; }

        [StringLength(MaxSubGroupLength)]
        public string SubGroup { get; set; }

        [Required]
        [StringLength(MaxNameLength)]
        public string Name { get; set; }
    }
}
