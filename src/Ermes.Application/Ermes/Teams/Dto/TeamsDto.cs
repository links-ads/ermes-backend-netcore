﻿using Ermes.Dto.Datatable;
using Ermes.Organizations.Dto;
using Ermes.Users.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ermes.Teams.Dto
{
    public class SetTeamMembersInput
    {
        [Required]
        public int TeamId { get; set; }
        [Required]
        public List<Guid> MembersGuids { get; set; }
    }
    public class TeamDto
    {
        public int Id { get; set; }
        [Required]
        [StringLength(Team.MAX_TEAMNAME_LENGTH)]
        public string Name { get; set; }
        public int? OrganizationId { get; set; }
        public OrganizationDto Organization { get; set; }
    }

    public class CreateUpdateTeamInput
    {
        [Required]
        public TeamDto Team { get; set; }
    }

    public class TeamOutputDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public OrganizationDto Organization { get; set; }
        public List<ListUsernamesDto> Members { get; set; }
    }
    public class GetTeamsInput : DTPagedSortedAndFilteredInputDto { }

}
