using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ermes.EntityHistory
{
    public interface IEntityHistoryHelper
    {
        SplitEntityChangeSet CreateEntityChangeSet(ICollection<EntityEntry> entityEntries);

        Task SaveAsync(SplitEntityChangeSet changeSet);

        void Save(SplitEntityChangeSet changeSet);
    }
}
