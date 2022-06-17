using Abp.Dependency;
using Castle.Core;
using Castle.MicroKernel.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ermes.Attributes
{
    internal static class ErmesInterceptorRegistrar
    {
        public static void Initialize(IIocManager iocManager)
        {
            iocManager.IocContainer.Kernel.ComponentRegistered += (key, handler) =>
            {
                Type type = handler.ComponentModel.Implementation;
                if (ShouldIntercept(type))
                {
                    handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(ErmesAuthorizationInterceptor)));
                }
                if(type.IsSubclassOf(typeof(ErmesAppServiceBase)) || type.GetTypeInfo().IsDefined(typeof(ErmesAuthorizeAttribute), true))
                    handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(ErmesLoadUserInterceptor)));
                if (type.IsSubclassOf(typeof(ErmesAppServiceBase)) || type.GetTypeInfo().IsDefined(typeof(ErmesGamificationAttribute), true))
                    handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(ErmesGamificationInterceptor)));
            };
        }

        private static bool ShouldIntercept(Type type)
        {
            if (type.GetTypeInfo().IsDefined(typeof(ErmesAuthorizeAttribute), true))
            {
                return true;
            }

            if (type.GetMethods().Any(m => m.IsDefined(typeof(ErmesAuthorizeAttribute), true)))
            {
                return true;
            }

            return false;
        }
    }
}
