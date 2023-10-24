using Abp.Runtime.Validation;
using Ermes.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ermes.Profile.Dto
{
    public class ChangeOrganizationInput : ICustomValidate
    {
        public int OrganizationId { get; set; }
        public string TaxCode { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (OrganizationId == 0)
                context.Results.Add(new ValidationResult("Invalid Organization Id"));
        }
    }
}
