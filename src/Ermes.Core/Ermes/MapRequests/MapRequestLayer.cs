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
        public const int MaxErrorMessageLength = 2000;

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

        [StringLength(MaxErrorMessageLength)]
        public string ErrorMessage { get; set; }
    }
}
