using Microsoft.EntityFrameworkCore;
using AFC27.KMS.SharedKernel.Domain;
using AFC27.KMS.SharedKernel.Interfaces;
using AFC27.KMS.Identity.Domain.Entities;
using AFC27.KMS.Content.Domain.Entities;
using AFC27.KMS.Collaboration.Domain.Entities;
using AFC27.KMS.Workflow.Domain.Entities;
using System.Linq.Expressions;

namespace AFC27.KMS.WebApi.Data;

/// <summary>
/// KMS Database Context with all module entities configured.
/// </summary>
public class KmsDbContext : DbContext, IUnitOfWork
{
    private readonly ICurrentUser? _currentUser;

    // Identity Module
    public DbSet<User> Users => Set<User>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Group> Groups => Set<Group>();

    // Content Module
    public DbSet<Article> Articles => Set<Article>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Space> Spaces => Set<Space>();
    public DbSet<SpaceMember> SpaceMembers => Set<SpaceMember>();
    public DbSet<SpacePermission> SpacePermissions => Set<SpacePermission>();
    public DbSet<VerificationRecord> VerificationRecords => Set<VerificationRecord>();

    // Collaboration Module - Lessons Learned
    public DbSet<LessonLearned> LessonsLearned => Set<LessonLearned>();
    public DbSet<LessonAction> LessonActions => Set<LessonAction>();
    public DbSet<LessonApplication> LessonApplications => Set<LessonApplication>();
    public DbSet<LessonTag> LessonTags => Set<LessonTag>();
    public DbSet<LessonUsefulVote> LessonUsefulVotes => Set<LessonUsefulVote>();

    // Workflow Module - Generalized Engine
    public DbSet<WorkflowDefinition> WorkflowDefinitions => Set<WorkflowDefinition>();
    public DbSet<WorkflowInstance> WorkflowInstances => Set<WorkflowInstance>();
    public DbSet<WorkflowStepInstance> WorkflowStepInstances => Set<WorkflowStepInstance>();

    public KmsDbContext(DbContextOptions<KmsDbContext> options)
        : base(options)
    {
    }

    public KmsDbContext(
        DbContextOptions<KmsDbContext> options,
        ICurrentUser currentUser)
        : base(options)
    {
        _currentUser = currentUser;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Identity Module entities
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Email).HasMaxLength(256).IsRequired();
            entity.Property(u => u.DisplayName).HasMaxLength(256).IsRequired();
            entity.Property(u => u.DisplayNameArabic).HasMaxLength(256);

            entity.HasOne(u => u.Department)
                .WithMany(d => d.Members)
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(u => u.Manager)
                .WithMany(u => u.DirectReports)
                .HasForeignKey(u => u.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Ignore(u => u.DomainEvents);
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Departments");
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Name).HasMaxLength(256).IsRequired();
            entity.Property(d => d.NameArabic).HasMaxLength(256);

            entity.HasOne(d => d.Parent)
                .WithMany(d => d.Children)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Ignore(d => d.DomainEvents);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(r => r.Id);
            entity.HasIndex(r => r.Name).IsUnique();
            entity.Property(r => r.Name).HasMaxLength(100).IsRequired();
            entity.Property(r => r.NameArabic).HasMaxLength(100);

            entity.Ignore(r => r.DomainEvents);
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.ToTable("Groups");
            entity.HasKey(g => g.Id);
            entity.Property(g => g.Name).HasMaxLength(256).IsRequired();
            entity.Property(g => g.NameArabic).HasMaxLength(256);

            entity.Ignore(g => g.DomainEvents);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("UserRoles");
            entity.HasKey(ur => new { ur.UserId, ur.RoleId });
        });

        modelBuilder.Entity<GroupMember>(entity =>
        {
            entity.ToTable("GroupMembers");
            entity.HasKey(gm => new { gm.GroupId, gm.UserId });
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTable("Permissions");
            entity.HasKey(p => p.Id);
            entity.HasIndex(p => p.Name).IsUnique();
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.ToTable("RolePermissions");
            entity.HasKey(rp => new { rp.RoleId, rp.PermissionId });
        });

        // Configure Content Module entities
        modelBuilder.Entity<Article>(entity =>
        {
            entity.ToTable("Articles");
            entity.HasKey(a => a.Id);
            entity.HasIndex(a => a.Slug).IsUnique();
            entity.Property(a => a.Slug).HasMaxLength(500).IsRequired();

            entity.OwnsOne(a => a.Title, title =>
            {
                title.Property(t => t.English).HasColumnName("TitleEn").HasMaxLength(500).IsRequired();
                title.Property(t => t.Arabic).HasColumnName("TitleAr").HasMaxLength(500);
            });

            entity.OwnsOne(a => a.Summary, summary =>
            {
                summary.Property(s => s.English).HasColumnName("SummaryEn").HasMaxLength(1000);
                summary.Property(s => s.Arabic).HasColumnName("SummaryAr").HasMaxLength(1000);
            });

            entity.OwnsOne(a => a.Content, content =>
            {
                content.Property(c => c.English).HasColumnName("ContentEn").IsRequired();
                content.Property(c => c.Arabic).HasColumnName("ContentAr");
            });

            entity.HasOne(a => a.Category)
                .WithMany(c => c.Articles)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            // Verification properties
            entity.Property(a => a.OwnerName).HasMaxLength(256);
            entity.Property(a => a.VerificationStatus).HasConversion<string>().HasMaxLength(20);
            entity.HasIndex(a => a.OwnerId);
            entity.HasIndex(a => a.VerificationStatus);
            entity.HasIndex(a => a.NextVerificationDue);

            entity.HasMany(a => a.VerificationRecords)
                .WithOne(v => v.Article)
                .HasForeignKey(v => v.ArticleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Ignore(a => a.DomainEvents);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Categories");
            entity.HasKey(c => c.Id);
            entity.HasIndex(c => c.Slug).IsUnique();

            entity.OwnsOne(c => c.Name, name =>
            {
                name.Property(n => n.English).HasColumnName("NameEn").HasMaxLength(256).IsRequired();
                name.Property(n => n.Arabic).HasColumnName("NameAr").HasMaxLength(256);
            });

            entity.OwnsOne(c => c.Description, desc =>
            {
                desc.Property(d => d.English).HasColumnName("DescriptionEn").HasMaxLength(1000);
                desc.Property(d => d.Arabic).HasColumnName("DescriptionAr").HasMaxLength(1000);
            });

            entity.HasOne(c => c.Parent)
                .WithMany(c => c.Children)
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Ignore(c => c.DomainEvents);
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.ToTable("Tags");
            entity.HasKey(t => t.Id);
            entity.HasIndex(t => t.Slug).IsUnique();

            entity.OwnsOne(t => t.Name, name =>
            {
                name.Property(n => n.English).HasColumnName("NameEn").HasMaxLength(100).IsRequired();
                name.Property(n => n.Arabic).HasColumnName("NameAr").HasMaxLength(100);
            });

            entity.Ignore(t => t.DomainEvents);
        });

        modelBuilder.Entity<ArticleTag>(entity =>
        {
            entity.ToTable("ArticleTags");
            entity.HasKey(at => new { at.ArticleId, at.TagId });
        });

        modelBuilder.Entity<ArticleVersion>(entity =>
        {
            entity.ToTable("ArticleVersions");
            entity.HasKey(av => av.Id);
        });

        modelBuilder.Entity<VerificationRecord>(entity =>
        {
            entity.ToTable("VerificationRecords");
            entity.HasKey(v => v.Id);

            entity.Property(v => v.VerifiedByName).HasMaxLength(256).IsRequired();
            entity.Property(v => v.Notes).HasMaxLength(2000);
            entity.Property(v => v.PreviousStatus).HasConversion<string>().HasMaxLength(20);
            entity.Property(v => v.NewStatus).HasConversion<string>().HasMaxLength(20);

            entity.HasIndex(v => v.ArticleId);
            entity.HasIndex(v => v.VerifiedById);
            entity.HasIndex(v => v.VerifiedAt);

            entity.Ignore(v => v.DomainEvents);
        });

        // Configure Content Module - Spaces
        modelBuilder.Entity<Space>(entity =>
        {
            entity.ToTable("Spaces");
            entity.HasKey(s => s.Id);

            entity.OwnsOne(s => s.Name, name =>
            {
                name.Property(n => n.English).HasColumnName("NameEn").HasMaxLength(256).IsRequired();
                name.Property(n => n.Arabic).HasColumnName("NameAr").HasMaxLength(256);
            });

            entity.OwnsOne(s => s.Description, desc =>
            {
                desc.Property(d => d.English).HasColumnName("DescriptionEn").HasMaxLength(1000);
                desc.Property(d => d.Arabic).HasColumnName("DescriptionAr").HasMaxLength(1000);
            });

            entity.Property(s => s.SpaceType).HasConversion<string>().HasMaxLength(30);
            entity.Property(s => s.OwnerName).HasMaxLength(256);
            entity.Property(s => s.IconName).HasMaxLength(100);
            entity.Property(s => s.Color).HasMaxLength(20);

            entity.HasOne(s => s.ParentSpace)
                .WithMany(s => s.ChildSpaces)
                .HasForeignKey(s => s.ParentSpaceId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(s => s.Members)
                .WithOne(m => m.Space)
                .HasForeignKey(m => m.SpaceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(s => s.Permissions)
                .WithOne(p => p.Space)
                .HasForeignKey(p => p.SpaceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(s => s.OwnerId);
            entity.HasIndex(s => s.SpaceType);
            entity.HasIndex(s => s.IsPublic);
            entity.HasIndex(s => s.ParentSpaceId);

            entity.Ignore(s => s.DomainEvents);
        });

        modelBuilder.Entity<SpaceMember>(entity =>
        {
            entity.ToTable("SpaceMembers");
            entity.HasKey(sm => sm.Id);
            entity.Property(sm => sm.UserName).HasMaxLength(256);
            entity.Property(sm => sm.Role).HasConversion<string>().HasMaxLength(20);

            entity.HasIndex(sm => new { sm.SpaceId, sm.UserId }).IsUnique();
            entity.HasIndex(sm => sm.UserId);
        });

        modelBuilder.Entity<SpacePermission>(entity =>
        {
            entity.ToTable("SpacePermissions");
            entity.HasKey(sp => sp.Id);
            entity.Property(sp => sp.AccessLevel).HasConversion<string>().HasMaxLength(20);

            entity.HasIndex(sp => sp.SpaceId);
            entity.HasIndex(sp => sp.UserId);
            entity.HasIndex(sp => sp.GroupId);
            entity.HasIndex(sp => sp.RoleId);
        });

        // Configure Article -> Space relationship
        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasOne(a => a.Space)
                .WithMany()
                .HasForeignKey(a => a.SpaceId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configure Collaboration Module - Lessons Learned
        modelBuilder.Entity<LessonLearned>(entity =>
        {
            entity.ToTable("LessonsLearned");
            entity.HasKey(e => e.Id);

            entity.OwnsOne(e => e.Title, title =>
            {
                title.Property(t => t.English).HasColumnName("TitleEn").HasMaxLength(500).IsRequired();
                title.Property(t => t.Arabic).HasColumnName("TitleAr").HasMaxLength(500);
            });

            entity.Property(e => e.Category).HasConversion<string>().HasMaxLength(50);
            entity.Property(e => e.Impact).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(30);
            entity.Property(e => e.ProjectPhase).HasConversion<string>().HasMaxLength(30);
            entity.Property(e => e.ImpactType).HasConversion<string>().HasMaxLength(30);
            entity.Property(e => e.RootCauseMethod).HasConversion<string>().HasMaxLength(30);

            entity.HasMany(e => e.Tags)
                .WithOne(t => t.Lesson)
                .HasForeignKey(t => t.LessonId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Actions)
                .WithOne(a => a.LessonLearned)
                .HasForeignKey(a => a.LessonLearnedId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.AuthorId);

            entity.Ignore(e => e.DomainEvents);
        });

        modelBuilder.Entity<LessonTag>(entity =>
        {
            entity.ToTable("LessonTags");
            entity.HasKey(e => new { e.LessonId, e.Tag });
        });

        modelBuilder.Entity<LessonAction>(entity =>
        {
            entity.ToTable("LessonActions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.Priority).HasConversion<string>().HasMaxLength(20);
            entity.HasIndex(e => e.LessonLearnedId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.DueDate);

            entity.Ignore(e => e.DomainEvents);
        });

        modelBuilder.Entity<LessonApplication>(entity =>
        {
            entity.ToTable("LessonApplications");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Outcome).HasConversion<string>().HasMaxLength(30);
            entity.HasIndex(e => e.LessonLearnedId);
        });

        modelBuilder.Entity<LessonUsefulVote>(entity =>
        {
            entity.ToTable("LessonUsefulVotes");
            entity.HasKey(e => new { e.LessonLearnedId, e.UserId });
            entity.HasOne(e => e.LessonLearned)
                .WithMany()
                .HasForeignKey(e => e.LessonLearnedId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Workflow Module - Generalized Engine
        modelBuilder.Entity<WorkflowDefinition>(entity =>
        {
            entity.ToTable("WorkflowDefinitions");
            entity.HasKey(e => e.Id);

            entity.OwnsOne(e => e.Name, name =>
            {
                name.Property(n => n.English).HasColumnName("NameEn").HasMaxLength(256).IsRequired();
                name.Property(n => n.Arabic).HasColumnName("NameAr").HasMaxLength(256);
            });

            entity.OwnsOne(e => e.Description, desc =>
            {
                desc.Property(d => d.English).HasColumnName("DescriptionEn").HasMaxLength(1000);
                desc.Property(d => d.Arabic).HasColumnName("DescriptionAr").HasMaxLength(1000);
            });

            entity.Property(e => e.Version).HasMaxLength(20);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.Type).HasConversion<string>().HasMaxLength(30);
            entity.Property(e => e.Scope).HasConversion<string>().HasMaxLength(40);

            entity.HasMany(e => e.Instances)
                .WithOne(i => i.WorkflowDefinition)
                .HasForeignKey(i => i.WorkflowDefinitionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Scope);

            entity.Ignore(e => e.DomainEvents);
        });

        modelBuilder.Entity<WorkflowStepDefinition>(entity =>
        {
            entity.ToTable("WorkflowStepDefinitions");
            entity.HasKey(e => e.Id);

            entity.OwnsOne(e => e.Name, name =>
            {
                name.Property(n => n.English).HasColumnName("NameEn").HasMaxLength(256).IsRequired();
                name.Property(n => n.Arabic).HasColumnName("NameAr").HasMaxLength(256);
            });

            entity.OwnsOne(e => e.Description, desc =>
            {
                desc.Property(d => d.English).HasColumnName("DescriptionEn").HasMaxLength(1000);
                desc.Property(d => d.Arabic).HasColumnName("DescriptionAr").HasMaxLength(1000);
            });

            entity.Property(e => e.Type).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.Action).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.AssignmentType).HasConversion<string>().HasMaxLength(30);

            entity.HasOne<WorkflowDefinition>()
                .WithMany(d => d.Steps)
                .HasForeignKey(e => e.WorkflowDefinitionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.WorkflowDefinitionId);

            entity.Ignore(e => e.DomainEvents);
        });

        modelBuilder.Entity<WorkflowInstance>(entity =>
        {
            entity.ToTable("WorkflowInstances");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.TargetEntityType).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.InitiatedByName).HasMaxLength(256);

            entity.HasMany(e => e.StepInstances)
                .WithOne(s => s.WorkflowInstance)
                .HasForeignKey(s => s.WorkflowInstanceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.WorkflowDefinitionId);
            entity.HasIndex(e => new { e.TargetEntityType, e.TargetEntityId });
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.InitiatedById);

            entity.Ignore(e => e.DomainEvents);
        });

        modelBuilder.Entity<WorkflowStepInstance>(entity =>
        {
            entity.ToTable("WorkflowStepInstances");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.StepName).HasMaxLength(256).IsRequired();
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.Outcome).HasMaxLength(100);
            entity.Property(e => e.Comments).HasMaxLength(2000);
            entity.Property(e => e.AssigneeName).HasMaxLength(256);

            entity.HasIndex(e => e.WorkflowInstanceId);
            entity.HasIndex(e => e.StepDefinitionId);
            entity.HasIndex(e => e.AssigneeId);
            entity.HasIndex(e => e.Status);

            entity.Ignore(e => e.DomainEvents);
        });

        // Global query filter for soft delete
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(AuditableEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(CreateSoftDeleteFilter(entityType.ClrType));
            }
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditableEntities()
    {
        var entries = ChangeTracker.Entries<AuditableEntity>();
        var userId = _currentUser?.UserId ?? Guid.Empty;

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.SetCreatedBy(userId);
                    break;
                case EntityState.Modified:
                    entry.Entity.SetModifiedBy(userId);
                    break;
            }
        }
    }

    private static LambdaExpression CreateSoftDeleteFilter(Type entityType)
    {
        var parameter = Expression.Parameter(entityType, "e");
        var property = Expression.Property(parameter, nameof(AuditableEntity.IsDeleted));
        var condition = Expression.Equal(property, Expression.Constant(false));
        return Expression.Lambda(condition, parameter);
    }
}
