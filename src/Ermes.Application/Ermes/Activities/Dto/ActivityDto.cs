using Abp.Application.Services.Dto;
using Ermes.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Activities.Dto
{
    public class ActivityDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int? ParentId { get; set; }
        public HazardType Hazard { get; set; }
    }
}
