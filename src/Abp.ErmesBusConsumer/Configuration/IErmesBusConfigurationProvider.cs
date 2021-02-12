
namespace Abp.ErmesBusConsumer.Configuration
{
    public interface IErmesBusConfigurationProvider
    {
        string GetConnectionString();        
        string GetGroupId();        
        string[] GetTopicList();        
    }
}
