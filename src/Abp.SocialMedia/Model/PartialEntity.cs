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
    /// PartialEntity
    /// </summary>
    [DataContract(Name = "PartialEntity")]
    public partial class PartialEntity : IEquatable<PartialEntity>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartialEntity" /> class.
        /// </summary>
        /// <param name="area">area.</param>
        /// <param name="displayValue">displayValue.</param>
        /// <param name="end">end.</param>
        /// <param name="extra">extra.</param>
        /// <param name="id">id.</param>
        /// <param name="idStr">idStr.</param>
        /// <param name="labelId">labelId.</param>
        /// <param name="labelName">labelName.</param>
        /// <param name="location">location.</param>
        /// <param name="start">start.</param>
        /// <param name="value">value.</param>
        public PartialEntity(Object area = default(Object), string displayValue = default(string), int end = default(int), Object extra = default(Object), long id = default(long), string idStr = default(string), int labelId = default(int), string labelName = default(string), Object location = default(Object), int start = default(int), string value = default(string))
        {
            this.Area = area;
            this.DisplayValue = displayValue;
            this.End = end;
            this.Extra = extra;
            this.Id = id;
            this.IdStr = idStr;
            this.LabelId = labelId;
            this.LabelName = labelName;
            this.Location = location;
            this.Start = start;
            this.Value = value;
        }

        /// <summary>
        /// Gets or Sets Area
        /// </summary>
        [DataMember(Name = "area", EmitDefaultValue = true)]
        public Object Area { get; set; }

        /// <summary>
        /// Gets or Sets DisplayValue
        /// </summary>
        [DataMember(Name = "display_value", EmitDefaultValue = true)]
        public string DisplayValue { get; set; }

        /// <summary>
        /// Gets or Sets End
        /// </summary>
        [DataMember(Name = "end", EmitDefaultValue = true)]
        public int End { get; set; }

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
        /// Gets or Sets LabelId
        /// </summary>
        [DataMember(Name = "label_id", EmitDefaultValue = true)]
        public int LabelId { get; set; }

        /// <summary>
        /// Gets or Sets LabelName
        /// </summary>
        [DataMember(Name = "label_name", EmitDefaultValue = true)]
        public string LabelName { get; set; }

        /// <summary>
        /// Gets or Sets Location
        /// </summary>
        [DataMember(Name = "location", EmitDefaultValue = true)]
        public Object Location { get; set; }

        /// <summary>
        /// Gets or Sets Start
        /// </summary>
        [DataMember(Name = "start", EmitDefaultValue = true)]
        public int Start { get; set; }

        /// <summary>
        /// Gets or Sets Value
        /// </summary>
        [DataMember(Name = "value", EmitDefaultValue = true)]
        public string Value { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class PartialEntity {\n");
            sb.Append("  Area: ").Append(Area).Append("\n");
            sb.Append("  DisplayValue: ").Append(DisplayValue).Append("\n");
            sb.Append("  End: ").Append(End).Append("\n");
            sb.Append("  Extra: ").Append(Extra).Append("\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  IdStr: ").Append(IdStr).Append("\n");
            sb.Append("  LabelId: ").Append(LabelId).Append("\n");
            sb.Append("  LabelName: ").Append(LabelName).Append("\n");
            sb.Append("  Location: ").Append(Location).Append("\n");
            sb.Append("  Start: ").Append(Start).Append("\n");
            sb.Append("  Value: ").Append(Value).Append("\n");
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
            return this.Equals(input as PartialEntity);
        }

        /// <summary>
        /// Returns true if PartialEntity instances are equal
        /// </summary>
        /// <param name="input">Instance of PartialEntity to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(PartialEntity input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Area == input.Area ||
                    (this.Area != null &&
                    this.Area.Equals(input.Area))
                ) && 
                (
                    this.DisplayValue == input.DisplayValue ||
                    (this.DisplayValue != null &&
                    this.DisplayValue.Equals(input.DisplayValue))
                ) && 
                (
                    this.End == input.End ||
                    this.End.Equals(input.End)
                ) && 
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
                    this.LabelId == input.LabelId ||
                    this.LabelId.Equals(input.LabelId)
                ) && 
                (
                    this.LabelName == input.LabelName ||
                    (this.LabelName != null &&
                    this.LabelName.Equals(input.LabelName))
                ) && 
                (
                    this.Location == input.Location ||
                    (this.Location != null &&
                    this.Location.Equals(input.Location))
                ) && 
                (
                    this.Start == input.Start ||
                    this.Start.Equals(input.Start)
                ) && 
                (
                    this.Value == input.Value ||
                    (this.Value != null &&
                    this.Value.Equals(input.Value))
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
                if (this.Area != null)
                    hashCode = hashCode * 59 + this.Area.GetHashCode();
                if (this.DisplayValue != null)
                    hashCode = hashCode * 59 + this.DisplayValue.GetHashCode();
                hashCode = hashCode * 59 + this.End.GetHashCode();
                if (this.Extra != null)
                    hashCode = hashCode * 59 + this.Extra.GetHashCode();
                hashCode = hashCode * 59 + this.Id.GetHashCode();
                if (this.IdStr != null)
                    hashCode = hashCode * 59 + this.IdStr.GetHashCode();
                hashCode = hashCode * 59 + this.LabelId.GetHashCode();
                if (this.LabelName != null)
                    hashCode = hashCode * 59 + this.LabelName.GetHashCode();
                if (this.Location != null)
                    hashCode = hashCode * 59 + this.Location.GetHashCode();
                hashCode = hashCode * 59 + this.Start.GetHashCode();
                if (this.Value != null)
                    hashCode = hashCode * 59 + this.Value.GetHashCode();
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
