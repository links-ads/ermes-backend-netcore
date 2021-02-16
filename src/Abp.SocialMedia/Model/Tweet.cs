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
    /// Tweet
    /// </summary>
    [DataContract(Name = "Tweet")]
    public partial class Tweet : IEquatable<Tweet>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Tweet" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected Tweet() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Tweet" /> class.
        /// </summary>
        /// <param name="author">author.</param>
        /// <param name="authorId">authorId.</param>
        /// <param name="createdAt">createdAt (required).</param>
        /// <param name="favoriteCount">favoriteCount.</param>
        /// <param name="filterId">filterId.</param>
        /// <param name="hashtags">hashtags.</param>
        /// <param name="id">id.</param>
        /// <param name="informative">informative.</param>
        /// <param name="insertedAt">insertedAt.</param>
        /// <param name="lang">lang (required).</param>
        /// <param name="location">location.</param>
        /// <param name="media">media.</param>
        /// <param name="parent">parent.</param>
        /// <param name="retweetCount">retweetCount.</param>
        /// <param name="retweeted">retweeted.</param>
        /// <param name="source">source.</param>
        /// <param name="text">text (required).</param>
        /// <param name="tokens">tokens.</param>
        public Tweet(Object author = default(Object), long authorId = default(long), DateTime createdAt = default(DateTime), int favoriteCount = default(int), int? filterId = default(int?), string hashtags = default(string), long id = default(long), bool? informative = default(bool?), DateTime insertedAt = default(DateTime), string lang = default(string), Object location = default(Object), List<Object> media = default(List<Object>), long? parent = default(long?), int retweetCount = default(int), bool retweeted = default(bool), string source = default(string), string text = default(string), string tokens = default(string))
        {
            this.CreatedAt = createdAt;
            // to ensure "lang" is required (not null)
            this.Lang = lang ?? throw new ArgumentNullException("lang is a required property for Tweet and cannot be null");
            // to ensure "text" is required (not null)
            this.Text = text ?? throw new ArgumentNullException("text is a required property for Tweet and cannot be null");
            this.Author = author;
            this.AuthorId = authorId;
            this.FavoriteCount = favoriteCount;
            this.FilterId = filterId;
            this.Hashtags = hashtags;
            this.Id = id;
            this.Informative = informative;
            this.InsertedAt = insertedAt;
            this.Location = location;
            this.Media = media;
            this.Parent = parent;
            this.RetweetCount = retweetCount;
            this.Retweeted = retweeted;
            this.Source = source;
            this.Tokens = tokens;
        }

        /// <summary>
        /// Gets or Sets Author
        /// </summary>
        [DataMember(Name = "author", EmitDefaultValue = true)]
        public Object Author { get; set; }

        /// <summary>
        /// Gets or Sets AuthorId
        /// </summary>
        [DataMember(Name = "author_id", EmitDefaultValue = false)]
        public long AuthorId { get; set; }

        /// <summary>
        /// Gets or Sets CreatedAt
        /// </summary>
        [DataMember(Name = "created_at", EmitDefaultValue = false)]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or Sets FavoriteCount
        /// </summary>
        [DataMember(Name = "favorite_count", EmitDefaultValue = false)]
        public int FavoriteCount { get; set; }

        /// <summary>
        /// Gets or Sets FilterId
        /// </summary>
        [DataMember(Name = "filter_id", EmitDefaultValue = true)]
        public int? FilterId { get; set; }

        /// <summary>
        /// Gets or Sets Hashtags
        /// </summary>
        [DataMember(Name = "hashtags", EmitDefaultValue = true)]
        public string Hashtags { get; set; }

        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public long Id { get; set; }

        /// <summary>
        /// Gets or Sets Informative
        /// </summary>
        [DataMember(Name = "informative", EmitDefaultValue = true)]
        public bool? Informative { get; set; }

        /// <summary>
        /// Gets or Sets InsertedAt
        /// </summary>
        [DataMember(Name = "inserted_at", EmitDefaultValue = false)]
        public DateTime InsertedAt { get; set; }

        /// <summary>
        /// Gets or Sets Lang
        /// </summary>
        [DataMember(Name = "lang", EmitDefaultValue = false)]
        public string Lang { get; set; }

        /// <summary>
        /// Gets or Sets Location
        /// </summary>
        [DataMember(Name = "location", EmitDefaultValue = true)]
        public Object Location { get; set; }

        /// <summary>
        /// Gets or Sets Media
        /// </summary>
        [DataMember(Name = "media", EmitDefaultValue = false)]
        public List<Object> Media { get; set; }

        /// <summary>
        /// Gets or Sets Parent
        /// </summary>
        [DataMember(Name = "parent", EmitDefaultValue = true)]
        public long? Parent { get; set; }

        /// <summary>
        /// Gets or Sets RetweetCount
        /// </summary>
        [DataMember(Name = "retweet_count", EmitDefaultValue = false)]
        public int RetweetCount { get; set; }

        /// <summary>
        /// Gets or Sets Retweeted
        /// </summary>
        [DataMember(Name = "retweeted", EmitDefaultValue = false)]
        public bool Retweeted { get; set; }

        /// <summary>
        /// Gets or Sets Source
        /// </summary>
        [DataMember(Name = "source", EmitDefaultValue = true)]
        public string Source { get; set; }

        /// <summary>
        /// Gets or Sets Text
        /// </summary>
        [DataMember(Name = "text", EmitDefaultValue = false)]
        public string Text { get; set; }

        /// <summary>
        /// Gets or Sets Tokens
        /// </summary>
        [DataMember(Name = "tokens", EmitDefaultValue = true)]
        public string Tokens { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Tweet {\n");
            sb.Append("  Author: ").Append(Author).Append("\n");
            sb.Append("  AuthorId: ").Append(AuthorId).Append("\n");
            sb.Append("  CreatedAt: ").Append(CreatedAt).Append("\n");
            sb.Append("  FavoriteCount: ").Append(FavoriteCount).Append("\n");
            sb.Append("  FilterId: ").Append(FilterId).Append("\n");
            sb.Append("  Hashtags: ").Append(Hashtags).Append("\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  Informative: ").Append(Informative).Append("\n");
            sb.Append("  InsertedAt: ").Append(InsertedAt).Append("\n");
            sb.Append("  Lang: ").Append(Lang).Append("\n");
            sb.Append("  Location: ").Append(Location).Append("\n");
            sb.Append("  Media: ").Append(Media).Append("\n");
            sb.Append("  Parent: ").Append(Parent).Append("\n");
            sb.Append("  RetweetCount: ").Append(RetweetCount).Append("\n");
            sb.Append("  Retweeted: ").Append(Retweeted).Append("\n");
            sb.Append("  Source: ").Append(Source).Append("\n");
            sb.Append("  Text: ").Append(Text).Append("\n");
            sb.Append("  Tokens: ").Append(Tokens).Append("\n");
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
            return this.Equals(input as Tweet);
        }

        /// <summary>
        /// Returns true if Tweet instances are equal
        /// </summary>
        /// <param name="input">Instance of Tweet to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Tweet input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Author == input.Author ||
                    (this.Author != null &&
                    this.Author.Equals(input.Author))
                ) && 
                (
                    this.AuthorId == input.AuthorId ||
                    this.AuthorId.Equals(input.AuthorId)
                ) && 
                (
                    this.CreatedAt == input.CreatedAt ||
                    (this.CreatedAt != null &&
                    this.CreatedAt.Equals(input.CreatedAt))
                ) && 
                (
                    this.FavoriteCount == input.FavoriteCount ||
                    this.FavoriteCount.Equals(input.FavoriteCount)
                ) && 
                (
                    this.FilterId == input.FilterId ||
                    (this.FilterId != null &&
                    this.FilterId.Equals(input.FilterId))
                ) && 
                (
                    this.Hashtags == input.Hashtags ||
                    (this.Hashtags != null &&
                    this.Hashtags.Equals(input.Hashtags))
                ) && 
                (
                    this.Id == input.Id ||
                    this.Id.Equals(input.Id)
                ) && 
                (
                    this.Informative == input.Informative ||
                    (this.Informative != null &&
                    this.Informative.Equals(input.Informative))
                ) && 
                (
                    this.InsertedAt == input.InsertedAt ||
                    (this.InsertedAt != null &&
                    this.InsertedAt.Equals(input.InsertedAt))
                ) && 
                (
                    this.Lang == input.Lang ||
                    (this.Lang != null &&
                    this.Lang.Equals(input.Lang))
                ) && 
                (
                    this.Location == input.Location ||
                    (this.Location != null &&
                    this.Location.Equals(input.Location))
                ) && 
                (
                    this.Media == input.Media ||
                    this.Media != null &&
                    input.Media != null &&
                    this.Media.SequenceEqual(input.Media)
                ) && 
                (
                    this.Parent == input.Parent ||
                    (this.Parent != null &&
                    this.Parent.Equals(input.Parent))
                ) && 
                (
                    this.RetweetCount == input.RetweetCount ||
                    this.RetweetCount.Equals(input.RetweetCount)
                ) && 
                (
                    this.Retweeted == input.Retweeted ||
                    this.Retweeted.Equals(input.Retweeted)
                ) && 
                (
                    this.Source == input.Source ||
                    (this.Source != null &&
                    this.Source.Equals(input.Source))
                ) && 
                (
                    this.Text == input.Text ||
                    (this.Text != null &&
                    this.Text.Equals(input.Text))
                ) && 
                (
                    this.Tokens == input.Tokens ||
                    (this.Tokens != null &&
                    this.Tokens.Equals(input.Tokens))
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
                if (this.Author != null)
                    hashCode = hashCode * 59 + this.Author.GetHashCode();
                hashCode = hashCode * 59 + this.AuthorId.GetHashCode();
                if (this.CreatedAt != null)
                    hashCode = hashCode * 59 + this.CreatedAt.GetHashCode();
                hashCode = hashCode * 59 + this.FavoriteCount.GetHashCode();
                if (this.FilterId != null)
                    hashCode = hashCode * 59 + this.FilterId.GetHashCode();
                if (this.Hashtags != null)
                    hashCode = hashCode * 59 + this.Hashtags.GetHashCode();
                hashCode = hashCode * 59 + this.Id.GetHashCode();
                if (this.Informative != null)
                    hashCode = hashCode * 59 + this.Informative.GetHashCode();
                if (this.InsertedAt != null)
                    hashCode = hashCode * 59 + this.InsertedAt.GetHashCode();
                if (this.Lang != null)
                    hashCode = hashCode * 59 + this.Lang.GetHashCode();
                if (this.Location != null)
                    hashCode = hashCode * 59 + this.Location.GetHashCode();
                if (this.Media != null)
                    hashCode = hashCode * 59 + this.Media.GetHashCode();
                if (this.Parent != null)
                    hashCode = hashCode * 59 + this.Parent.GetHashCode();
                hashCode = hashCode * 59 + this.RetweetCount.GetHashCode();
                hashCode = hashCode * 59 + this.Retweeted.GetHashCode();
                if (this.Source != null)
                    hashCode = hashCode * 59 + this.Source.GetHashCode();
                if (this.Text != null)
                    hashCode = hashCode * 59 + this.Text.GetHashCode();
                if (this.Tokens != null)
                    hashCode = hashCode * 59 + this.Tokens.GetHashCode();
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
            // Hashtags (string) maxLength
            if(this.Hashtags != null && this.Hashtags.Length > 1024)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Hashtags, length must be less than 1024.", new [] { "Hashtags" });
            }

            // Lang (string) maxLength
            if(this.Lang != null && this.Lang.Length > 3)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Lang, length must be less than 3.", new [] { "Lang" });
            }

            // Source (string) maxLength
            if(this.Source != null && this.Source.Length > 128)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Source, length must be less than 128.", new [] { "Source" });
            }

            // Text (string) maxLength
            if(this.Text != null && this.Text.Length > 1024)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Text, length must be less than 1024.", new [] { "Text" });
            }

            // Tokens (string) maxLength
            if(this.Tokens != null && this.Tokens.Length > 1024)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Tokens, length must be less than 1024.", new [] { "Tokens" });
            }

            yield break;
        }
    }

}
