using System;
using System.Collections.Generic;
using System.Text;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ermes.Persons;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Ermes.Enums;
using Microsoft.EntityFrameworkCore;
using Ermes.Helpers;
using Abp.Auditing;

namespace Ermes.Preferences
{

    // TODO_DS: PK should be <UserId, Source>
    [Table("preferences")]
    public class Preference: AuditedEntity<(long, String)>
    {
        public override (long, string) Id { get { return (PreferenceOwnerId, SourceString); } set { } }
        private Person _preferenceOwner;
        // ForeignKey-> PersonId set in DbContext
        public Person PreferenceOwner { get { return _preferenceOwner; } set { _preferenceOwner = value; PreferenceOwnerId = value.Id; } }
        public long PreferenceOwnerId { get; set; }

        [Required, Column("Source")]
        public string SourceString
        {
            get { return Source.ToString(); }
            private set { Source = value.ParseEnum<SourceDeviceType>(); }
        }
        [NotMapped]
        public SourceDeviceType Source { get; set; } 

        [Required, Column(TypeName = "jsonb")]
        public string Details { get; set; }
    }
}
