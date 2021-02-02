using Abp.EntityFrameworkCore;
using Ermes.Persons;
using Ermes.Organizations;
using Microsoft.EntityFrameworkCore;
using Ermes.Activities;
using Ermes.Migrations.Seed;
using Ermes.CompetenceAreas;
using Ermes.Roles;
using Ermes.Permissions;
using Ermes.Missions;
using System.Linq;
using Ermes.Categories;
using System;
using Ermes.Reports;
using Ermes.ReportRequests;
using Ermes.Communications;
using Ermes.Notifications;
using Ermes.Preferences;
using Ermes.Teams;
using Abp.Authorization;
using Ermes.Exceptions;
using System.Threading.Tasks;
using System.Threading;
using Abp.UI;
using Abp.Localization;
using Ermes.Localization;
using Abp.Events.Bus.Entities;
using Ermes.EntityHistory;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Ermes.EntityFrameworkCore
{
    public class ErmesDbContext : AbpDbContext
    {
        //Add DbSet properties for your entities...
        public virtual DbSet<Organization> Organizations { get; set; }
        public virtual DbSet<Person> Persons { get; set; }
        public virtual DbSet<Activity> Activities { get; set; }
        public virtual DbSet<ActivityTranslation> ActivityTranslations { get; set; }
        public virtual DbSet<CompetenceArea> CompetenceAreas { get; set; }
        public virtual DbSet<OrganizationCompetenceArea> OrganizationCompetenceAreas { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<PersonRole> PersonRoles { get; set; }
        public virtual DbSet<ErmesPermission> Permissions { get; set; }
        public virtual DbSet<PersonAction> PersonActions { get; set; }
        public virtual DbSet<Mission> Missions { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CategoryTranslation> CategoryTranslations { get; set; }
        public virtual DbSet<Report> Reports { get; set; }
        public virtual DbSet<ReportRequest> ReportRequests { get; set; }
        public virtual DbSet<Communication> Communications { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Preference> Preferences { get; set; }
        public virtual DbSet<Team> Teams { get; set; }
        public virtual DbSet<SplitEntityChange> EntityChange { get; set; }
        public virtual DbSet<SplitEntityChangeSet> EntityChangeSet { get; set; }
        public virtual DbSet<SplitEntityPropertyChange> EntityPropertyChange { get; set; }
        public IEntityHistoryHelper EntityHistoryHelper { get; set; }

        private readonly ErmesLocalizationHelper _localizer;
        public ErmesDbContext(DbContextOptions<ErmesDbContext> options, ErmesLocalizationHelper localizer)
            : base(options)
        {
            _localizer = localizer;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PersonActionActivity>();
            modelBuilder.Entity<PersonActionTracking>();
            modelBuilder.Entity<PersonActionStatus>();
            modelBuilder.Entity<Preference>().HasKey(p => new { p.PreferenceOwnerId, p.SourceString });
            modelBuilder.Entity<Preference>().HasOne<Person>(p => p.PreferenceOwner).WithOne().HasForeignKey<Preference>(p => p.PreferenceOwnerId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Preference>().HasIndex(p => p.PreferenceOwnerId).IsUnique(false);
            modelBuilder.Entity<Preference>().Ignore(p => p.Id);
            modelBuilder.Entity<CategoryTranslation>().HasIndex(i => new { i.Group, i.Name, i.Language }).IsUnique();
            modelBuilder.Entity<Team>().HasIndex(t => new { t.Name, t.OrganizationId }).IsUnique(true);
            modelBuilder.Entity<Organization>().HasIndex(t => t.Name).IsUnique(true);
            modelBuilder.Entity<Organization>().HasIndex(t => t.ShortName).IsUnique(true);
            modelBuilder.Entity<PersonRole>().HasIndex(pr => new { pr.PersonId, pr.RoleId }).IsUnique(true);
            modelBuilder.Entity<ErmesPermission>().HasIndex(ep => new { ep.Name, ep.RoleId }).IsUnique(true);
            modelBuilder.Entity<CompetenceArea>().HasIndex(ca => ca.Uuid).IsUnique(true);
            modelBuilder.Entity<ActivityTranslation>().HasIndex(at => new { at.Language, at.Name }).IsUnique(true);
            modelBuilder.Entity<ActivityTranslation>().HasIndex(at => new { at.Language, at.CoreId, }).IsUnique(true);
            modelBuilder.Entity<Role>().HasIndex(r => r.Name).IsUnique(true);
            modelBuilder.Entity<Activity>().HasIndex(a => a.ShortName).IsUnique(true);
            modelBuilder.Entity<OrganizationCompetenceArea>().HasIndex(oca => new { oca.OrganizationId, oca.CompetenceAreaId }).IsUnique(true);

            #region EntityHistory
            modelBuilder.Entity<SplitEntityChange>().HasMany(e => e.PropertyChanges).WithOne().HasForeignKey(e => e.EntityChangeId);
            modelBuilder.Entity<SplitEntityChange>().HasIndex(e => e.EntityChangeSetId);
            modelBuilder.Entity<SplitEntityChange>().HasIndex(e => e.EntityTypeName);
            modelBuilder.Entity<SplitEntityChange>().HasIndex(e => e.EntityId);
            modelBuilder.Entity<SplitEntityChangeSet>().HasMany(e => e.EntityChanges).WithOne().HasForeignKey(e => e.EntityChangeSetId);
            modelBuilder.Entity<SplitEntityPropertyChange>().HasIndex(e => e.EntityChangeId);
            #endregion

            //modelBuilder.SeedData();
        }

        public override int SaveChanges()
        {
            try
            {
                //return base.SaveChanges();
                var changeSet = EntityHistoryHelper?.CreateEntityChangeSet(ChangeTracker.Entries().ToList());

                var result = base.SaveChanges();

                EntityHistoryHelper?.Save(changeSet);

                return result;
            }
            catch (Exception e)
            {
                if (_localizer != null && e is DbUpdateException)
                {
                    CoreExceptions.FriendlySQLExceptions(e, out string userFriendlyMessageCode, out Object[] userFriendlyParams);
                    string userFriendlyMessage = _localizer.L(userFriendlyMessageCode, userFriendlyParams);
                    throw new UserFriendlyException(userFriendlyMessage);
                }
                else
                    throw e;
            }
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                //return await base.SaveChangesAsync();
                var changeSet = EntityHistoryHelper?.CreateEntityChangeSet(ChangeTracker.Entries().ToList());
                var result = await base.SaveChangesAsync(cancellationToken);
                if (EntityHistoryHelper != null)
                {
                    await EntityHistoryHelper.SaveAsync(changeSet);
                }

                return result;
            }
            catch (Exception e)
            {
                if (_localizer != null && e is DbUpdateException)
                {
                    CoreExceptions.FriendlySQLExceptions(e, out string userFriendlyMessageCode, out Object[] userFriendlyParams);
                    string userFriendlyMessage = _localizer.L(userFriendlyMessageCode, userFriendlyParams);
                    throw new UserFriendlyException(userFriendlyMessage);
                }
                else
                    throw e;
            }
        }


    }
}
