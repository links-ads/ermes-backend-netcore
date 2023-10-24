using Castle.DynamicProxy;

namespace Ermes.Attributes
{
    internal class ErmesLoadUserInterceptor : IInterceptor
    {
        private readonly ErmesAppSession _session;
        public ErmesLoadUserInterceptor(ErmesAppSession session)
        {
            _session = session;
        }

        public void Intercept(IInvocation invocation)
        {
            if (invocation.Method.IsPublic)
                _session.LoadUserId();
            invocation.Proceed();
        }
    }
}
