using Abp.AutoMapper;
using Ermes.Categories;
using Ermes.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Reports.Dto
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public CategoryType Type { get; set; }
        public HazardType Hazard { get; set; }
        public string TargetKey { get; set; }
        public string Target { get; set; }
        public string Group { get; set; }
        //Key value for Group, not influenced by localization
        public string GroupKey { get; set; }
        public string SubGroup { get; set; }
        //Key value for SubGroup, not influenced by localization
        public string SubGroupKey { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string GroupCode { get; set; }
        public string UnitOfMeasure { get; set; }
        public string MinValue { get; set; }
        public string MaxValue { get; set; }
        public string[] Values { get; set; }
        public string GroupIcon { get; set; }
        public FieldType FieldType { get; set; }
    }
}
