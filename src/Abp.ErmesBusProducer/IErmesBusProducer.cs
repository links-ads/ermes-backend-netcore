using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Abp.ErmesBusProducer
{
    public interface IErmesBusProducer: ISingletonDependency
    {
        Task<bool> Publish(string topic, string message);
    }
}
