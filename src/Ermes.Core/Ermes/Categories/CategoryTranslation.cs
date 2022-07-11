using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ermes.Categories
{
    [Table("category_translations")]
    public class CategoryTranslation : Entity, IEntityTranslation<Category>
    {
        public const int MaxNameLength = 255;
        public const int MaxGroupLength = 100;
        public const int MaxSubGroupLength = 255;
        public const int MaxLanguageLength = 2;

        [Required]
        [StringLength(MaxGroupLength)]
        public string Group { get; set; }

        [StringLength(MaxSubGroupLength)]
        public string SubGroup { get; set; }

        [Required]
        [StringLength(MaxNameLength)]
        public string Name { get; set; }
        public string[] Values { get; set; }

        public Category Core { get; set; }
        public int CoreId { get; set; }
        [StringLength(MaxLanguageLength)]
        public string Language { get; set; }
        public string UnitOfMeasure { get; set; }
        public string Target { get; set; }

    }
}
