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
    /// Media
    /// </summary>
    [DataContract(Name = "Media")]
    public partial class Media : IEquatable<Media>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Media" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected Media() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Media" /> class.
        /// </summary>
        /// <param name="extra">extra.</param>
        /// <param name="id">id.</param>
        /// <param name="idStr">idStr.</param>
        /// <param name="informative">informative.</param>
        /// <param name="tweetId">tweetId.</param>
        /// <param name="tweetIdStr">tweetIdStr.</param>
        /// <param name="type">type (required).</param>
        /// <param name="url">url (required).</param>
        public Media(Object extra = default(Object), long id = default(long), string idStr = default(string), bool? informative = default(bool?), long tweetId = default(long), string tweetIdStr = default(string), string type = default(string), string url = default(string))
        {
            // to ensure "type" is required (not null)
            this.Type = type ?? throw new ArgumentNullException("type is a required property for Media and cannot be null");
            // to ensure "url" is required (not null)
            this.Url = url ?? throw new ArgumentNullException("url is a required property for Media and cannot be null");
            this.Extra = extra;
            this.Id = id;
            this.IdStr = idStr;
            this.Informative = informative;
            this.TweetId = tweetId;
            this.TweetIdStr = tweetIdStr;
        }

        /// <summary>
        /// Gets or Sets Extra
        /// </summary>
        [DataMember(Name = "extra", EmitDefaultValue = true)]
        public Object Extra { get; set; }

        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [DataMember(Name = "id", EmitDefaultValue = true)]
        public long Id { get; set; }

        /// <summary>
        /// Gets or Sets IdStr
        /// </summary>
        [DataMember(Name = "id_str", EmitDefaultValue = true)]
        public string IdStr { get; set; }

        /// <summary>
        /// Gets or Sets Informative
        /// </summary>
        [DataMember(Name = "informative", EmitDefaultValue = true)]
        public bool? Informative { get; set; }

        /// <summary>
        /// Gets or Sets TweetId
        /// </summary>
        [DataMember(Name = "tweet_id", EmitDefaultValue = true)]
        public long TweetId { get; set; }

        /// <summary>
        /// Gets or Sets TweetIdStr
        /// </summary>
        [DataMember(Name = "tweet_id_str", EmitDefaultValue = true)]
        public string TweetIdStr { get; set; }

        /// <summary>
        /// Gets or Sets Type
        /// </summary>
        [DataMember(Name = "type", EmitDefaultValue = true)]
        public string Type { get; set; }

        /// <summary>
        /// Gets or Sets Url
        /// </summary>
        [DataMember(Name = "url", EmitDefaultValue = true)]
        public string Url { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Media {\n");
            sb.Append("  Extra: ").Append(Extra).Append("\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  IdStr: ").Append(IdStr).Append("\n");
            sb.Append("  Informative: ").Append(Informative).Append("\n");
            sb.Append("  TweetId: ").Append(TweetId).Append("\n");
            sb.Append("  TweetIdStr: ").Append(TweetIdStr).Append("\n");
            sb.Append("  Type: ").Append(Type).Append("\n");
            sb.Append("  Url: ").Append(Url).Append("\n");
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
            return this.Equals(input as Media);
        }

        /// <summary>
        /// Returns true if Media instances are equal
        /// </summary>
        /// <param name="input">Instance of Media to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Media input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Extra == input.Extra ||
                    (this.Extra != null &&
                    this.Extra.Equals(input.Extra))
                ) && 
                (
                    this.Id == input.Id ||
                    this.Id.Equals(input.Id)
                ) && 
                (
                    this.IdStr == input.IdStr ||
                    (this.IdStr != null &&
                    this.IdStr.Equals(input.IdStr))
                ) && 
                (
                    this.Informative == input.Informative ||
                    (this.Informative != null &&
                    this.Informative.Equals(input.Informative))
                ) && 
                (
                    this.TweetId == input.TweetId ||
                    this.TweetId.Equals(input.TweetId)
                ) && 
                (
                    this.TweetIdStr == input.TweetIdStr ||
                    (this.TweetIdStr != null &&
                    this.TweetIdStr.Equals(input.TweetIdStr))
                ) && 
                (
                    this.Type == input.Type ||
                    (this.Type != null &&
                    this.Type.Equals(input.Type))
                ) && 
                (
                    this.Url == input.Url ||
                    (this.Url != null &&
                    this.Url.Equals(input.Url))
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
                if (this.Extra != null)
                    hashCode = hashCode * 59 + this.Extra.GetHashCode();
                hashCode = hashCode * 59 + this.Id.GetHashCode();
                if (this.IdStr != null)
                    hashCode = hashCode * 59 + this.IdStr.GetHashCode();
                if (this.Informative != null)
                    hashCode = hashCode * 59 + this.Informative.GetHashCode();
                hashCode = hashCode * 59 + this.TweetId.GetHashCode();
                if (this.TweetIdStr != null)
                    hashCode = hashCode * 59 + this.TweetIdStr.GetHashCode();
                if (this.Type != null)
                    hashCode = hashCode * 59 + this.Type.GetHashCode();
                if (this.Url != null)
                    hashCode = hashCode * 59 + this.Url.GetHashCode();
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
            // Type (string) maxLength
            if(this.Type != null && this.Type.Length > 5)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Type, length must be less than 5.", new [] { "Type" });
            }

            // Url (string) maxLength
            if(this.Url != null && this.Url.Length > 1024)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Url, length must be less than 1024.", new [] { "Url" });
            }

            yield break;
        }
    }

}
