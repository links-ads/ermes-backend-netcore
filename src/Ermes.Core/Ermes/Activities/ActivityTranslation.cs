using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ermes.Activities
{
    [Table("activity_translations")]
    public class ActivityTranslation : Entity, IEntityTranslation<Activity>
    {
        public const int MaxNameLength = 255;

        [StringLength(MaxNameLength)]
        public string Name { get; set; }
        public Activity Core { get; set; }
        public int CoreId { get; set; }
        public string Language { get; set; }
    }
}
