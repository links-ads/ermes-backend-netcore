using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ermes.EntityHistory
{
    /// <summary>
    /// Implements <see cref="IEntityHistoryStore"/> to save entity change informations to database.
    /// </summary>
    public class EntityHistoryManager : ITransientDependency
    {
        private readonly IRepository<SplitEntityChangeSet, long> _changeSetRepository;
        private readonly IRepository<SplitEntityChange, long> _changeRepository;

        public IQueryable<SplitEntityChange> EntityChanges
        {
            get
            {
                return _changeRepository.GetAllIncluding(ec => ec.PropertyChanges);
            }
        }

        public IQueryable<SplitEntityChangeSet> EntityChangeSets
        {
            get
            {
                return _changeSetRepository.GetAll();
            }
        }

        /// <summary>
        /// Creates a new <see cref="EntityHistoryManager"/>.
        /// </summary>
        public EntityHistoryManager(IRepository<SplitEntityChangeSet, long> changeSetRepository, IRepository<SplitEntityChange, long> changeRepository)
        {
            _changeSetRepository = changeSetRepository;
            _changeRepository = changeRepository;
        }

        public virtual Task SaveAsync(SplitEntityChangeSet changeSet)
        {
            return _changeSetRepository.InsertAsync(changeSet);
        }

        public virtual void Save(SplitEntityChangeSet changeSet)
        {
            _changeSetRepository.Insert(changeSet);
        }
    }
}
