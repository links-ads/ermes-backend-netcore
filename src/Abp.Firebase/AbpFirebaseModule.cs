using Abp.Modules;
using System;
using System.Reflection;

namespace Abp.Firebase
{
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpFirebaseModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IFirebaseManager, FirebaseManager>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
