using Abp.Authorization;
using Castle.DynamicProxy;
using Ermes.Helpers;
using System;

namespace Ermes.Attributes
{
    internal class ErmesAuthorizationInterceptor : IInterceptor
    {
        private readonly ErmesAuthorizationHelper _authorizationHelper;
        public ErmesAuthorizationInterceptor(
                //ErmesAppSession session,
                ErmesAuthorizationHelper authorizationHelper
            )
        {
            //_session = session;
            _authorizationHelper = authorizationHelper;
        }

        public void Intercept(IInvocation invocation)
        {
            _authorizationHelper.Authorize(invocation.MethodInvocationTarget, invocation.TargetType);
            invocation.Proceed();
        }
    }
}
