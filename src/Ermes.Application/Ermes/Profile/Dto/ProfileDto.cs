﻿using Ermes.Auth.Dto;
using Ermes.Enums;
using Ermes.Gamification.Dto;
using Ermes.Missions.Dto;
using Ermes.Organizations.Dto;
using Ermes.Teams.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ermes.Profile.Dto
{
    public class ProfileDto
    {
        [Required]
        public UserDto User { get; set; }
        public OrganizationDto Organization { get; set; }
        public TeamDto Team { get; set; }
        public int CurrentActivityId { get; set; }
        public ActionStatusType CurrentStatus { get; set; }
        public long PersonId { get; set; }
        public Location? Location { get; set; }
        public List<MissionDto> CurrentMissions { get; set; }
        public bool IsFirstLogin { get; set; }
        public bool IsNewUser { get; set; }
        public int? LegacyId { get; set; }
        public string TaxCode { get; set; }
        public int Points { get; set; }
        public string Level { get; set; }
        public List<MedalDto> Medals { get; set; }
        public List<BadgeDto> Badges { get; set; }
        public string[] Permissions { get; set; }

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
