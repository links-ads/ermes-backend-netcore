using Abp.Events.Bus.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;

namespace Ermes.Logging.Dto
{
    public class EntitySelectDto
    {
        [Required]
        public string EntityTypeName { get; set; }
        [Required]
        public string Id { get; set; }
    }

    public class EntityHistoriesInputDto
    {
        public List<String> EntityTypeNames { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class ChangeInfoDto
    {
        public DateTime ChangeTime { get; set; } 
        public string EntityId { get; set; }
        public String ChangeType { get; set; }
        public string EntityTypeName { get; set; }
    }

    public class ChangeAuthorDto
    {
        public string BrowserInfo { get; set; }
        public string Reason { get; set; }
        public long UserId { get; set;  }
    }

    [DataContract]
    public class ChangeDto
    {
        [DataMember]
        public string NewValue { get; set; }
        [DataMember]
        public string OriginalValue { get; set; }
        [DataMember]
        public string PropertyName { get; set; }
        public int? SplitIndex { get; set; } // To assist in truncation reconstruction
    }

    public class EntityHistoryOutputItemDto
    {
        public List<ChangeDto> Changes { get; set; }
        public ChangeAuthorDto ChangeAuthor { get; set; }
        public ChangeInfoDto ChangeInfo { get; set; }
    }

    public class EntityHistoryOutputDto
    {
        public List<EntityHistoryOutputItemDto> HistoryItems { get; set; }
    }
}
