using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.EntityHistory.Extensions;
using Abp.Events.Bus.Entities;
using Abp.Extensions;
using Ermes.Communications;
using Ermes.Preferences;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Abp.Json;
using Newtonsoft.Json;
using System.Text.Json;
using NpgsqlTypes;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Abp.EntityHistory;

namespace Ermes.EntityHistory
{
    public class EntityHistoryHelper : EntityHistoryHelperBase, IEntityHistoryHelper, ITransientDependency
    {
        private readonly EntityHistoryManager _entityHistoryManager;

        public static readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public EntityHistoryHelper(
            IEntityHistoryConfiguration configuration,
            IUnitOfWorkManager unitOfWorkManager,
            EntityHistoryManager entityHistoryManager)
            : base(configuration, unitOfWorkManager)
        {
            _entityHistoryManager = entityHistoryManager;
        }
               

        public virtual SplitEntityChangeSet CreateEntityChangeSet(ICollection<EntityEntry> entityEntries)
        {
            var changeSet = new SplitEntityChangeSet
            {
                Reason = EntityChangeSetReasonProvider.Reason.TruncateWithPostfix(SplitEntityChangeSet.MaxReasonLength),

                // Fill "who did this change"
                BrowserInfo = ClientInfoProvider.BrowserInfo.TruncateWithPostfix(SplitEntityChangeSet.MaxBrowserInfoLength),
                ClientName = ClientInfoProvider.ComputerName.TruncateWithPostfix(SplitEntityChangeSet.MaxClientNameLength),
                ImpersonatorTenantId = AbpSession.ImpersonatorTenantId,
                ImpersonatorUserId = AbpSession.ImpersonatorUserId,
                TenantId = AbpSession.TenantId,
                UserId = AbpSession.UserId
            };

            if (!IsEntityHistoryEnabled)
            {
                return changeSet;
            }

            foreach (var entityEntry in entityEntries)
            {
                if (entityEntry.Entity is SplitEntityChange || entityEntry.Entity is SplitEntityChangeSet || entityEntry.Entity is SplitEntityPropertyChange)
                    continue;

                var typeOfEntity = entityEntry.Entity.GetType();
                var shouldTrackEntity = IsTypeOfTrackedEntity(typeOfEntity);
                if (shouldTrackEntity.HasValue && !shouldTrackEntity.Value)
                {
                    continue;
                }

                if (!IsTypeOfEntity(typeOfEntity) && !entityEntry.Metadata.IsOwned())
                {
                    continue;
                }

                var shouldAuditEntity = IsTypeOfAuditedEntity(typeOfEntity);
                if (shouldAuditEntity.HasValue && !shouldAuditEntity.Value)
                {
                    continue;
                }

                bool? shouldAuditOwnerEntity = null;
                bool? shouldAuditOwnerProperty = null;
                if (!shouldAuditEntity.HasValue && entityEntry.Metadata.IsOwned())
                {
                    // Check if owner entity has auditing attribute
                    var ownerForeignKey = entityEntry.Metadata.GetForeignKeys().First(fk => fk.IsOwnership);
                    var ownerEntityType = ownerForeignKey.PrincipalEntityType.ClrType;

                    shouldAuditOwnerEntity = IsTypeOfAuditedEntity(ownerEntityType);
                    if (shouldAuditOwnerEntity.HasValue && !shouldAuditOwnerEntity.Value)
                    {
                        continue;
                    }

                    var ownerPropertyInfo = ownerForeignKey.PrincipalToDependent.PropertyInfo;
                    shouldAuditOwnerProperty = IsAuditedPropertyInfo(ownerPropertyInfo);
                    if (shouldAuditOwnerProperty.HasValue && !shouldAuditOwnerProperty.Value)
                    {
                        continue;
                    }
                }

                var entityChange = CreateEntityChange(entityEntry);
                if (entityChange == null)
                {
                    continue;
                }

                var shouldSaveAuditedPropertiesOnly = false;
                var propertyChanges = GetPropertyChanges(entityEntry, shouldSaveAuditedPropertiesOnly);
                if (propertyChanges.Count == 0)
                {
                    continue;
                }

                entityChange.PropertyChanges = propertyChanges;
                changeSet.EntityChanges.Add(entityChange);
            }

            return changeSet;
        }

        public virtual async Task SaveAsync(SplitEntityChangeSet changeSet)
        {
            if (!IsEntityHistoryEnabled)
            {
                return;
            }

            UpdateChangeSet(changeSet);

            if (changeSet.EntityChanges.Count == 0)
            {
                return;
            }

            using (var uow = UnitOfWorkManager.Begin(TransactionScopeOption.Suppress))
            {
                await _entityHistoryManager.SaveAsync(changeSet);
                await uow.CompleteAsync();
            }
        }

        public virtual void Save(SplitEntityChangeSet changeSet)
        {
            if (!IsEntityHistoryEnabled)
            {
                return;
            }

            UpdateChangeSet(changeSet);

            if (changeSet.EntityChanges.Count == 0)
            {
                return;
            }

            using (var uow = UnitOfWorkManager.Begin(TransactionScopeOption.Suppress))
            {
                _entityHistoryManager.Save(changeSet);
                uow.Complete();
            }
        }

        protected virtual string GetEntityId(EntityEntry entry)
        {
            var primaryKeys = entry.Properties.Where(p => p.Metadata.IsPrimaryKey());
            return primaryKeys.First().CurrentValue?.ToJsonString();
        }

        [CanBeNull]
        private SplitEntityChange CreateEntityChange(EntityEntry entityEntry)
        {
            var entityId = GetEntityId(entityEntry);
            var entityTypeFullName = entityEntry.Entity.GetType().FullName;
            EntityChangeType changeType;
            switch (entityEntry.State)
            {
                case EntityState.Added:
                    changeType = EntityChangeType.Created;
                    break;
                case EntityState.Deleted:
                    changeType = EntityChangeType.Deleted;
                    break;
                case EntityState.Modified:
                    changeType = entityEntry.IsDeleted() ? EntityChangeType.Deleted : EntityChangeType.Updated;
                    break;
                case EntityState.Detached:
                case EntityState.Unchanged:
                    return null;
                default:
                    Logger.ErrorFormat("Unexpected {0} - {1}", nameof(entityEntry.State), entityEntry.State);
                    return null;
            }

            if (entityId == null && changeType != EntityChangeType.Created)
            {
                Logger.ErrorFormat("EntityChangeType {0} must have non-empty entity id", changeType);
                return null;
            }

            return new SplitEntityChange
            {
                ChangeType = changeType,
                EntityEntry = entityEntry, // [NotMapped]
                EntityId = entityId,
                EntityTypeName = entityTypeFullName.Split('.')[^1],
                TenantId = AbpSession.TenantId
            };
        }

        /// <summary>
        /// Gets the property changes for this entry.
        /// </summary>
        private ICollection<SplitEntityPropertyChange> GetPropertyChanges(EntityEntry entityEntry, bool auditedPropertiesOnly)
        {
            var propertyChanges = new List<SplitEntityPropertyChange>();
            var properties = entityEntry.Metadata.GetProperties();

            foreach (var property in properties)
            {
                if (property.IsPrimaryKey())
                {
                    continue;
                }

                var shouldSaveProperty = property.IsShadowProperty() // i.e. property.PropertyInfo == null
                    ? !auditedPropertiesOnly
                    : IsAuditedPropertyInfo(property.PropertyInfo) ?? !auditedPropertiesOnly;

                if (shouldSaveProperty)
                {
                    var propertyEntry = entityEntry.Property(property.Name);

                    foreach (SplitEntityPropertyChange ec in CreateEntityPropertyChange(propertyEntry.GetOriginalValue(), propertyEntry.GetNewValue(), property))
                        propertyChanges.Add(ec);
                }
            }

            return propertyChanges;
        }

        /// <summary>
        /// Updates change time, entity id, Adds foreign keys, Removes/Updates property changes after SaveChanges is called.
        /// </summary>
        private void UpdateChangeSet(SplitEntityChangeSet changeSet)
        {
            var entityChangesToRemove = new List<SplitEntityChange>();
            foreach (var entityChange in changeSet.EntityChanges)
            {
                var entityEntry = entityChange.EntityEntry.As<EntityEntry>();
                var isAuditedEntity = IsTypeOfAuditedEntity(entityEntry.Entity.GetType()) == true;

                /* Update change time */
                entityChange.ChangeTime = GetChangeTime(entityChange.ChangeType, entityEntry.Entity);

                /* Update entity id */
                entityChange.EntityId = GetEntityId(entityEntry);

                /* Update property changes */
                var trackedPropertyNames = entityChange.PropertyChanges.Select(pc => pc.PropertyName);
                var trackedNavigationProperties = entityEntry.Navigations
                                                    .Where(np => trackedPropertyNames.Contains(np.Metadata.Name))
                                                    .ToList();
                var additionalForeignKeys = trackedNavigationProperties
                                                  .Where(np => !trackedPropertyNames.Contains(np.Metadata.Name))
                                                  .Select(np => np.Metadata.ForeignKey)
                                                  .Distinct()
                                                  .ToList();

                /* Add additional foreign keys from navigation properties */
                foreach (var foreignKey in additionalForeignKeys)
                {
                    foreach (var property in foreignKey.Properties)
                    {
                        var shouldSaveProperty = property.IsShadowProperty() ?
                                                   null :
                                                   IsAuditedPropertyInfo(property.PropertyInfo);
                        if (shouldSaveProperty.HasValue && !shouldSaveProperty.Value)
                        {
                            continue;
                        }

                        var propertyEntry = entityEntry.Property(property.Name);
                        // TODO: fix new value comparison before truncation
                        var newValue = propertyEntry.GetNewValue()?? null;
                        var oldValue = propertyEntry.GetOriginalValue()?? null;
                        // Add foreign key
                        foreach(SplitEntityPropertyChange ec in CreateEntityPropertyChange(oldValue, newValue, property))
                            entityChange.PropertyChanges.Add(ec);
                    }
                }

                /* Update/Remove property changes */
                var propertyChangesToRemove = new List<SplitEntityPropertyChange>();
                foreach (var propertyChange in entityChange.PropertyChanges)
                {
                    var propertyEntry = entityEntry.Property(propertyChange.PropertyName);
                    var isAuditedProperty = !propertyEntry.Metadata.IsShadowProperty() &&
                                            IsAuditedPropertyInfo(propertyEntry.Metadata.PropertyInfo) == true;

                    // TODO: fix new value comparison before truncation
                    // WARNING: Hereby I assume that truncated properties do not get changed between the CreateEntityChangeSet and the Save
                    if (propertyChange.SplitIndex == null)
                    {
                        propertyChange.NewValue = TransformAndSerialize(propertyEntry.GetNewValue());
                        if (!isAuditedProperty && propertyChange.OriginalValue == propertyChange.NewValue)
                        {
                            // No change
                            propertyChangesToRemove.Add(propertyChange);
                        }
                    }
                }

                foreach (var propertyChange in propertyChangesToRemove)
                {
                    entityChange.PropertyChanges.Remove(propertyChange);
                }

                if (!isAuditedEntity && entityChange.PropertyChanges.Count == 0)
                {
                    entityChangesToRemove.Add(entityChange);
                }
            }

            foreach (var entityChange in entityChangesToRemove)
            {
                changeSet.EntityChanges.Remove(entityChange);
            }
        }

        private String TransformAndSerialize(object value)
        {
            if (value == null)
                return null;
            else if (value is DateTime dt)
            {
                if (dt.Kind != DateTimeKind.Utc && dt.Kind != DateTimeKind.Unspecified)
                    dt = dt.ToUniversalTime();
                return dt.ToJsonString(serializerSettings).Replace("Z", "");
            }
            else if (value is NpgsqlRange<DateTime> range)
            {
                NpgsqlRange<DateTime> value_utc;
                if (range.UpperBound.Kind != DateTimeKind.Utc && range.UpperBound.Kind != DateTimeKind.Unspecified)
                    value_utc = new NpgsqlRange<DateTime>(range.LowerBound.ToUniversalTime(), range.LowerBoundIsInclusive, range.UpperBound.ToUniversalTime(), range.UpperBoundIsInclusive);
                else
                    value_utc = range;
                return value_utc.ToJsonString(serializerSettings).Replace("Z", "");
            }
            else if (value is Geometry)
                return new GeoJsonWriter().Write(value);
            else
                return value.ToJsonString(serializerSettings);
        }

        private List<String> Split(string value, int maxvaluelength)
        {
            List<String> retval = new List<string>();
            if (value == null)
                return retval;

            while (value.Length > maxvaluelength)
            {
                string thisplit = value.Substring(0, maxvaluelength);
                value = value.Substring(thisplit.Length);
                retval.Add(thisplit);
            }
            retval.Add(value);
            return retval;
        }

        private List<SplitEntityPropertyChange> CreateEntityPropertyChange(object oldValue, object newValue, IProperty property)
        {
            List<SplitEntityPropertyChange> retval = new List<SplitEntityPropertyChange>();
            if (newValue == oldValue)
                return retval;
            string serialized_old = TransformAndSerialize(oldValue);
            string serialized_new = TransformAndSerialize(newValue);
            if (serialized_new == serialized_old)
                return retval;
            List<String> oldValues = Split(serialized_old, SplitEntityPropertyChange.MaxValueLength);
            List<String> newValues = Split(serialized_new, SplitEntityPropertyChange.MaxValueLength);
            for(int split=0; split < Math.Max(oldValues.Count, newValues.Count); split++)
            {
                retval.Add(new SplitEntityPropertyChange()
                {
                    OriginalValue = split < oldValues.Count ? oldValues[split] : null,
                    NewValue = split < newValues.Count ? newValues[split] : null,
                    PropertyName = property.Name.TruncateWithPostfix(SplitEntityPropertyChange.MaxPropertyNameLength),
                    PropertyTypeFullName = property.ClrType.FullName.TruncateWithPostfix(SplitEntityPropertyChange.MaxPropertyTypeFullNameLength),
                    TenantId = AbpSession.TenantId,
                    SplitIndex = (oldValues.Count > 1 || newValues.Count > 1) ? (int?)split : null
                });
            }
            return retval;
        }
    }
}
