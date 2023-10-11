using Abp.Runtime.Validation;
using Ermes.Dto;
using Ermes.Dto.Spatial;
using Ermes.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Ermes.Communications.Dto
{
    public class CommunicationDto : ICustomValidate
    {
        public int Id { get; set; }
        public string Type { get; } = "Communication";
        [Required]
        public string Message { get; set; }
        public RangeDto<DateTime> Duration { get; set; }
        public PointPosition Centroid { get; set; }
        public string OrganizationName { get; set; }
        public CommunicationScopeType Scope { get; set; }
        public CommunicationRestrictionType Restriction { get; set; }

        /// <summary>
        /// List of OrganizationIds whose member will receive the communication.
        /// It's a mandatory field when Restriction is set to 'Organization'
        /// </summary>
        public List<int> OrganizationReceiverIds { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (Restriction == CommunicationRestrictionType.Organization && (OrganizationReceiverIds == null || OrganizationReceiverIds.Count == 0 || OrganizationReceiverIds.Where(a => a == 0).Count() > 0))
                context.Results.Add(new ValidationResult("When the Restriction is set to 'Organization', a list of receiver organization must be specified"));
        }
    }
}
