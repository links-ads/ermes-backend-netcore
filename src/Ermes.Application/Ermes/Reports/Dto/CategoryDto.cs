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
        public string Group { get; set; }
        public string SubGroup { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string GroupCode { get; set; }
        public string Description { get; set; }
        public string UnitOfMeasure { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public string[] Values { get; set; }
        public string[] StatusValues { get; set; }
    }
}
