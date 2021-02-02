using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Dto.Datatable
{
    //public class DTColumn
    //{
    //    /// <summary>
    //    /// Column's data source, as defined by columns.data.
    //    /// </summary>
    //    public string Data { get; set; }

    //    /// <summary>
    //    /// Column's name, as defined by columns.name.
    //    /// </summary>
    //    public string Name { get; set; }

    //    /// <summary>
    //    /// Flag to indicate if this column is searchable (true) or not (false). This is controlled by columns.searchable.
    //    /// </summary>
    //    public bool Searchable { get; set; }

    //    /// <summary>
    //    /// Flag to indicate if this column is orderable (true) or not (false). This is controlled by columns.orderable.
    //    /// </summary>
    //    public bool Orderable { get; set; }

    //    /// <summary>
    //    /// Specific search value.
    //    /// </summary>
    //    public DTSearch Search { get; set; }
    //}

    /// <summary>
    /// An order, as sent by jQuery DataTables when doing AJAX queries.
    /// </summary>
    public class DTOrder
    {
        /// <summary>
        /// Column to which ordering should be applied.
        /// This is an index reference to the columns array of information that is also submitted to the server.
        /// </summary>
        public string Column { get; set; }

        /// <summary>
        /// Ordering direction for this column.
        /// It will be dt-string asc or dt-string desc to indicate ascending ordering or descending ordering, respectively.
        /// </summary>
        public DTOrderDir Dir { get; set; }
    }

    /// <summary>
    /// Sort orders of jQuery DataTables.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DTOrderDir
    {
        ASC,
        DESC
    }

    /// <summary>
    /// A search, as sent by jQuery DataTables when doing AJAX queries.
    /// </summary>
    public class DTSearch
    {
        /// <summary>
        /// Global search value. To be applied to all columns which have searchable as true.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// true if the global filter should be treated as a regular expression for advanced searching, false otherwise.
        /// Note that normally server-side processing scripts will not perform regular expression searching for performance reasons on large data sets, but it is technically possible and at the discretion of your script.
        /// </summary>
        public bool Regex { get; set; }
    }
}
