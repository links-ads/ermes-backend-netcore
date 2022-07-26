using Abp.AutoMapper;
using Ermes.Dto.Spatial;
using Ermes.Enums;
using Ermes.Persons;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Actions.Dto
{
    [AutoMap(typeof(PersonAction), typeof(PersonActionTracking), typeof(PersonActionActivity), typeof(PersonActionStatus))]
    public class PersonActionDto
    {
        public int Id { get; set; }
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }
        public PointPosition Location { get {
                if (Longitude.HasValue && Latitude.HasValue)
                    return new PointPosition(Longitude.Value, Latitude.Value);
                else
                    return null;
            } 
        }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime Timestamp { get; set; }
        public string ExtensionData { get; set; }
        public ActionStatusType Status { get; set; }
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public int ActivityId { get; set; }
        public string ActivityName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string DisplayName
        {
            get { return Username ?? Email; }
            set { DisplayName = value; }
        }
        public PersonActionType Type { get; set; }
        public long PersonId { get; set; }
        public int TeamId { get; set; }
    }
}
