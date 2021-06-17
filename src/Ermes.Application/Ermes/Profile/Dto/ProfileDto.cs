using Ermes.Auth.Dto;
using Ermes.Enums;
using Ermes.Missions.Dto;
using Ermes.Organizations.Dto;
using Ermes.Teams.Dto;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ermes.Profile.Dto
{
    public class ProfileDto
    {
        [Required]
        public UserDto User { get; set; }
        public OrganizationDto Organization { get; set; }
        public TeamDto Team { get; set; }
        public int CurrentActivityId  { get; set; }
        public ActionStatusType CurrentStatus { get; set; }
        public long PersonId { get; set; }
        public Location? Location { get; set; }
        public List<MissionDto> CurrentMissions { get; set; }
        public bool IsFirstLogin { get; set; }

    }

    public struct Location
    {
        public Location(double longitude, double latitude, DateTime timestamp)
        {
            Latitude = latitude;
            Longitude = longitude;
            Timestamp = timestamp.ToUniversalTime();
        }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
