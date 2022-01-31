using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Importer
{
    public interface IImporterManager
    {
        Task<object> GetLayers(List<string> datatype_ids, string bbox, DateTime start, DateTime end, string request_code);
    }
}
