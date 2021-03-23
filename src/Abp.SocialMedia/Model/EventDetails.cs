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
    /// EventDetails
    /// </summary>
    [DataContract(Name = "EventDetails")]
    public partial class EventDetails : IEquatable<EventDetails>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventDetails" /> class.
        /// </summary>
        /// <param name="activatedAt">activatedAt.</param>
        /// <param name="endedAt">endedAt.</param>
        /// <param name="hazardId">hazardId.</param>
        /// <param name="hotspots">hotspots.</param>
        /// <param name="id">id.</param>
        /// <param name="lang">lang.</param>
        /// <param name="name">name.</param>
        /// <param name="startedAt">startedAt.</param>
        /// <param name="totalArea">totalArea.</param>
        /// <param name="trackingStoppedAt">trackingStoppedAt.</param>
        /// <param name="tweets">tweets.</param>
        /// <param name="updatedAt">updatedAt.</param>
        /// <param name="verified">verified.</param>
        public EventDetails(DateTime activatedAt = default(DateTime), DateTime endedAt = default(DateTime), int hazardId = default(int), Object hotspots = default(Object), int id = default(int), string lang = default(string), string name = default(string), DateTime startedAt = default(DateTime), Object totalArea = default(Object), DateTime trackingStoppedAt = default(DateTime), List<PartialTweet> tweets = default(List<PartialTweet>), DateTime updatedAt = default(DateTime), bool verified = default(bool))
        {
            this.ActivatedAt = activatedAt;
            this.EndedAt = endedAt;
            this.HazardId = hazardId;
            this.Hotspots = hotspots;
            this.Id = id;
            this.Lang = lang;
            this.Name = name;
            this.StartedAt = startedAt;
            this.TotalArea = totalArea;
            this.TrackingStoppedAt = trackingStoppedAt;
            this.Tweets = tweets;
            this.UpdatedAt = updatedAt;
            this.Verified = verified;
        }

        /// <summary>
        /// Gets or Sets ActivatedAt
        /// </summary>
        [DataMember(Name = "activated_at", EmitDefaultValue = false)]
        public DateTime ActivatedAt { get; set; }

        /// <summary>
        /// Gets or Sets EndedAt
        /// </summary>
        [DataMember(Name = "ended_at", EmitDefaultValue = false)]
        public DateTime EndedAt { get; set; }

        /// <summary>
        /// Gets or Sets HazardId
        /// </summary>
        [DataMember(Name = "hazard_id", EmitDefaultValue = false)]
        public int HazardId { get; set; }

        /// <summary>
        /// Gets or Sets Hotspots
        /// </summary>
        [DataMember(Name = "hotspots", EmitDefaultValue = true)]
        public Object Hotspots { get; set; }

        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or Sets Lang
        /// </summary>
        [DataMember(Name = "lang", EmitDefaultValue = false)]
        public string Lang { get; set; }

        /// <summary>
        /// Gets or Sets Name
        /// </summary>
        [DataMember(Name = "name", EmitDefaultValue = false)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets StartedAt
        /// </summary>
        [DataMember(Name = "started_at", EmitDefaultValue = false)]
        public DateTime StartedAt { get; set; }

        /// <summary>
        /// Gets or Sets TotalArea
        /// </summary>
        [DataMember(Name = "total_area", EmitDefaultValue = true)]
        public Object TotalArea { get; set; }

        /// <summary>
        /// Gets or Sets TrackingStoppedAt
        /// </summary>
        [DataMember(Name = "tracking_stopped_at", EmitDefaultValue = false)]
        public DateTime TrackingStoppedAt { get; set; }

        /// <summary>
        /// Gets or Sets Tweets
        /// </summary>
        [DataMember(Name = "tweets", EmitDefaultValue = false)]
        public List<PartialTweet> Tweets { get; set; }

        /// <summary>
        /// Gets or Sets UpdatedAt
        /// </summary>
        [DataMember(Name = "updated_at", EmitDefaultValue = false)]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Gets or Sets Verified
        /// </summary>
        [DataMember(Name = "verified", EmitDefaultValue = false)]
        public bool Verified { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class EventDetails {\n");
            sb.Append("  ActivatedAt: ").Append(ActivatedAt).Append("\n");
            sb.Append("  EndedAt: ").Append(EndedAt).Append("\n");
            sb.Append("  HazardId: ").Append(HazardId).Append("\n");
            sb.Append("  Hotspots: ").Append(Hotspots).Append("\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  Lang: ").Append(Lang).Append("\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  StartedAt: ").Append(StartedAt).Append("\n");
            sb.Append("  TotalArea: ").Append(TotalArea).Append("\n");
            sb.Append("  TrackingStoppedAt: ").Append(TrackingStoppedAt).Append("\n");
            sb.Append("  Tweets: ").Append(Tweets).Append("\n");
            sb.Append("  UpdatedAt: ").Append(UpdatedAt).Append("\n");
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
            return this.Equals(input as EventDetails);
        }

        /// <summary>
        /// Returns true if EventDetails instances are equal
        /// </summary>
        /// <param name="input">Instance of EventDetails to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(EventDetails input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.ActivatedAt == input.ActivatedAt ||
                    (this.ActivatedAt != null &&
                    this.ActivatedAt.Equals(input.ActivatedAt))
                ) && 
                (
                    this.EndedAt == input.EndedAt ||
                    (this.EndedAt != null &&
                    this.EndedAt.Equals(input.EndedAt))
                ) && 
                (
                    this.HazardId == input.HazardId ||
                    this.HazardId.Equals(input.HazardId)
                ) && 
                (
                    this.Hotspots == input.Hotspots ||
                    (this.Hotspots != null &&
                    this.Hotspots.Equals(input.Hotspots))
                ) && 
                (
                    this.Id == input.Id ||
                    this.Id.Equals(input.Id)
                ) && 
                (
                    this.Lang == input.Lang ||
                    (this.Lang != null &&
                    this.Lang.Equals(input.Lang))
                ) && 
                (
                    this.Name == input.Name ||
                    (this.Name != null &&
                    this.Name.Equals(input.Name))
                ) && 
                (
                    this.StartedAt == input.StartedAt ||
                    (this.StartedAt != null &&
                    this.StartedAt.Equals(input.StartedAt))
                ) && 
                (
                    this.TotalArea == input.TotalArea ||
                    (this.TotalArea != null &&
                    this.TotalArea.Equals(input.TotalArea))
                ) && 
                (
                    this.TrackingStoppedAt == input.TrackingStoppedAt ||
                    (this.TrackingStoppedAt != null &&
                    this.TrackingStoppedAt.Equals(input.TrackingStoppedAt))
                ) && 
                (
                    this.Tweets == input.Tweets ||
                    this.Tweets != null &&
                    input.Tweets != null &&
                    this.Tweets.SequenceEqual(input.Tweets)
                ) && 
                (
                    this.UpdatedAt == input.UpdatedAt ||
                    (this.UpdatedAt != null &&
                    this.UpdatedAt.Equals(input.UpdatedAt))
                ) && 
                (
                    this.Verified == input.Verified ||
                    this.Verified.Equals(input.Verified)
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
                if (this.ActivatedAt != null)
                    hashCode = hashCode * 59 + this.ActivatedAt.GetHashCode();
                if (this.EndedAt != null)
                    hashCode = hashCode * 59 + this.EndedAt.GetHashCode();
                hashCode = hashCode * 59 + this.HazardId.GetHashCode();
                if (this.Hotspots != null)
                    hashCode = hashCode * 59 + this.Hotspots.GetHashCode();
                hashCode = hashCode * 59 + this.Id.GetHashCode();
                if (this.Lang != null)
                    hashCode = hashCode * 59 + this.Lang.GetHashCode();
                if (this.Name != null)
                    hashCode = hashCode * 59 + this.Name.GetHashCode();
                if (this.StartedAt != null)
                    hashCode = hashCode * 59 + this.StartedAt.GetHashCode();
                if (this.TotalArea != null)
                    hashCode = hashCode * 59 + this.TotalArea.GetHashCode();
                if (this.TrackingStoppedAt != null)
                    hashCode = hashCode * 59 + this.TrackingStoppedAt.GetHashCode();
                if (this.Tweets != null)
                    hashCode = hashCode * 59 + this.Tweets.GetHashCode();
                if (this.UpdatedAt != null)
                    hashCode = hashCode * 59 + this.UpdatedAt.GetHashCode();
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
            // Name (string) maxLength
            if(this.Name != null && this.Name.Length > 256)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Name, length must be less than 256.", new [] { "Name" });
            }

            yield break;
        }
    }

}
