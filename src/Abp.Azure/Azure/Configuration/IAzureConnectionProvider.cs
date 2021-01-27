
namespace Abp.Azure.Configuration
{
    public interface IAzureConnectionProvider
    {
        string GetStorageConnectionString();
        string GetStorageBasePath();
    }
}
