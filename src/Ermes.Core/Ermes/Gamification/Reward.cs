using Abp.Domain.Entities;
using Ermes.Enums;
using Ermes.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ermes.Gamification
{
    /// <summary>
    /// There are two kind of rewards: Achievements and Awards.
    /// The main difference between them is that the formers are gained and retained forever, 
    /// while the latter are assigned with a given periodicity, thus they can be gained and loosed.
    /// </summary>
    [Table("gamification_rewards")]
    public class Reward: Entity
    {
        public const int MaxNameLength = 1000;

        [Column("Competence")]
        public string CompetenceString
        {
            get { return Competence.ToString(); }
            private set { Competence = value.ParseEnum<CompetenceType>(); }
        }
        [NotMapped]
        public CompetenceType Competence { get; set; }

        [Required]
        [StringLength(MaxNameLength)]
        public string Name { get; set; }
        public virtual Barrier Barrier { get; set; }
    }

    public class GamificationDetail
    {
        public int Threshold { get; set; }
        public int Points { get; set; }
    }
}
