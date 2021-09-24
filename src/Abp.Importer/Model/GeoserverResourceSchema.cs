/*
 * Importer & Mapper API
 *
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: 0.1.0
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
using OpenAPIDateConverter = Abp.Importer.Client.OpenAPIDateConverter;

namespace Abp.Importer.Model
{
    /// <summary>
    /// Generic pydantic model in ORM mode by default, to deal with 90% of the use cases.     
    /// </summary>
    [DataContract(Name = "GeoserverResourceSchema")]
    public partial class GeoserverResourceSchema : IEquatable<GeoserverResourceSchema>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeoserverResourceSchema" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected GeoserverResourceSchema() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="GeoserverResourceSchema" /> class.
        /// </summary>
        /// <param name="datatypeId">datatypeId (required).</param>
        /// <param name="workspaceName">workspaceName (required).</param>
        /// <param name="storeName">storeName (required).</param>
        /// <param name="layerName">layerName (required).</param>
        /// <param name="storageLocation">storageLocation.</param>
        /// <param name="expireOn">expireOn.</param>
        /// <param name="start">start (required).</param>
        /// <param name="end">end (required).</param>
        /// <param name="resourceId">resourceId (required).</param>
        /// <param name="metadataId">metadataId (required).</param>
        /// <param name="bbox">bbox (required).</param>
        public GeoserverResourceSchema(string datatypeId = default(string), string workspaceName = default(string), string storeName = default(string), string layerName = default(string), string storageLocation = default(string), DateTime expireOn = default(DateTime), DateTime start = default(DateTime), DateTime end = default(DateTime), string resourceId = default(string), string metadataId = default(string), string bbox = default(string))
        {
            // to ensure "datatypeId" is required (not null)
            this.DatatypeId = datatypeId ?? throw new ArgumentNullException("datatypeId is a required property for GeoserverResourceSchema and cannot be null");
            // to ensure "workspaceName" is required (not null)
            this.WorkspaceName = workspaceName ?? throw new ArgumentNullException("workspaceName is a required property for GeoserverResourceSchema and cannot be null");
            // to ensure "storeName" is required (not null)
            this.StoreName = storeName ?? throw new ArgumentNullException("storeName is a required property for GeoserverResourceSchema and cannot be null");
            // to ensure "layerName" is required (not null)
            this.LayerName = layerName ?? throw new ArgumentNullException("layerName is a required property for GeoserverResourceSchema and cannot be null");
            this.Start = start;
            this.End = end;
            // to ensure "resourceId" is required (not null)
            this.ResourceId = resourceId ?? throw new ArgumentNullException("resourceId is a required property for GeoserverResourceSchema and cannot be null");
            // to ensure "metadataId" is required (not null)
            this.MetadataId = metadataId ?? throw new ArgumentNullException("metadataId is a required property for GeoserverResourceSchema and cannot be null");
            // to ensure "bbox" is required (not null)
            this.Bbox = bbox ?? throw new ArgumentNullException("bbox is a required property for GeoserverResourceSchema and cannot be null");
            this.StorageLocation = storageLocation;
            this.ExpireOn = expireOn;
        }

        /// <summary>
        /// Gets or Sets DatatypeId
        /// </summary>
        [DataMember(Name = "datatype_id", EmitDefaultValue = true)]
        public string DatatypeId { get; set; }

        /// <summary>
        /// Gets or Sets WorkspaceName
        /// </summary>
        [DataMember(Name = "workspace_name", EmitDefaultValue = true)]
        public string WorkspaceName { get; set; }

        /// <summary>
        /// Gets or Sets StoreName
        /// </summary>
        [DataMember(Name = "store_name", EmitDefaultValue = true)]
        public string StoreName { get; set; }

        /// <summary>
        /// Gets or Sets LayerName
        /// </summary>
        [DataMember(Name = "layer_name", EmitDefaultValue = true)]
        public string LayerName { get; set; }

        /// <summary>
        /// Gets or Sets StorageLocation
        /// </summary>
        [DataMember(Name = "storage_location", EmitDefaultValue = true)]
        public string StorageLocation { get; set; }

        /// <summary>
        /// Gets or Sets ExpireOn
        /// </summary>
        [DataMember(Name = "expire_on", EmitDefaultValue = true)]
        public DateTime ExpireOn { get; set; }

        /// <summary>
        /// Gets or Sets Start
        /// </summary>
        [DataMember(Name = "start", EmitDefaultValue = true)]
        public DateTime Start { get; set; }

        /// <summary>
        /// Gets or Sets End
        /// </summary>
        [DataMember(Name = "end", EmitDefaultValue = true)]
        public DateTime End { get; set; }

        /// <summary>
        /// Gets or Sets ResourceId
        /// </summary>
        [DataMember(Name = "resource_id", EmitDefaultValue = true)]
        public string ResourceId { get; set; }

        /// <summary>
        /// Gets or Sets MetadataId
        /// </summary>
        [DataMember(Name = "metadata_id", EmitDefaultValue = true)]
        public string MetadataId { get; set; }

        /// <summary>
        /// Gets or Sets Bbox
        /// </summary>
        [DataMember(Name = "bbox", EmitDefaultValue = true)]
        public string Bbox { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class GeoserverResourceSchema {\n");
            sb.Append("  DatatypeId: ").Append(DatatypeId).Append("\n");
            sb.Append("  WorkspaceName: ").Append(WorkspaceName).Append("\n");
            sb.Append("  StoreName: ").Append(StoreName).Append("\n");
            sb.Append("  LayerName: ").Append(LayerName).Append("\n");
            sb.Append("  StorageLocation: ").Append(StorageLocation).Append("\n");
            sb.Append("  ExpireOn: ").Append(ExpireOn).Append("\n");
            sb.Append("  Start: ").Append(Start).Append("\n");
            sb.Append("  End: ").Append(End).Append("\n");
            sb.Append("  ResourceId: ").Append(ResourceId).Append("\n");
            sb.Append("  MetadataId: ").Append(MetadataId).Append("\n");
            sb.Append("  Bbox: ").Append(Bbox).Append("\n");
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
            return this.Equals(input as GeoserverResourceSchema);
        }

        /// <summary>
        /// Returns true if GeoserverResourceSchema instances are equal
        /// </summary>
        /// <param name="input">Instance of GeoserverResourceSchema to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(GeoserverResourceSchema input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.DatatypeId == input.DatatypeId ||
                    (this.DatatypeId != null &&
                    this.DatatypeId.Equals(input.DatatypeId))
                ) && 
                (
                    this.WorkspaceName == input.WorkspaceName ||
                    (this.WorkspaceName != null &&
                    this.WorkspaceName.Equals(input.WorkspaceName))
                ) && 
                (
                    this.StoreName == input.StoreName ||
                    (this.StoreName != null &&
                    this.StoreName.Equals(input.StoreName))
                ) && 
                (
                    this.LayerName == input.LayerName ||
                    (this.LayerName != null &&
                    this.LayerName.Equals(input.LayerName))
                ) && 
                (
                    this.StorageLocation == input.StorageLocation ||
                    (this.StorageLocation != null &&
                    this.StorageLocation.Equals(input.StorageLocation))
                ) && 
                (
                    this.ExpireOn == input.ExpireOn ||
                    (this.ExpireOn != null &&
                    this.ExpireOn.Equals(input.ExpireOn))
                ) && 
                (
                    this.Start == input.Start ||
                    (this.Start != null &&
                    this.Start.Equals(input.Start))
                ) && 
                (
                    this.End == input.End ||
                    (this.End != null &&
                    this.End.Equals(input.End))
                ) && 
                (
                    this.ResourceId == input.ResourceId ||
                    (this.ResourceId != null &&
                    this.ResourceId.Equals(input.ResourceId))
                ) && 
                (
                    this.MetadataId == input.MetadataId ||
                    (this.MetadataId != null &&
                    this.MetadataId.Equals(input.MetadataId))
                ) && 
                (
                    this.Bbox == input.Bbox ||
                    (this.Bbox != null &&
                    this.Bbox.Equals(input.Bbox))
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
                if (this.DatatypeId != null)
                    hashCode = hashCode * 59 + this.DatatypeId.GetHashCode();
                if (this.WorkspaceName != null)
                    hashCode = hashCode * 59 + this.WorkspaceName.GetHashCode();
                if (this.StoreName != null)
                    hashCode = hashCode * 59 + this.StoreName.GetHashCode();
                if (this.LayerName != null)
                    hashCode = hashCode * 59 + this.LayerName.GetHashCode();
                if (this.StorageLocation != null)
                    hashCode = hashCode * 59 + this.StorageLocation.GetHashCode();
                if (this.ExpireOn != null)
                    hashCode = hashCode * 59 + this.ExpireOn.GetHashCode();
                if (this.Start != null)
                    hashCode = hashCode * 59 + this.Start.GetHashCode();
                if (this.End != null)
                    hashCode = hashCode * 59 + this.End.GetHashCode();
                if (this.ResourceId != null)
                    hashCode = hashCode * 59 + this.ResourceId.GetHashCode();
                if (this.MetadataId != null)
                    hashCode = hashCode * 59 + this.MetadataId.GetHashCode();
                if (this.Bbox != null)
                    hashCode = hashCode * 59 + this.Bbox.GetHashCode();
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
