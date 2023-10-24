using Castle.DynamicProxy;
using Ermes.Helpers;

namespace Ermes.Attributes
{
    internal class ErmesAuthorizationInterceptor : IInterceptor
    {
        private readonly ErmesAuthorizationHelper _authorizationHelper;
        public ErmesAuthorizationInterceptor(
                ErmesAuthorizationHelper authorizationHelper
            )
        {
            _authorizationHelper = authorizationHelper;
        }

        public void Intercept(IInvocation invocation)
        {
            _authorizationHelper.Authorize(invocation.MethodInvocationTarget, invocation.TargetType);
            invocation.Proceed();
        }
    }
}
