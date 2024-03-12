
namespace Abp.BusConsumer.Configuration
{
    public interface IBusConfigurationProvider
    {
        string GetConnectionString();        
        string GetGroupId();        
        string[] GetTopicList();
        bool IsEnabled();
        string GetHostname();
        string GetUsername();
        string GetPassword();
        int GetPort();
        string GetVirtualHost();
        string GetExchange();
        string GetQueue();
        bool MustDeclareBindings();

    }
}
