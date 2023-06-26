using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using NetTopologySuite.Geometries;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ermes.Stations
{
    [Table("stations")]
    public class Station: Entity ,IHasCreationTime
    {
        [Required]
        public string SensorServiceId { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public string Brand { get; set; }
        public string StationType { get; set; }
        [Required]
        public Point Location { get; set; }
        public decimal Altitude { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
