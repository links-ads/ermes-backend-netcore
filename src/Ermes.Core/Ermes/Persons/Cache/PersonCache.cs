using Abp.Dependency;
using Abp.Domain.Entities.Caching;
using Abp.Domain.Repositories;
using Abp.Modules;
using Abp.Runtime.Caching;
using Abp.Configuration;
using Abp.Reflection.Extensions;
using Abp.Runtime.Session;
using Abp.Configuration.Startup;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Ermes.Persons.Cache
{
    public class PersonCache : EntityCache<Person, PersonCacheDto, long>, ITransientDependency
    {
        public PersonCache(ICacheManager cacheManager, IRepository<Person, long> repository) : base(cacheManager, repository)
        {
        }
        protected override PersonCacheDto MapToCacheItem(Person p) 
        {
            return new PersonCacheDto(p);
        }
    }
}
