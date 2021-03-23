using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;

namespace Abp.SocialMedia.Dto
{
    //this class should be made available by the sdk
    //at the moment has been creted by hand, but should be replaced by an auto-generated class
    public class GetEventsQuery
    {
        /// <summary>
        /// Defines Languages
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum LanguagesEnum
        {
            /// <summary>
            /// Enum En for value: en
            /// </summary>
            [EnumMember(Value = "en")]
            En = 1,

            /// <summary>
            /// Enum It for value: it
            /// </summary>
            [EnumMember(Value = "it")]
            It = 2,

            /// <summary>
            /// Enum Es for value: es
            /// </summary>
            [EnumMember(Value = "es")]
            Es = 3,

            /// <summary>
            /// Enum Tr for value: tr
            /// </summary>
            [EnumMember(Value = "tr")]
            Tr = 4,

            /// <summary>
            /// Enum Hr for value: hr
            /// </summary>
            [EnumMember(Value = "hr")]
            Hr = 5,

            /// <summary>
            /// Enum Nl for value: nl
            /// </summary>
            [EnumMember(Value = "nl")]
            Nl = 6,

            /// <summary>
            /// Enum Fi for value: fi
            /// </summary>
            [EnumMember(Value = "fi")]
            Fi = 7,

            /// <summary>
            /// Enum El for value: el
            /// </summary>
            [EnumMember(Value = "el")]
            El = 8,

            /// <summary>
            /// Enum Fr for value: fr
            /// </summary>
            [EnumMember(Value = "fr")]
            Fr = 9

        }

        /// <summary>
        /// Gets or Sets Limit
        /// </summary>
        [DataMember(Name = "limit", EmitDefaultValue = true)]
        public int? Limit { get; set; }

        /// <summary>
        /// Gets or Sets Page
        /// </summary>
        [DataMember(Name = "page", EmitDefaultValue = true)]
        public int? Page { get; set; }


        /// <summary>
        /// Gets or Sets Languages
        /// </summary>
        [DataMember(Name = "languages", EmitDefaultValue = true)]
        public List<LanguagesEnum> Languages { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="GetEventsQuery" /> class.
        /// </summary>
        /// <param name="end">end.</param>
        /// <param name="hazards">hazards.</param>
        /// <param name="infotypes">infotypes.</param>
        /// <param name="languages">languages.</param>
        /// <param name="northEast">northEast.</param>
        /// <param name="southWest">southWest.</param>
        /// <param name="start">start.</param>
        /// <param name="page">page.</param>
        /// <param name="limit">limit.</param>
        public GetEventsQuery(DateTime? end = default(DateTime?), List<int> hazards = default(List<int>), List<int> infotypes = default(List<int>), List<LanguagesEnum> languages = default(List<LanguagesEnum>), List<float> northEast = default(List<float>), List<float> southWest = default(List<float>), DateTime? start = default(DateTime?), int? limit = default(int?), int? page = default(int?))
        {
            this.End = end;
            this.Hazards = hazards;
            this.Infotypes = infotypes;
            this.Languages = languages;
            this.NorthEast = northEast;
            this.SouthWest = southWest;
            this.Start = start;
            this.Limit = limit;
            this.Page = page;

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
        /// Gets or Sets Infotypes
        /// </summary>
        [DataMember(Name = "infotypes", EmitDefaultValue = true)]
        public List<int> Infotypes { get; set; }

        /// <summary>
        /// Gets or Sets NorthEast
        /// </summary>
        [DataMember(Name = "north_east", EmitDefaultValue = true)]
        public List<float> NorthEast { get; set; }

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
    }
}
