/*
 * Event Detection Module - Public API
 *
 *  ### Public endpoints for the Social Media Analysis module (SMA). 
 *
 * The version of the OpenAPI document: 1.0.0
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */


using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using OpenAPIDateConverter = Abp.SocialMedia.Client.OpenAPIDateConverter;

namespace Abp.SocialMedia.Model
{
    /// <summary>
    /// EventQuery
    /// </summary>
    [DataContract(Name = "EventQuery")]
    public partial class EventQuery : IEquatable<EventQuery>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventQuery" /> class.
        /// </summary>
        /// <param name="end">end.</param>
        /// <param name="hazards">hazards.</param>
        /// <param name="limit">limit.</param>
        /// <param name="northEast">northEast.</param>
        /// <param name="page">page.</param>
        /// <param name="southWest">southWest.</param>
        /// <param name="start">start.</param>
        /// <param name="verified">verified.</param>
        public EventQuery(DateTime? end = default(DateTime?), List<int> hazards = default(List<int>), int? limit = default(int?), List<float> northEast = default(List<float>), int? page = default(int?), List<float> southWest = default(List<float>), DateTime? start = default(DateTime?), bool? verified = default(bool?))
        {
            this.End = end;
            this.Hazards = hazards;
            this.Limit = limit;
            this.NorthEast = northEast;
            this.Page = page;
            this.SouthWest = southWest;
            this.Start = start;
            this.Verified = verified;
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
        /// Gets or Sets Limit
        /// </summary>
        [DataMember(Name = "limit", EmitDefaultValue = true)]
        public int? Limit { get; set; }

        /// <summary>
        /// Gets or Sets NorthEast
        /// </summary>
        [DataMember(Name = "north_east", EmitDefaultValue = true)]
        public List<float> NorthEast { get; set; }

        /// <summary>
        /// Gets or Sets Page
        /// </summary>
        [DataMember(Name = "page", EmitDefaultValue = true)]
        public int? Page { get; set; }

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
        /// Gets or Sets Verified
        /// </summary>
        [DataMember(Name = "verified", EmitDefaultValue = true)]
        public bool? Verified { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class EventQuery {\n");
            sb.Append("  End: ").Append(End).Append("\n");
            sb.Append("  Hazards: ").Append(Hazards).Append("\n");
            sb.Append("  Limit: ").Append(Limit).Append("\n");
            sb.Append("  NorthEast: ").Append(NorthEast).Append("\n");
            sb.Append("  Page: ").Append(Page).Append("\n");
            sb.Append("  SouthWest: ").Append(SouthWest).Append("\n");
            sb.Append("  Start: ").Append(Start).Append("\n");
            sb.Append("  Verified: ").Append(Verified).Append("\n");
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
            return this.Equals(input as EventQuery);
        }

        /// <summary>
        /// Returns true if EventQuery instances are equal
        /// </summary>
        /// <param name="input">Instance of EventQuery to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(EventQuery input)
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
                    this.Limit == input.Limit ||
                    (this.Limit != null &&
                    this.Limit.Equals(input.Limit))
                ) && 
                (
                    this.NorthEast == input.NorthEast ||
                    this.NorthEast != null &&
                    input.NorthEast != null &&
                    this.NorthEast.SequenceEqual(input.NorthEast)
                ) && 
                (
                    this.Page == input.Page ||
                    (this.Page != null &&
                    this.Page.Equals(input.Page))
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
                ) && 
                (
                    this.Verified == input.Verified ||
                    (this.Verified != null &&
                    this.Verified.Equals(input.Verified))
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
                if (this.Limit != null)
                    hashCode = hashCode * 59 + this.Limit.GetHashCode();
                if (this.NorthEast != null)
                    hashCode = hashCode * 59 + this.NorthEast.GetHashCode();
                if (this.Page != null)
                    hashCode = hashCode * 59 + this.Page.GetHashCode();
                if (this.SouthWest != null)
                    hashCode = hashCode * 59 + this.SouthWest.GetHashCode();
                if (this.Start != null)
                    hashCode = hashCode * 59 + this.Start.GetHashCode();
                if (this.Verified != null)
                    hashCode = hashCode * 59 + this.Verified.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// To validate all properties of the instance
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }

}
