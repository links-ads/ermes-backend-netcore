using Ermes.Actions.Dto;
using Ermes.Enums;
using Ermes.Stations.Dto;
using System.Collections.Generic;

namespace Ermes.Dashboard.Dto
{
    public class GetStatisticsOutput
    {
        public List<GroupDto> ReportsByHazard { get; set; }
        public List<GroupDto> MissionsByStatus { get; set; }
        public List<GroupDto> PersonsByStatus { get; set; }
        public List<PersonActionDto> Persons { get; set; }
        public Dictionary<ActionStatusType, List<ActivationDto>> ActivationsByDay { get; set; }
        public List<GroupDto> CommunicationsByRestriction { get; set; }
        public List<GroupDto> MapRequestByType { get; set; }
        public List<GroupDto> AlertsByRestriction { get; set; }
        public List<StationDto> Stations { get; set; }

    }
}
