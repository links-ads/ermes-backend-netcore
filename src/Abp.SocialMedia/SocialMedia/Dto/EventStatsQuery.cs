using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;

namespace Abp.SocialMedia.Dto
{
    public class EventStatsQuery
    {
        /// <summary>
        /// Defines Languages
        /// </summary>
        [System.Text.Json.Serialization.JsonConverter(typeof(StringEnumConverter))]
        public enum LanguagesEnum
        {
            /// <summary>
            /// Enum En for value: en
            /// </summary>
            [EnumMember(Value = "en")]
            En = 1,

            /// <summary>
            /// Enum It for value: it
            /// </summary>
            [EnumMember(Value = "it")]
            It = 2,

            /// <summary>
            /// Enum Es for value: es
            /// </summary>
            [EnumMember(Value = "es")]
            Es = 3,

            /// <summary>
            /// Enum Tr for value: tr
            /// </summary>
            [EnumMember(Value = "tr")]
            Tr = 4,

            /// <summary>
            /// Enum Hr for value: hr
            /// </summary>
            [EnumMember(Value = "hr")]
            Hr = 5,

            /// <summary>
            /// Enum Nl for value: nl
            /// </summary>
            [EnumMember(Value = "nl")]
            Nl = 6,

            /// <summary>
            /// Enum Fi for value: fi
            /// </summary>
            [EnumMember(Value = "fi")]
            Fi = 7,

            /// <summary>
            /// Enum El for value: el
            /// </summary>
            [EnumMember(Value = "el")]
            El = 8,

            /// <summary>
            /// Enum Fr for value: fr
            /// </summary>
            [EnumMember(Value = "fr")]
            Fr = 9

        }


        /// <summary>
        /// Gets or Sets Languages
        /// </summary>
        [DataMember(Name = "languages", EmitDefaultValue = true)]
        public List<LanguagesEnum> Languages { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="EventStatsQuery" /> class.
        /// </summary>
        /// <param name="end">end.</param>
        /// <param name="hazards">hazards.</param>
        /// <param name="infotypes">infotypes.</param>
        /// <param name="languages">languages.</param>
        /// <param name="northEast">northEast.</param>
        /// <param name="southWest">southWest.</param>
        /// <param name="start">start.</param>
        public EventStatsQuery(DateTime? end = default(DateTime?), List<int> hazards = default(List<int>), List<int> infotypes = default(List<int>), List<LanguagesEnum> languages = default(List<LanguagesEnum>), List<float> northEast = default(List<float>), List<float> southWest = default(List<float>), DateTime? start = default(DateTime?))
        {
            this.End = end;
            this.Hazards = hazards;
            this.Infotypes = infotypes;
            this.Languages = languages;
            this.NorthEast = northEast;
            this.SouthWest = southWest;
            this.Start = start;
        }

        /// <summary>
        /// Gets or Sets End
        /// </summary>
        [DataMember(Name = "end", EmitDefaultValue = true)]
        public DateTime? End { get; set; }

        /// <summary>
        /// Gets or Sets Hazards
        /// </summary>
        [DataMember(Name = "hazards", EmitDefaultValue = true)]
        public List<int> Hazards { get; set; }

        /// <summary>
        /// Gets or Sets Infotypes
        /// </summary>
        [DataMember(Name = "infotypes", EmitDefaultValue = true)]
        public List<int> Infotypes { get; set; }

        /// <summary>
        /// Gets or Sets NorthEast
        /// </summary>
        [DataMember(Name = "north_east", EmitDefaultValue = true)]
        public List<float> NorthEast { get; set; }

        /// <summary>
        /// Gets or Sets SouthWest
        /// </summary>
        [DataMember(Name = "south_west", EmitDefaultValue = true)]
        public List<float> SouthWest { get; set; }

        /// <summary>
        /// Gets or Sets Start
        /// </summary>
        [DataMember(Name = "start", EmitDefaultValue = true)]
        public DateTime? Start { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class GenericQuery {\n");
            sb.Append("  End: ").Append(End).Append("\n");
            sb.Append("  Hazards: ").Append(Hazards).Append("\n");
            sb.Append("  Infotypes: ").Append(Infotypes).Append("\n");
            sb.Append("  Languages: ").Append(Languages).Append("\n");
            sb.Append("  NorthEast: ").Append(NorthEast).Append("\n");
            sb.Append("  SouthWest: ").Append(SouthWest).Append("\n");
            sb.Append("  Start: ").Append(Start).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object input)
        {
            return this.Equals(input as EventStatsQuery);
        }

        /// <summary>
        /// Returns true if GenericQuery instances are equal
        /// </summary>
        /// <param name="input">Instance of GenericQuery to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(EventStatsQuery input)
        {
            if (input == null)
                return false;

            return
                (
                    this.End == input.End ||
                    (this.End != null &&
                    this.End.Equals(input.End))
                ) &&
                (
                    this.Hazards == input.Hazards ||
                    this.Hazards != null &&
                    input.Hazards != null &&
                    this.Hazards.SequenceEqual(input.Hazards)
                ) &&
                (
                    this.Infotypes == input.Infotypes ||
                    this.Infotypes != null &&
                    input.Infotypes != null &&
                    this.Infotypes.SequenceEqual(input.Infotypes)
                ) &&
                (
                    this.Languages == input.Languages ||
                    this.Languages.SequenceEqual(input.Languages)
                ) &&
                (
                    this.NorthEast == input.NorthEast ||
                    this.NorthEast != null &&
                    input.NorthEast != null &&
                    this.NorthEast.SequenceEqual(input.NorthEast)
                ) &&
                (
                    this.SouthWest == input.SouthWest ||
                    this.SouthWest != null &&
                    input.SouthWest != null &&
                    this.SouthWest.SequenceEqual(input.SouthWest)
                ) &&
                (
                    this.Start == input.Start ||
                    (this.Start != null &&
                    this.Start.Equals(input.Start))
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hashCode = 41;
                if (this.End != null)
                    hashCode = hashCode * 59 + this.End.GetHashCode();
                if (this.Hazards != null)
                    hashCode = hashCode * 59 + this.Hazards.GetHashCode();
                if (this.Infotypes != null)
                    hashCode = hashCode * 59 + this.Infotypes.GetHashCode();
                hashCode = hashCode * 59 + this.Languages.GetHashCode();
                if (this.NorthEast != null)
                    hashCode = hashCode * 59 + this.NorthEast.GetHashCode();
                if (this.SouthWest != null)
                    hashCode = hashCode * 59 + this.SouthWest.GetHashCode();
                if (this.Start != null)
                    hashCode = hashCode * 59 + this.Start.GetHashCode();
                return hashCode;
            }
        }
    }
}
