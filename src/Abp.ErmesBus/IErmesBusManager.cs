using System;
using System.Threading.Tasks;

namespace Abp.Bus
{
    public interface IErmesBusManager
    {
        public Task<bool> Publish(string topic, string message);
    }
}
