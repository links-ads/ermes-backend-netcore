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
using OpenAPIDateConverter = Abp.ErmesSocialNetCore.Client.OpenAPIDateConverter;

namespace Abp.ErmesSocialNetCore.Model
{
    /// <summary>
    /// LabelQuery
    /// </summary>
    [DataContract(Name = "LabelQuery")]
    public partial class LabelQuery : IEquatable<LabelQuery>, IValidatableObject
    {
        /// <summary>
        /// Defines Task
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum TaskEnum
        {
            /// <summary>
            /// Enum Hazardtype for value: hazard_type
            /// </summary>
            [EnumMember(Value = "hazard_type")]
            hazard_type = 1,

            /// <summary>
            /// Enum Informationtype for value: information_type
            /// </summary>
            [EnumMember(Value = "information_type")]
            information_type = 2,

            /// <summary>
            /// Enum Namedentity for value: named_entity
            /// </summary>
            [EnumMember(Value = "named_entity")]
            named_entity = 3

        }

        /// <summary>
        /// Gets or Sets Task
        /// </summary>
        [DataMember(Name = "task", EmitDefaultValue = true)]
        public TaskEnum? Task { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="LabelQuery" /> class.
        /// </summary>
        /// <param name="operational">operational.</param>
        /// <param name="task">task.</param>
        public LabelQuery(bool? operational = default(bool?), TaskEnum? task = default(TaskEnum?))
        {
            this.Operational = operational;
            this.Task = task;
        }

        /// <summary>
        /// Gets or Sets Operational
        /// </summary>
        [DataMember(Name = "operational", EmitDefaultValue = true)]
        public bool? Operational { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class LabelQuery {\n");
            sb.Append("  Operational: ").Append(Operational).Append("\n");
            sb.Append("  Task: ").Append(Task).Append("\n");
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
            return this.Equals(input as LabelQuery);
        }

        /// <summary>
        /// Returns true if LabelQuery instances are equal
        /// </summary>
        /// <param name="input">Instance of LabelQuery to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(LabelQuery input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Operational == input.Operational ||
                    (this.Operational != null &&
                    this.Operational.Equals(input.Operational))
                ) && 
                (
                    this.Task == input.Task ||
                    this.Task.Equals(input.Task)
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
                if (this.Operational != null)
                    hashCode = hashCode * 59 + this.Operational.GetHashCode();
                hashCode = hashCode * 59 + this.Task.GetHashCode();
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
