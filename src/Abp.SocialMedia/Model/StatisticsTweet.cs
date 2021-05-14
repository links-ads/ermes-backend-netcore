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
    /// StatisticsTweet
    /// </summary>
    [DataContract(Name = "StatisticsTweet")]
    public partial class StatisticsTweet : IEquatable<StatisticsTweet>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticsTweet" /> class.
        /// </summary>
        /// <param name="hazardsCount">hazardsCount.</param>
        /// <param name="informativenessRatio">informativenessRatio.</param>
        /// <param name="infotypesCount">infotypesCount.</param>
        /// <param name="languagesCount">languagesCount.</param>
        /// <param name="tweetsCount">tweetsCount.</param>
        public StatisticsTweet(Object hazardsCount = default(Object), float informativenessRatio = default(float), Object infotypesCount = default(Object), Object languagesCount = default(Object), int tweetsCount = default(int))
        {
            this.HazardsCount = hazardsCount;
            this.InformativenessRatio = informativenessRatio;
            this.InfotypesCount = infotypesCount;
            this.LanguagesCount = languagesCount;
            this.TweetsCount = tweetsCount;
        }

        /// <summary>
        /// Gets or Sets HazardsCount
        /// </summary>
        [DataMember(Name = "hazards_count", EmitDefaultValue = true)]
        public Object HazardsCount { get; set; }

        /// <summary>
        /// Gets or Sets InformativenessRatio
        /// </summary>
        [DataMember(Name = "informativeness_ratio", EmitDefaultValue = true)]
        public float InformativenessRatio { get; set; }

        /// <summary>
        /// Gets or Sets InfotypesCount
        /// </summary>
        [DataMember(Name = "infotypes_count", EmitDefaultValue = true)]
        public Object InfotypesCount { get; set; }

        /// <summary>
        /// Gets or Sets LanguagesCount
        /// </summary>
        [DataMember(Name = "languages_count", EmitDefaultValue = true)]
        public Object LanguagesCount { get; set; }

        /// <summary>
        /// Gets or Sets TweetsCount
        /// </summary>
        [DataMember(Name = "tweets_count", EmitDefaultValue = true)]
        public int TweetsCount { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class StatisticsTweet {\n");
            sb.Append("  HazardsCount: ").Append(HazardsCount).Append("\n");
            sb.Append("  InformativenessRatio: ").Append(InformativenessRatio).Append("\n");
            sb.Append("  InfotypesCount: ").Append(InfotypesCount).Append("\n");
            sb.Append("  LanguagesCount: ").Append(LanguagesCount).Append("\n");
            sb.Append("  TweetsCount: ").Append(TweetsCount).Append("\n");
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
            return this.Equals(input as StatisticsTweet);
        }

        /// <summary>
        /// Returns true if StatisticsTweet instances are equal
        /// </summary>
        /// <param name="input">Instance of StatisticsTweet to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(StatisticsTweet input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.HazardsCount == input.HazardsCount ||
                    (this.HazardsCount != null &&
                    this.HazardsCount.Equals(input.HazardsCount))
                ) && 
                (
                    this.InformativenessRatio == input.InformativenessRatio ||
                    this.InformativenessRatio.Equals(input.InformativenessRatio)
                ) && 
                (
                    this.InfotypesCount == input.InfotypesCount ||
                    (this.InfotypesCount != null &&
                    this.InfotypesCount.Equals(input.InfotypesCount))
                ) && 
                (
                    this.LanguagesCount == input.LanguagesCount ||
                    (this.LanguagesCount != null &&
                    this.LanguagesCount.Equals(input.LanguagesCount))
                ) && 
                (
                    this.TweetsCount == input.TweetsCount ||
                    this.TweetsCount.Equals(input.TweetsCount)
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
                if (this.HazardsCount != null)
                    hashCode = hashCode * 59 + this.HazardsCount.GetHashCode();
                hashCode = hashCode * 59 + this.InformativenessRatio.GetHashCode();
                if (this.InfotypesCount != null)
                    hashCode = hashCode * 59 + this.InfotypesCount.GetHashCode();
                if (this.LanguagesCount != null)
                    hashCode = hashCode * 59 + this.LanguagesCount.GetHashCode();
                hashCode = hashCode * 59 + this.TweetsCount.GetHashCode();
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