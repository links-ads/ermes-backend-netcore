using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Events.Bus.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.EntityHistory
{
    [Table("entitypropertychanges")]
    public class SplitEntityPropertyChange : Entity<long>, IMayHaveTenant
    {
        public const int MaxPropertyNameLength = 96;
        public const int MaxValueLength = 512;
        public const int MaxPropertyTypeFullNameLength = 192;

        public virtual long EntityChangeId { get; set; }
        [StringLength(MaxValueLength)]
        public virtual string NewValue { get; set; }
        [StringLength(MaxValueLength)]
        public virtual string OriginalValue { get; set; }
        [StringLength(MaxPropertyNameLength)]
        public virtual string PropertyName { get; set; }
        [StringLength(MaxPropertyTypeFullNameLength)]
        public virtual string PropertyTypeFullName { get; set; }
        public virtual int? TenantId { get; set; }
        public int? SplitIndex { get; set; }
    }

    [Table("entitychanges")]
    public class SplitEntityChange: Entity<long>, IMayHaveTenant
    {
        public const int MaxEntityIdLength = 48;
        public const int MaxEntityTypeNameLength = 96;
        public DateTime ChangeTime { get; set; }
        public EntityChangeType ChangeType { get; set; }
        public long EntityChangeSetId { get; set; }
        [StringLength(MaxEntityIdLength)]
        public string EntityId { get; set; }
        [StringLength(MaxEntityTypeNameLength)]
        public string EntityTypeName { get; set; }
        public int? TenantId { get; set; }
        public ICollection<SplitEntityPropertyChange> PropertyChanges { get; set; }
        [NotMapped]
        public object EntityEntry { get; set; }
    }
    [Table("entitychangesets")]
    public class SplitEntityChangeSet : Entity<long>, IHasCreationTime, IMayHaveTenant, IExtendableObject
    {
        public const int MaxBrowserInfoLength = 512;
        public const int MaxClientNameLength = 128; 
        public const int MaxReasonLength = 256;

        [StringLength(MaxBrowserInfoLength)]
        public string BrowserInfo { get; set; }
        [StringLength(MaxClientNameLength)]
        public string ClientName { get; set; }
        public DateTime CreationTime { get; set; }
        public string ExtensionData { get; set; }
        public int? ImpersonatorTenantId { get; set; }
        public long? ImpersonatorUserId { get; set; }
        [StringLength(MaxReasonLength)]
        public virtual string Reason { get; set; }
        public virtual int? TenantId { get; set; }
        public virtual long? UserId { get; set; }
        public virtual IList<SplitEntityChange> EntityChanges { get; set; } = new List<SplitEntityChange>();
    }
}
