using Abp.Domain.Entities;
using Ermes.Enums;
using Ermes.Helpers;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.CompetenceAreas
{
    [Table("competence_areas")]
    public class CompetenceArea : Entity
    {
        public const int MaxUuidLength = 255;

        public CompetenceArea(){}
        public CompetenceArea(string uuid, string name, string competenceAreaTypeString, Geometry aoi)
        {
            Uuid = uuid;
            Name = name;
            CompetenceAreaTypeString = competenceAreaTypeString;
            AreaOfInterest = aoi;
        }

        [StringLength(MaxUuidLength)]
        public string Uuid { get; set; }

        [Required]
        public string Name { get; set; }
        
        [Required]
        [Column("CompetenceAreaType")]
        public string CompetenceAreaTypeString {
            get { return CompetenceAreaType.ToString(); }
            private set { CompetenceAreaType = value.ParseEnum<CompetenceAreaType>(); }
        }

        [NotMapped]
        public CompetenceAreaType CompetenceAreaType { get; set; }

        [Required]
        [Column(TypeName = "geography")]
        public Geometry AreaOfInterest { get; set; }
        
        public string Source { get; set; }
        
        [Column(TypeName = "jsonb")]
        public string Metadata { get; set; }
    }
}