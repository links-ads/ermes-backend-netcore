using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Importer
{
    public interface IImporterManager
    {
        Task<object> GetLayers(List<string> datatype_ids, string bbox, DateTime start, DateTime end, List<string> request_codes, bool includeMapRequest);
        Task<object> GetTimeSeries(string datatype_id, string point, string request_code = null, string layer_name = null, DateTime? start = null, DateTime? end = null);
        Task<object> GetMetadata(string metadata_id);
        Task<List<string>> DeleteMapRequests(List<string> mapRequestCodes);
    }
}
