
namespace Abp.BusProducer.Configuration
{
    public interface IBusConfigurationProvider
    {
        string GetConnectionString();        
        string GetHostname();        
        string GetUsername();        
        string GetPassword();
        int GetPort();
        string GetVirtualHost();
        string GetExchange();
    }
}
