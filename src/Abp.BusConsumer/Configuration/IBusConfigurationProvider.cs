
namespace Abp.BusConsumer.Configuration
{
    public interface IBusConfigurationProvider
    {
        string GetConnectionString();        
        string GetGroupId();        
        string[] GetTopicList();
        bool IsEnabled();
    }
}
