using Ermes.Actions.Dto;
using Ermes.Enums;
using Ermes.Reports.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Dashboard.Dto
{
    public class GetStatisticsOutput
    {
        public List<GroupDto> ReportsByHazard { get; set; }
        public List<GroupDto> MissionsByStatus { get; set; }
        public List<GroupDto> PersonsByStatus { get; set; }
        public List<PersonActionDto> Persons { get; set; }
    }
}
