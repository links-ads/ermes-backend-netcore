using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.ExternalServices.Csi
{
    public interface ICsiManager
    {
        Task<int> SearchVolontarioAsync(string taxCode, long personId);
    }
}
