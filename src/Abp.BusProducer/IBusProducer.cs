using Abp.Dependency;
using System.Threading.Tasks;

namespace Abp.BusProducer
{
    public interface IBusProducer: ISingletonDependency
    {
        Task<bool> Publish(string topic, string message);
    }
}
