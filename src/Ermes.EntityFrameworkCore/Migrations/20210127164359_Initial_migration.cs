using System;
using System.Collections.Generic;
using Ermes.Reports;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using NpgsqlTypes;

namespace Ermes.Migrations
{
    public partial class Initial_migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "activities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShortName = table.Column<string>(maxLength: 8, nullable: true),
                    ParentId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_activities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_activities_activities_ParentId",
                        column: x => x.ParentId,
                        principalTable: "activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<string>(nullable: false),
                    Hazard = table.Column<string>(nullable: false),
                    Code = table.Column<string>(nullable: false),
                    GroupCode = table.Column<string>(nullable: false),
                    UnitOfMeasure = table.Column<string>(nullable: true),
                    MinValue = table.Column<int>(nullable: false),
                    MaxValue = table.Column<int>(nullable: false),
                    StatusValues = table.Column<string[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "competence_areas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Uuid = table.Column<string>(maxLength: 255, nullable: true),
                    Name = table.Column<string>(nullable: false),
                    CompetenceAreaType = table.Column<string>(nullable: false),
                    AreaOfInterest = table.Column<Geometry>(type: "geography", nullable: false),
                    Source = table.Column<string>(nullable: true),
                    Metadata = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_competence_areas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "entitychangesets",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BrowserInfo = table.Column<string>(maxLength: 512, nullable: true),
                    ClientIpAddress = table.Column<string>(maxLength: 64, nullable: true),
                    ClientName = table.Column<string>(maxLength: 128, nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    ExtensionData = table.Column<string>(nullable: true),
                    ImpersonatorTenantId = table.Column<int>(nullable: true),
                    ImpersonatorUserId = table.Column<long>(nullable: true),
                    Reason = table.Column<string>(maxLength: 256, nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    UserId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entitychangesets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "organizations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    ShortName = table.Column<string>(maxLength: 30, nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Description = table.Column<string>(maxLength: 1024, nullable: true),
                    WebSite = table.Column<string>(maxLength: 255, nullable: true),
                    LogoUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Name = table.Column<string>(maxLength: 50, nullable: true),
                    Description = table.Column<string>(maxLength: 255, nullable: true),
                    Default = table.Column<bool>(nullable: false),
                    SuperRole = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "activity_translations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(maxLength: 255, nullable: true),
                    CoreId = table.Column<int>(nullable: false),
                    Language = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_activity_translations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_activity_translations_activities_CoreId",
                        column: x => x.CoreId,
                        principalTable: "activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "category_translations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Group = table.Column<string>(maxLength: 100, nullable: false),
                    SubGroup = table.Column<string>(maxLength: 500, nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    Values = table.Column<string[]>(nullable: true),
                    CoreId = table.Column<int>(nullable: false),
                    Language = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_category_translations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_category_translations_categories_CoreId",
                        column: x => x.CoreId,
                        principalTable: "categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "entitychanges",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChangeTime = table.Column<DateTime>(nullable: false),
                    ChangeType = table.Column<byte>(nullable: false),
                    EntityChangeSetId = table.Column<long>(nullable: false),
                    EntityId = table.Column<string>(maxLength: 48, nullable: true),
                    EntityTypeName = table.Column<string>(maxLength: 96, nullable: true),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entitychanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_entitychanges_entitychangesets_EntityChangeSetId",
                        column: x => x.EntityChangeSetId,
                        principalTable: "entitychangesets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "organization_competence_areas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrganizationId = table.Column<int>(nullable: false),
                    CompetenceAreaId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organization_competence_areas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_organization_competence_areas_competence_areas_CompetenceAr~",
                        column: x => x.CompetenceAreaId,
                        principalTable: "competence_areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_organization_competence_areas_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "teams",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    OrganizationId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_teams_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    RoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_permissions_roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "entitypropertychanges",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EntityChangeId = table.Column<long>(nullable: false),
                    NewValue = table.Column<string>(maxLength: 512, nullable: true),
                    OriginalValue = table.Column<string>(maxLength: 512, nullable: true),
                    PropertyName = table.Column<string>(maxLength: 96, nullable: true),
                    PropertyTypeFullName = table.Column<string>(maxLength: 192, nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    SplitIndex = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entitypropertychanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_entitypropertychanges_entitychanges_EntityChangeId",
                        column: x => x.EntityChangeId,
                        principalTable: "entitychanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "persons",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    FusionAuthUserGuid = table.Column<Guid>(nullable: false),
                    OrganizationId = table.Column<int>(nullable: true),
                    TeamId = table.Column<int>(nullable: true),
                    Username = table.Column<string>(maxLength: 255, nullable: true),
                    RegistrationToken = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_persons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_persons_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_persons_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "communications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Message = table.Column<string>(maxLength: 1000, nullable: false),
                    Duration = table.Column<NpgsqlRange<DateTime>>(nullable: false),
                    AreaOfInterest = table.Column<Geometry>(type: "geography", nullable: false),
                    Visibility = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_communications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_communications_persons_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "missions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Title = table.Column<string>(maxLength: 255, nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    Duration = table.Column<NpgsqlRange<DateTime>>(nullable: false),
                    AreaOfInterest = table.Column<Geometry>(type: "geography", nullable: false),
                    CurrentStatus = table.Column<string>(nullable: false),
                    CoordinatorPersonId = table.Column<long>(nullable: true),
                    CoordinatorTeamId = table.Column<int>(nullable: true),
                    Notes = table.Column<string>(maxLength: 1000, nullable: true),
                    Visibility = table.Column<int>(nullable: false),
                    OrganizationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_missions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_missions_persons_CoordinatorPersonId",
                        column: x => x.CoordinatorPersonId,
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_missions_teams_CoordinatorTeamId",
                        column: x => x.CoordinatorTeamId,
                        principalTable: "teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_missions_persons_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_missions_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: true),
                    ReceiverId = table.Column<long>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Channel = table.Column<string>(nullable: true),
                    EntityId = table.Column<int>(nullable: false),
                    Entity = table.Column<string>(nullable: true),
                    Title = table.Column<string>(maxLength: 100, nullable: true),
                    Message = table.Column<string>(maxLength: 1024, nullable: true),
                    CreatorId = table.Column<long>(nullable: true),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    FailureMessage = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_notifications_persons_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_notifications_persons_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "person_actions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    Location = table.Column<Point>(nullable: true),
                    CurrentStatus = table.Column<string>(nullable: false),
                    DeviceId = table.Column<string>(maxLength: 100, nullable: true),
                    DeviceName = table.Column<string>(maxLength: 255, nullable: true),
                    PersonId = table.Column<long>(nullable: false),
                    Visibility = table.Column<int>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    ActivityId = table.Column<int>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    ExtensionData = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_person_actions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_person_actions_persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_person_actions_activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "person_roles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    PersonId = table.Column<long>(nullable: false),
                    RoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_person_roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_person_roles_persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_person_roles_roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "preferences",
                columns: table => new
                {
                    PreferenceOwnerId = table.Column<long>(nullable: false),
                    Source = table.Column<string>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Details = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_preferences", x => new { x.PreferenceOwnerId, x.Source });
                    table.ForeignKey(
                        name: "FK_preferences_persons_PreferenceOwnerId",
                        column: x => x.PreferenceOwnerId,
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "reportrequests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Title = table.Column<string>(maxLength: 255, nullable: false),
                    AreaOfInterest = table.Column<Geometry>(type: "geography", nullable: false),
                    Duration = table.Column<NpgsqlRange<DateTime>>(nullable: false),
                    SelectedCategories = table.Column<List<int>>(nullable: false),
                    Visibility = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reportrequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_reportrequests_persons_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "reports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Hazard = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Location = table.Column<Point>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    Address = table.Column<string>(nullable: true),
                    MediaURIs = table.Column<List<string>>(nullable: true),
                    ExtensionData = table.Column<List<ReportExtensionData>>(type: "jsonb", nullable: true),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(maxLength: 1000, nullable: true),
                    Targets = table.Column<List<ReportTarget>>(type: "jsonb", nullable: true),
                    Visibility = table.Column<int>(nullable: false),
                    Source = table.Column<string>(nullable: true),
                    RelativeMissionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_reports_persons_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_reports_missions_RelativeMissionId",
                        column: x => x.RelativeMissionId,
                        principalTable: "missions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_activities_ParentId",
                table: "activities",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_activities_ShortName",
                table: "activities",
                column: "ShortName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_activity_translations_CoreId",
                table: "activity_translations",
                column: "CoreId");

            migrationBuilder.CreateIndex(
                name: "IX_activity_translations_Language_CoreId",
                table: "activity_translations",
                columns: new[] { "Language", "CoreId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_activity_translations_Language_Name",
                table: "activity_translations",
                columns: new[] { "Language", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_category_translations_CoreId",
                table: "category_translations",
                column: "CoreId");

            migrationBuilder.CreateIndex(
                name: "IX_category_translations_Group_Name_Language",
                table: "category_translations",
                columns: new[] { "Group", "Name", "Language" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_communications_CreatorUserId",
                table: "communications",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_competence_areas_Uuid",
                table: "competence_areas",
                column: "Uuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_entitychanges_EntityChangeSetId",
                table: "entitychanges",
                column: "EntityChangeSetId");

            migrationBuilder.CreateIndex(
                name: "IX_entitychanges_EntityId",
                table: "entitychanges",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_entitychanges_EntityTypeName",
                table: "entitychanges",
                column: "EntityTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_entitypropertychanges_EntityChangeId",
                table: "entitypropertychanges",
                column: "EntityChangeId");

            migrationBuilder.CreateIndex(
                name: "IX_missions_CoordinatorPersonId",
                table: "missions",
                column: "CoordinatorPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_missions_CoordinatorTeamId",
                table: "missions",
                column: "CoordinatorTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_missions_CreatorUserId",
                table: "missions",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_missions_OrganizationId",
                table: "missions",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_CreatorId",
                table: "notifications",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_ReceiverId",
                table: "notifications",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_organization_competence_areas_CompetenceAreaId",
                table: "organization_competence_areas",
                column: "CompetenceAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_organization_competence_areas_OrganizationId_CompetenceArea~",
                table: "organization_competence_areas",
                columns: new[] { "OrganizationId", "CompetenceAreaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_organizations_Name",
                table: "organizations",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_organizations_ShortName",
                table: "organizations",
                column: "ShortName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_permissions_RoleId",
                table: "permissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_permissions_Name_RoleId",
                table: "permissions",
                columns: new[] { "Name", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_person_actions_PersonId",
                table: "person_actions",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_person_actions_ActivityId",
                table: "person_actions",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_person_roles_RoleId",
                table: "person_roles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_person_roles_PersonId_RoleId",
                table: "person_roles",
                columns: new[] { "PersonId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_persons_OrganizationId",
                table: "persons",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_persons_TeamId",
                table: "persons",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_preferences_PreferenceOwnerId",
                table: "preferences",
                column: "PreferenceOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_reportrequests_CreatorUserId",
                table: "reportrequests",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_reports_CreatorUserId",
                table: "reports",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_reports_RelativeMissionId",
                table: "reports",
                column: "RelativeMissionId");

            migrationBuilder.CreateIndex(
                name: "IX_roles_Name",
                table: "roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_teams_OrganizationId",
                table: "teams",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_teams_Name_OrganizationId",
                table: "teams",
                columns: new[] { "Name", "OrganizationId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "activity_translations");

            migrationBuilder.DropTable(
                name: "category_translations");

            migrationBuilder.DropTable(
                name: "communications");

            migrationBuilder.DropTable(
                name: "entitypropertychanges");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "organization_competence_areas");

            migrationBuilder.DropTable(
                name: "permissions");

            migrationBuilder.DropTable(
                name: "person_actions");

            migrationBuilder.DropTable(
                name: "person_roles");

            migrationBuilder.DropTable(
                name: "preferences");

            migrationBuilder.DropTable(
                name: "reportrequests");

            migrationBuilder.DropTable(
                name: "reports");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "entitychanges");

            migrationBuilder.DropTable(
                name: "competence_areas");

            migrationBuilder.DropTable(
                name: "activities");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "missions");

            migrationBuilder.DropTable(
                name: "entitychangesets");

            migrationBuilder.DropTable(
                name: "persons");

            migrationBuilder.DropTable(
                name: "teams");

            migrationBuilder.DropTable(
                name: "organizations");
        }
    }
}
