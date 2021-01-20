using System;
using System.Threading.Tasks;
using Abp.TestBase;
using Ermes.EntityFrameworkCore;
using Ermes.Tests.TestDatas;

namespace Ermes.Tests
{
    public class ErmesTestBase : AbpIntegratedTestBase<ErmesTestModule>
    {
        public ErmesTestBase()
        {
            UsingDbContext(context => new TestDataBuilder(context).Build());
        }

        protected virtual void UsingDbContext(Action<ErmesDbContext> action)
        {
            using (var context = LocalIocManager.Resolve<ErmesDbContext>())
            {
                action(context);
                context.SaveChanges();
            }
        }

        protected virtual T UsingDbContext<T>(Func<ErmesDbContext, T> func)
        {
            T result;

            using (var context = LocalIocManager.Resolve<ErmesDbContext>())
            {
                result = func(context);
                context.SaveChanges();
            }

            return result;
        }

        protected virtual async Task UsingDbContextAsync(Func<ErmesDbContext, Task> action)
        {
            using (var context = LocalIocManager.Resolve<ErmesDbContext>())
            {
                await action(context);
                await context.SaveChangesAsync(true);
            }
        }

        protected virtual async Task<T> UsingDbContextAsync<T>(Func<ErmesDbContext, Task<T>> func)
        {
            T result;

            using (var context = LocalIocManager.Resolve<ErmesDbContext>())
            {
                result = await func(context);
                context.SaveChanges();
            }

            return result;
        }
    }
}
