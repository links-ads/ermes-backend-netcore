using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Ermes.Enums;
using Ermes.Helpers;
using Ermes.Layers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.MapRequests
{
    [Table("map_request_layers")]

    public class MapRequestLayer: CreationAuditedEntity
    {
        public virtual Layer Layer { get; set; }
        public int LayerDataTypeId { get; set; }

        public virtual MapRequest MapRequest { get; set; }
        public string MapRequestCode { get; set; }

        [Column("Status")]
        public string StatusString
        {
            get { return Status.ToString(); }
            private set { Status = value.ParseEnum<LayerImportStatusType>(); }
        }
        [NotMapped]
        public LayerImportStatusType Status { get; set; }

        public int ReceivedUpdates { get; set; }

        [Column(TypeName = "jsonb")]
        public List<MapRequestLayerError> ErrorMessages { get; set; }
    }

    public class MapRequestLayerError
    {
        public MapRequestLayerError()
        {

        }
        public MapRequestLayerError(string message, DateTime acquisitionDate)
        {
            Message = message;
            AcquisitionDate = acquisitionDate;
        }
        public string Message { get; set; }
        public DateTime AcquisitionDate { get; set; }
    }
}
