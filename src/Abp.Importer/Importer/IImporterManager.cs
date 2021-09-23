using Abp.Importer.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Importer
{
    public interface IImporterManager
    {
        Task<List<string>> GetLayers();
    }
}
