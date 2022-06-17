using Abp.UI;
using Castle.DynamicProxy;
using Ermes.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ermes.Attributes
{
    public class ErmesGamificationInterceptor: IInterceptor
    {
        private readonly ErmesAppSession _session;

        public ErmesGamificationInterceptor(ErmesAppSession session)
        {
            _session = session;
        }

        public void Intercept(IInvocation invocation)
        {            
            //Only citizens take part to gamification
            if (!_session.Roles.Any(r => r == AppRoles.CITIZEN))
                throw new UserFriendlyException("DoNotTakePartToGamification");
            invocation.Proceed();
        }
    }
}
