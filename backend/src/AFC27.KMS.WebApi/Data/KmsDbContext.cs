using Microsoft.EntityFrameworkCore;
using AFC27.KMS.SharedKernel.Domain;
using AFC27.KMS.SharedKernel.Interfaces;
using AFC27.KMS.Identity.Domain.Entities;
using AFC27.KMS.Content.Domain.Entities;
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
