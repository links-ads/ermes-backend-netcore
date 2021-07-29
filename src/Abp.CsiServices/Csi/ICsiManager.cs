using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Abp.CsiServices.Csi
{
    public interface ICsiManager 
    {
        Task<int> SearchVolontarioAsync(string taxCode);
    }
}
