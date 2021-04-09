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
    /// Author
    /// </summary>
    [DataContract(Name = "Author")]
    public partial class Author : IEquatable<Author>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Author" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected Author() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Author" /> class.
        /// </summary>
        /// <param name="createdAt">createdAt.</param>
        /// <param name="displayName">displayName (required).</param>
        /// <param name="favoriteCount">favoriteCount.</param>
        /// <param name="followedCount">followedCount.</param>
        /// <param name="followerCount">followerCount.</param>
        /// <param name="id">id.</param>
        /// <param name="idStr">idStr.</param>
        /// <param name="profileImage">profileImage.</param>
        /// <param name="statusesCount">statusesCount.</param>
        /// <param name="userName">userName (required).</param>
        /// <param name="verified">verified.</param>
        public Author(string createdAt = default(string), string displayName = default(string), int favoriteCount = default(int), int followedCount = default(int), int followerCount = default(int), long id = default(long), string idStr = default(string), string profileImage = default(string), int statusesCount = default(int), string userName = default(string), bool verified = default(bool))
        {
            // to ensure "displayName" is required (not null)
            this.DisplayName = displayName ?? throw new ArgumentNullException("displayName is a required property for Author and cannot be null");
            // to ensure "userName" is required (not null)
            this.UserName = userName ?? throw new ArgumentNullException("userName is a required property for Author and cannot be null");
            this.CreatedAt = createdAt;
            this.FavoriteCount = favoriteCount;
            this.FollowedCount = followedCount;
            this.FollowerCount = followerCount;
            this.Id = id;
            this.IdStr = idStr;
            this.ProfileImage = profileImage;
            this.StatusesCount = statusesCount;
            this.Verified = verified;
        }

        /// <summary>
        /// Gets or Sets CreatedAt
        /// </summary>
        [DataMember(Name = "created_at", EmitDefaultValue = true)]
        public string CreatedAt { get; set; }

        /// <summary>
        /// Gets or Sets DisplayName
        /// </summary>
        [DataMember(Name = "display_name", EmitDefaultValue = true)]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or Sets FavoriteCount
        /// </summary>
        [DataMember(Name = "favorite_count", EmitDefaultValue = true)]
        public int FavoriteCount { get; set; }

        /// <summary>
        /// Gets or Sets FollowedCount
        /// </summary>
        [DataMember(Name = "followed_count", EmitDefaultValue = true)]
        public int FollowedCount { get; set; }

        /// <summary>
        /// Gets or Sets FollowerCount
        /// </summary>
        [DataMember(Name = "follower_count", EmitDefaultValue = true)]
        public int FollowerCount { get; set; }

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
        /// Gets or Sets ProfileImage
        /// </summary>
        [DataMember(Name = "profile_image", EmitDefaultValue = true)]
        public string ProfileImage { get; set; }

        /// <summary>
        /// Gets or Sets StatusesCount
        /// </summary>
        [DataMember(Name = "statuses_count", EmitDefaultValue = true)]
        public int StatusesCount { get; set; }

        /// <summary>
        /// Gets or Sets UserName
        /// </summary>
        [DataMember(Name = "user_name", EmitDefaultValue = true)]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or Sets Verified
        /// </summary>
        [DataMember(Name = "verified", EmitDefaultValue = true)]
        public bool Verified { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Author {\n");
            sb.Append("  CreatedAt: ").Append(CreatedAt).Append("\n");
            sb.Append("  DisplayName: ").Append(DisplayName).Append("\n");
            sb.Append("  FavoriteCount: ").Append(FavoriteCount).Append("\n");
            sb.Append("  FollowedCount: ").Append(FollowedCount).Append("\n");
            sb.Append("  FollowerCount: ").Append(FollowerCount).Append("\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  IdStr: ").Append(IdStr).Append("\n");
            sb.Append("  ProfileImage: ").Append(ProfileImage).Append("\n");
            sb.Append("  StatusesCount: ").Append(StatusesCount).Append("\n");
            sb.Append("  UserName: ").Append(UserName).Append("\n");
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
            return this.Equals(input as Author);
        }

        /// <summary>
        /// Returns true if Author instances are equal
        /// </summary>
        /// <param name="input">Instance of Author to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Author input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.CreatedAt == input.CreatedAt ||
                    (this.CreatedAt != null &&
                    this.CreatedAt.Equals(input.CreatedAt))
                ) && 
                (
                    this.DisplayName == input.DisplayName ||
                    (this.DisplayName != null &&
                    this.DisplayName.Equals(input.DisplayName))
                ) && 
                (
                    this.FavoriteCount == input.FavoriteCount ||
                    this.FavoriteCount.Equals(input.FavoriteCount)
                ) && 
                (
                    this.FollowedCount == input.FollowedCount ||
                    this.FollowedCount.Equals(input.FollowedCount)
                ) && 
                (
                    this.FollowerCount == input.FollowerCount ||
                    this.FollowerCount.Equals(input.FollowerCount)
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
                    this.ProfileImage == input.ProfileImage ||
                    (this.ProfileImage != null &&
                    this.ProfileImage.Equals(input.ProfileImage))
                ) && 
                (
                    this.StatusesCount == input.StatusesCount ||
                    this.StatusesCount.Equals(input.StatusesCount)
                ) && 
                (
                    this.UserName == input.UserName ||
                    (this.UserName != null &&
                    this.UserName.Equals(input.UserName))
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
                if (this.CreatedAt != null)
                    hashCode = hashCode * 59 + this.CreatedAt.GetHashCode();
                if (this.DisplayName != null)
                    hashCode = hashCode * 59 + this.DisplayName.GetHashCode();
                hashCode = hashCode * 59 + this.FavoriteCount.GetHashCode();
                hashCode = hashCode * 59 + this.FollowedCount.GetHashCode();
                hashCode = hashCode * 59 + this.FollowerCount.GetHashCode();
                hashCode = hashCode * 59 + this.Id.GetHashCode();
                if (this.IdStr != null)
                    hashCode = hashCode * 59 + this.IdStr.GetHashCode();
                if (this.ProfileImage != null)
                    hashCode = hashCode * 59 + this.ProfileImage.GetHashCode();
                hashCode = hashCode * 59 + this.StatusesCount.GetHashCode();
                if (this.UserName != null)
                    hashCode = hashCode * 59 + this.UserName.GetHashCode();
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
            // DisplayName (string) maxLength
            if(this.DisplayName != null && this.DisplayName.Length > 50)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for DisplayName, length must be less than 50.", new [] { "DisplayName" });
            }

            // ProfileImage (string) maxLength
            if(this.ProfileImage != null && this.ProfileImage.Length > 1024)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for ProfileImage, length must be less than 1024.", new [] { "ProfileImage" });
            }

            // UserName (string) maxLength
            if(this.UserName != null && this.UserName.Length > 32)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for UserName, length must be less than 32.", new [] { "UserName" });
            }

            yield break;
        }
    }

}
