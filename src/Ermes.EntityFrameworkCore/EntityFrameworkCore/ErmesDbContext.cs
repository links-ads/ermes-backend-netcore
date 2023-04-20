using Abp.EntityFrameworkCore;
using Abp.UI;
using Ermes.Activities;
using Ermes.Answers;
using Ermes.Categories;
using Ermes.Communications;
using Ermes.CompetenceAreas;
using Ermes.EntityHistory;
using Ermes.Exceptions;
using Ermes.Gamification;
using Ermes.Layers;
using Ermes.Localization;
using Ermes.MapRequests;
using Ermes.Missions;
using Ermes.Notifications;
using Ermes.Operations;
using Ermes.Organizations;
using Ermes.Permissions;
using Ermes.Persons;
using Ermes.Preferences;
using Ermes.Quizzes;
using Ermes.ReportRequests;
using Ermes.Reports;
using Ermes.Roles;
using Ermes.Teams;
using Ermes.Tips;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
        public virtual DbSet<ReportValidation> ReportValidations { get; set; }
        public virtual DbSet<ReportRequest> ReportRequests { get; set; }
        public virtual DbSet<Communication> Communications { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Preference> Preferences { get; set; }
        public virtual DbSet<Team> Teams { get; set; }
        public virtual DbSet<SplitEntityChange> EntityChange { get; set; }
        public virtual DbSet<SplitEntityChangeSet> EntityChangeSet { get; set; }
        public virtual DbSet<SplitEntityPropertyChange> EntityPropertyChange { get; set; }
        public virtual DbSet<Tip> Tips { get; set; }
        public virtual DbSet<PersonTip> PersonTips { get; set; }
        public virtual DbSet<TipTranslation> TipTranslations { get; set; }
        public virtual DbSet<Quiz> Quizzes { get; set; }
        public virtual DbSet<PersonQuiz> PersonQuizzes { get; set; }
        public virtual DbSet<QuizTranslation> QuizTranslations { get; set; }        
        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<AnswerTranslation> AnswerTranslations { get; set; }
        public virtual DbSet<MapRequest> MapRequests { get; set; }
        public virtual DbSet<Layer> Layers { get; set; }
        public virtual DbSet<LayerTranslation> LayerTranslations { get; set; }
        public virtual DbSet<Operation> Operations { get; set; }
        public virtual DbSet<Level> Levels { get; set; }
        public virtual DbSet<GamificationAction> GamificationActions { get; set; }
        public virtual DbSet<Reward> Rewards { get; set; }
        public virtual DbSet<Achievement> Achievements { get; set; }
        public virtual DbSet<Medal> Medals { get; set; }
        public virtual DbSet<Badge> Badges { get; set; }
        public virtual DbSet<Award> Awards { get; set; }
        public virtual DbSet<GamificationAudit> GamificationAudit { get; set; }
        public virtual DbSet<Gamification.Barrier> Barriers { get; set; }
        public virtual DbSet<MapRequestLayer> MapRequestLayers { get; set; }
        public virtual DbSet<CommunicationReceiver> CommunicationReceivers { get; set; }

        public IEntityHistoryHelper EntityHistoryHelper { get; set; }

        private readonly ErmesLocalizationHelper _localizer;

        public ErmesDbContext(DbContextOptions<ErmesDbContext> options) : base(options)
        {

        }
        public ErmesDbContext(DbContextOptions<ErmesDbContext> options, ErmesLocalizationHelper localizer)
            : base(options)
        {
            _localizer = localizer;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //It's necessary to create a specific value-comparer for jsonb properties
            //This allow the update of this kind of field
            //see https://stackoverflow.com/questions/62021228/entity-framework-not-detecting-jsonb-properties-changes-in-c-sharp
            //and https://docs.microsoft.com/en-us/ef/core/modeling/value-comparers?tabs=ef5
            modelBuilder
                .Entity<Report>()
                .Property(r => r.ExtensionData)
                .Metadata
                .SetValueComparer(
                new ValueComparer<List<ReportExtensionData>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList())
            );
            modelBuilder
                .Entity<MapRequestLayer>()
                .Property(r => r.ErrorMessages)
                .Metadata
                .SetValueComparer(
                new ValueComparer<List<MapRequestLayerError>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList())
            );
            modelBuilder.Entity<PersonActionActivity>();
            modelBuilder.Entity<PersonActionTracking>();
            modelBuilder.Entity<PersonActionStatus>();
            modelBuilder.Entity<PersonActionSharingPosition>();
            modelBuilder.Entity<Preference>().HasKey(p => new { p.PreferenceOwnerId, p.SourceString });
            modelBuilder.Entity<Preference>().HasOne<Person>(p => p.PreferenceOwner).WithOne().HasForeignKey<Preference>(p => p.PreferenceOwnerId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Preference>().HasIndex(p => p.PreferenceOwnerId).IsUnique(false);
            modelBuilder.Entity<Preference>().Ignore(p => p.Id);
            modelBuilder.Entity<CategoryTranslation>().HasIndex(i => new { i.Group, i.SubGroup, i.Name, i.Language }).IsUnique();
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
            modelBuilder.Entity<Tip>().HasIndex(i => i.Code).IsUnique();
            modelBuilder.Entity<Quiz>().HasIndex(i => i.Code).IsUnique();
            modelBuilder.Entity<Answer>().HasIndex(i => i.Code).IsUnique();
            modelBuilder.Entity<QuizTranslation>().HasIndex(i => new { i.CoreId, i.Language }).IsUnique();
            modelBuilder.Entity<TipTranslation>().HasIndex(i => new { i.CoreId, i.Language }).IsUnique();
            modelBuilder.Entity<AnswerTranslation>().HasIndex(i => new { i.CoreId, i.Language }).IsUnique();
            modelBuilder.Entity<Quiz>()
                .HasOne<Tip>(q => q.Tip)
                .WithMany(t => t.Quizzes)
                .HasPrincipalKey(t => t.Code)
                .HasForeignKey(t => t.TipCode)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Answer>()
                .HasOne<Quiz>(a => a.Quiz)
                .WithMany(t => t.Answers)
                .HasPrincipalKey(q => q.Code)
                .HasForeignKey(a => a.QuizCode)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MapRequest>().HasIndex(t => t.Code).IsUnique(true);

            modelBuilder.Entity<Layer>().HasIndex(i => i.DataTypeId).IsUnique();
            modelBuilder.Entity<Layer>()
                .HasOne<Layer>(l => l.Parent)
                .WithMany(l => l.AssociatedLayers)
                .HasPrincipalKey(l => l.DataTypeId)
                .HasForeignKey(l => l.ParentDataTypeId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LayerTranslation>().HasIndex(i => new { i.CoreId, i.Language }).IsUnique();

            modelBuilder.Entity<PersonTip>()
                .HasIndex(pt => new { pt.PersonId, pt.TipCode })
                .IsUnique();

            modelBuilder.Entity<PersonTip>()
                .HasOne<Person>(pt => pt.Person)
                .WithMany(p => p.Tips)
                .HasPrincipalKey(p => p.Id)
                .HasForeignKey(pt => pt.PersonId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PersonTip>()
                .HasOne<Tip>(pt => pt.Tip)
                .WithMany(p => p.Readers)
                .HasPrincipalKey(t => t.Code)
                .HasForeignKey(pt => pt.TipCode)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PersonQuiz>()
                .HasIndex(pt => new { pt.PersonId, pt.QuizCode })
                .IsUnique();

            modelBuilder.Entity<PersonQuiz>()
                .HasOne<Person>(pt => pt.Person)
                .WithMany(p => p.Quizzes)
                .HasPrincipalKey(p => p.Id)
                .HasForeignKey(pt => pt.PersonId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PersonQuiz>()
                .HasOne<Quiz>(pt => pt.Quiz)
                .WithMany(p => p.Solvers)
                .HasPrincipalKey(t => t.Code)
                .HasForeignKey(pt => pt.QuizCode)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GamificationAction>().HasIndex(i => i.Code).IsUnique();

            modelBuilder.Entity<Achievement>()
                .HasOne<GamificationAction>(a => a.GamificationAction)
                .WithMany(a => a.Achievements)
                .HasPrincipalKey(a => a.Code)
                .HasForeignKey(a => a.GamificationActionCode)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Gamification.Barrier>()
                .HasIndex(b => new { b.LevelName, b.RewardName }).IsUnique(true);

            modelBuilder.Entity<Gamification.Barrier>()
                .HasOne<Level>(b => b.Level)
                .WithMany(l => l.Barriers)
                .HasPrincipalKey(l => l.Name)
                .HasForeignKey(b => b.LevelName)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Gamification.Barrier>()
                .HasOne<Reward>(b => b.Reward)
                .WithOne(a => a.Barrier)
                .HasPrincipalKey<Reward>(r => r.Name)
                .HasForeignKey<Gamification.Barrier>(b => b.RewardName)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MapRequestLayer>()
                .HasIndex(mrl => new { mrl.MapRequestCode, mrl.LayerDataTypeId })
                .IsUnique(true);
            modelBuilder.Entity<MapRequestLayer>()
                .HasOne(mrl => mrl.MapRequest)
                .WithMany(mr => mr.MapRequestLayers)
                .HasPrincipalKey(mr => mr.Code)
                .HasForeignKey(mrl => mrl.MapRequestCode)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<MapRequestLayer>()
                .HasOne(mrl => mrl.Layer)
                .WithMany(l => l.MapRequestLayers)
                .HasPrincipalKey(l => l.DataTypeId)
                .HasForeignKey(mrl => mrl.LayerDataTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CommunicationReceiver>().HasIndex(i => new { i.CommunicationId, i.OrganizationId }).IsUnique();
            modelBuilder.Entity<CommunicationReceiver>()
                .HasOne<Communication>(cr => cr.Communication)
                .WithMany(c => c.CommunicationReceivers)
                .HasPrincipalKey(c => c.Id)
                .HasForeignKey(cr => cr.CommunicationId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReportValidation>().HasIndex(i => new { i.PersonId, i.ReportId }).IsUnique();
            modelBuilder.Entity<ReportValidation>()
                .HasOne<Person>(rv => rv.Person)
                .WithMany(p => p.ReportValidations)
                .HasPrincipalKey(p => p.Id)
                .HasForeignKey(rv => rv.PersonId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ReportValidation>()
                .HasOne<Report>(rv => rv.Report)
                .WithMany(r => r.Validations)
                .HasPrincipalKey(r => r.Id)
                .HasForeignKey(rv => rv.ReportId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

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
