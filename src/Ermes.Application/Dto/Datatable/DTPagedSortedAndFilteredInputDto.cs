using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ermes.Dto.Datatable
{
    public class DTPagedSortedAndFilteredInputDto : IPagedResultRequest, IDTParameters
    {

        public DTPagedSortedAndFilteredInputDto()
        {
            Search = new DTSearch();
            Order = new List<DTOrder>();
        }

        [Required]
        [Range(1, AppConsts.MaxPageSize)]
        public int MaxResultCount { get; set; }

        [Range(0, int.MaxValue)]
        public int SkipCount { get; set; }

        public int Draw { get; set; }

        public DTSearch Search { get; set; }

        public List<DTOrder> Order { get; set; }

    }
}
