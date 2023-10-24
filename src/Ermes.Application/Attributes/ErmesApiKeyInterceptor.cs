using Abp.UI;
using Castle.DynamicProxy;

namespace Ermes.Attributes
{
    public class ErmesApiKeyInterceptor : IInterceptor
    {
        private readonly ErmesAppSession _session;
        public ErmesApiKeyInterceptor(
                ErmesAppSession session
            )
        {
            _session = session;
        }

        public void Intercept(IInvocation invocation)
        {
            if (_session.ApiKey == null || _session.ApiKey == string.Empty)
                throw new UserFriendlyException("InvalidApiKey");
            invocation.Proceed();
        }
    }
}
