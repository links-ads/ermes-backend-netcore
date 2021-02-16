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
    /// Annotation
    /// </summary>
    [DataContract(Name = "Annotation")]
    public partial class Annotation : IEquatable<Annotation>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Annotation" /> class.
        /// </summary>
        /// <param name="author">author.</param>
        /// <param name="createdAt">createdAt.</param>
        /// <param name="entities">entities.</param>
        /// <param name="favoriteCount">favoriteCount.</param>
        /// <param name="hashtags">hashtags.</param>
        /// <param name="hazardTypes">hazardTypes.</param>
        /// <param name="id">id.</param>
        /// <param name="informationTypes">informationTypes.</param>
        /// <param name="informative">informative.</param>
        /// <param name="lang">lang.</param>
        /// <param name="location">location.</param>
        /// <param name="media">media.</param>
        /// <param name="quoted">quoted.</param>
        /// <param name="retweetCount">retweetCount.</param>
        /// <param name="retweeted">retweeted.</param>
        /// <param name="source">source.</param>
        /// <param name="text">text.</param>
        public Annotation(PartialAuthor author = default(PartialAuthor), DateTime createdAt = default(DateTime), List<EntityDetails> entities = default(List<EntityDetails>), int favoriteCount = default(int), List<string> hashtags = default(List<string>), List<ClassificationDetails> hazardTypes = default(List<ClassificationDetails>), long id = default(long), List<ClassificationDetails> informationTypes = default(List<ClassificationDetails>), bool informative = default(bool), string lang = default(string), Object location = default(Object), List<PartialMedia> media = default(List<PartialMedia>), int quoted = default(int), int retweetCount = default(int), bool retweeted = default(bool), string source = default(string), string text = default(string))
        {
            this.Author = author;
            this.CreatedAt = createdAt;
            this.Entities = entities;
            this.FavoriteCount = favoriteCount;
            this.Hashtags = hashtags;
            this.HazardTypes = hazardTypes;
            this.Id = id;
            this.InformationTypes = informationTypes;
            this.Informative = informative;
            this.Lang = lang;
            this.Location = location;
            this.Media = media;
            this.Quoted = quoted;
            this.RetweetCount = retweetCount;
            this.Retweeted = retweeted;
            this.Source = source;
            this.Text = text;
        }

        /// <summary>
        /// Gets or Sets Author
        /// </summary>
        [DataMember(Name = "author", EmitDefaultValue = false)]
        public PartialAuthor Author { get; set; }

        /// <summary>
        /// Gets or Sets CreatedAt
        /// </summary>
        [DataMember(Name = "created_at", EmitDefaultValue = false)]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or Sets Entities
        /// </summary>
        [DataMember(Name = "entities", EmitDefaultValue = false)]
        public List<EntityDetails> Entities { get; set; }

        /// <summary>
        /// Gets or Sets FavoriteCount
        /// </summary>
        [DataMember(Name = "favorite_count", EmitDefaultValue = false)]
        public int FavoriteCount { get; set; }

        /// <summary>
        /// Gets or Sets Hashtags
        /// </summary>
        [DataMember(Name = "hashtags", EmitDefaultValue = false)]
        public List<string> Hashtags { get; set; }

        /// <summary>
        /// Gets or Sets HazardTypes
        /// </summary>
        [DataMember(Name = "hazard_types", EmitDefaultValue = false)]
        public List<ClassificationDetails> HazardTypes { get; set; }

        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public long Id { get; set; }

        /// <summary>
        /// Gets or Sets InformationTypes
        /// </summary>
        [DataMember(Name = "information_types", EmitDefaultValue = false)]
        public List<ClassificationDetails> InformationTypes { get; set; }

        /// <summary>
        /// Gets or Sets Informative
        /// </summary>
        [DataMember(Name = "informative", EmitDefaultValue = false)]
        public bool Informative { get; set; }

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
        public List<PartialMedia> Media { get; set; }

        /// <summary>
        /// Gets or Sets Quoted
        /// </summary>
        [DataMember(Name = "quoted", EmitDefaultValue = false)]
        public int Quoted { get; set; }

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
        [DataMember(Name = "source", EmitDefaultValue = false)]
        public string Source { get; set; }

        /// <summary>
        /// Gets or Sets Text
        /// </summary>
        [DataMember(Name = "text", EmitDefaultValue = false)]
        public string Text { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Annotation {\n");
            sb.Append("  Author: ").Append(Author).Append("\n");
            sb.Append("  CreatedAt: ").Append(CreatedAt).Append("\n");
            sb.Append("  Entities: ").Append(Entities).Append("\n");
            sb.Append("  FavoriteCount: ").Append(FavoriteCount).Append("\n");
            sb.Append("  Hashtags: ").Append(Hashtags).Append("\n");
            sb.Append("  HazardTypes: ").Append(HazardTypes).Append("\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  InformationTypes: ").Append(InformationTypes).Append("\n");
            sb.Append("  Informative: ").Append(Informative).Append("\n");
            sb.Append("  Lang: ").Append(Lang).Append("\n");
            sb.Append("  Location: ").Append(Location).Append("\n");
            sb.Append("  Media: ").Append(Media).Append("\n");
            sb.Append("  Quoted: ").Append(Quoted).Append("\n");
            sb.Append("  RetweetCount: ").Append(RetweetCount).Append("\n");
            sb.Append("  Retweeted: ").Append(Retweeted).Append("\n");
            sb.Append("  Source: ").Append(Source).Append("\n");
            sb.Append("  Text: ").Append(Text).Append("\n");
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
            return this.Equals(input as Annotation);
        }

        /// <summary>
        /// Returns true if Annotation instances are equal
        /// </summary>
        /// <param name="input">Instance of Annotation to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Annotation input)
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
                    this.CreatedAt == input.CreatedAt ||
                    (this.CreatedAt != null &&
                    this.CreatedAt.Equals(input.CreatedAt))
                ) && 
                (
                    this.Entities == input.Entities ||
                    this.Entities != null &&
                    input.Entities != null &&
                    this.Entities.SequenceEqual(input.Entities)
                ) && 
                (
                    this.FavoriteCount == input.FavoriteCount ||
                    this.FavoriteCount.Equals(input.FavoriteCount)
                ) && 
                (
                    this.Hashtags == input.Hashtags ||
                    this.Hashtags != null &&
                    input.Hashtags != null &&
                    this.Hashtags.SequenceEqual(input.Hashtags)
                ) && 
                (
                    this.HazardTypes == input.HazardTypes ||
                    this.HazardTypes != null &&
                    input.HazardTypes != null &&
                    this.HazardTypes.SequenceEqual(input.HazardTypes)
                ) && 
                (
                    this.Id == input.Id ||
                    this.Id.Equals(input.Id)
                ) && 
                (
                    this.InformationTypes == input.InformationTypes ||
                    this.InformationTypes != null &&
                    input.InformationTypes != null &&
                    this.InformationTypes.SequenceEqual(input.InformationTypes)
                ) && 
                (
                    this.Informative == input.Informative ||
                    this.Informative.Equals(input.Informative)
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
                    this.Quoted == input.Quoted ||
                    this.Quoted.Equals(input.Quoted)
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
                if (this.CreatedAt != null)
                    hashCode = hashCode * 59 + this.CreatedAt.GetHashCode();
                if (this.Entities != null)
                    hashCode = hashCode * 59 + this.Entities.GetHashCode();
                hashCode = hashCode * 59 + this.FavoriteCount.GetHashCode();
                if (this.Hashtags != null)
                    hashCode = hashCode * 59 + this.Hashtags.GetHashCode();
                if (this.HazardTypes != null)
                    hashCode = hashCode * 59 + this.HazardTypes.GetHashCode();
                hashCode = hashCode * 59 + this.Id.GetHashCode();
                if (this.InformationTypes != null)
                    hashCode = hashCode * 59 + this.InformationTypes.GetHashCode();
                hashCode = hashCode * 59 + this.Informative.GetHashCode();
                if (this.Lang != null)
                    hashCode = hashCode * 59 + this.Lang.GetHashCode();
                if (this.Location != null)
                    hashCode = hashCode * 59 + this.Location.GetHashCode();
                if (this.Media != null)
                    hashCode = hashCode * 59 + this.Media.GetHashCode();
                hashCode = hashCode * 59 + this.Quoted.GetHashCode();
                hashCode = hashCode * 59 + this.RetweetCount.GetHashCode();
                hashCode = hashCode * 59 + this.Retweeted.GetHashCode();
                if (this.Source != null)
                    hashCode = hashCode * 59 + this.Source.GetHashCode();
                if (this.Text != null)
                    hashCode = hashCode * 59 + this.Text.GetHashCode();
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
