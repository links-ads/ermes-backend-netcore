using Abp.Domain.Entities;
using Abp.EntityFrameworkCore;
using Abp.EntityFrameworkCore.Repositories;

namespace Ermes.EntityFrameworkCore.Repositories
{
    //Base class for all repositories in my application
    public class ErmesRepositoryBase<TEntity, TPrimaryKey> : EfCoreRepositoryBase<ErmesDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        public ErmesRepositoryBase(IDbContextProvider<ErmesDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        //add common methods for all repositories
    }

    //A shortcut for entities those have integer Id
    public class ErmesRepositoryBase<TEntity> : ErmesRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        public ErmesRepositoryBase(IDbContextProvider<ErmesDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        //do not add methods here, add them to the class above (because this class inherits it)
    }
}
