using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using NetTopologySuite.Geometries;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ermes.Alerts
{
    [Table("alert_areasofinterest")]
    public class AlertAreaOfInterest : Entity, IHasCreationTime
    {
        public AlertAreaOfInterest()
        {
                
        }

        public AlertAreaOfInterest(int alertId, Geometry aoi)
        {
            AlertId = alertId;
            AreaOfInterest = aoi;
        }

        [ForeignKey("AlertId")]
        public virtual Alert Alert { get; set; }
        public int AlertId { get; set; }


        [Required]
        [Column(TypeName = "geography")]
        public Geometry AreaOfInterest { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
