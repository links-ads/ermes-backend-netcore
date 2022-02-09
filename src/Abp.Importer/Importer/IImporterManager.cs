using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Importer
{
    public interface IImporterManager
    {
        Task<object> GetLayers(List<string> datatype_ids, string bbox, DateTime start, DateTime end, string request_code, bool includeMapRequest);
        Task<object> GetTimeSeries(string datatype_id, string point, string attribute, DateTime start, DateTime end);
    }
}
