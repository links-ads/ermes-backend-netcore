using Castle.MicroKernel.SubSystems.Conversion;
using Ermes.Enums;
using Ermes.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.Gamification
{
    /// <summary>
    /// A Medal reinforces and rewards a specific activity, it is Competence-driven
    /// </summary>
    public class Medal: Achievement
    {

        [Column("Type")]
        public string TypeString
        {
            get { return Type.ToString(); }
            private set { Type = value.ParseEnum<MedalType>(); }
        }
        [NotMapped]
        public MedalType Type { get; set; }
    }
}
