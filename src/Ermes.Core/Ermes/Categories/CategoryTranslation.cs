using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ermes.Categories
{
    [Table("category_translations")]
    public class CategoryTranslation : Entity, IEntityTranslation<Category>
    {
        public const int MaxDescriptionLength = 500;
        public const int MaxNameLength = 255;
        public const int MaxGroupLength = 100;
        public const int MaxSubGroupLength = 500;

        [Required]
        [StringLength(MaxGroupLength)]
        public string Group { get; set; }

        [Required]
        [StringLength(MaxSubGroupLength)]
        public string SubGroup { get; set; }

        [Required]
        [StringLength(MaxNameLength)]
        public string Name { get; set; }
        
        [StringLength(MaxDescriptionLength)]
        public string Description { get; set; }

        public string[] Values { get; set; }

        public Category Core { get; set; }
        public int CoreId { get; set; }
        public string Language { get; set; }

    }
}
