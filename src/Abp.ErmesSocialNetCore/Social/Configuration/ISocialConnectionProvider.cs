
namespace Abp.ErmesSocialNetCore.Social.Configuration
{
    public interface ISocialConnectionProvider
    {
        string GetApiSecret();
        string GetApiKey();
        string GetBasePath();
    }
}
