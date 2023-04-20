using Abp.Domain.Entities.Auditing;
using Ermes.Persons;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.Reports
{
    [Table("report_validations")]
    public class ReportValidation: AuditedEntity
    {
        public const int MaxNotesLength = 1000;

        [Required]
        [ForeignKey("PersonId")]
        public virtual Person Person { get; set; }
        public long PersonId { get; set; }

        [Required]
        [ForeignKey("ReportId")]
        public virtual Report Report { get; set; }
        public int ReportId { get; set; }
        public bool IsValid { get; set; }
        [StringLength(MaxNotesLength)]
        public string RejectionNote { get; set; } 
    }
}
