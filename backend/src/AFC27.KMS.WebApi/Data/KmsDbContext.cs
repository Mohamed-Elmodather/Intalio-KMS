using Microsoft.EntityFrameworkCore;
using AFC27.KMS.SharedKernel.Domain;
using AFC27.KMS.SharedKernel.Interfaces;
using AFC27.KMS.Identity.Domain.Entities;
using AFC27.KMS.Content.Domain.Entities;
using AFC27.KMS.Documents.Domain.Entities;
using AFC27.KMS.Admin.Domain.Entities;
using AFC27.KMS.AI.Domain.Entities;
using AFC27.KMS.WebApi.Data.Entities;
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
    public DbSet<ContentBlock> ContentBlocks => Set<ContentBlock>();
    public DbSet<CollaborationSession> CollaborationSessions => Set<CollaborationSession>();
    public DbSet<CollaborationParticipant> CollaborationParticipants => Set<CollaborationParticipant>();

    // Documents Module
    public DbSet<DocumentLibrary> DocumentLibraries => Set<DocumentLibrary>();
    public DbSet<Folder> Folders => Set<Folder>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<DocumentVersion> DocumentVersions => Set<DocumentVersion>();
    public DbSet<DocumentAudit> DocumentAudits => Set<DocumentAudit>();
    public DbSet<LibraryPermission> LibraryPermissions => Set<LibraryPermission>();
    public DbSet<FolderPermission> FolderPermissions => Set<FolderPermission>();
    public DbSet<DocumentPermission> DocumentPermissions => Set<DocumentPermission>();

    // Admin Module
    public DbSet<AuditLogEntry> AuditLogs => Set<AuditLogEntry>();
    public DbSet<LegalHold> LegalHolds => Set<LegalHold>();
    public DbSet<LegalHoldDocument> LegalHoldDocuments => Set<LegalHoldDocument>();
    public DbSet<LegalHoldCustodian> LegalHoldCustodians => Set<LegalHoldCustodian>();
    public DbSet<QuarantinedDocument> QuarantinedDocuments => Set<QuarantinedDocument>();
    public DbSet<ImpersonationSession> ImpersonationSessions => Set<ImpersonationSession>();

    // AI Module
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<DocumentIndex> DocumentIndexes => Set<DocumentIndex>();
    public DbSet<AIUsageLog> AIUsageLogs => Set<AIUsageLog>();

    // Meeting Management Module
    public DbSet<ExternalMeetingLinkEntity> ExternalMeetingLinks => Set<ExternalMeetingLinkEntity>();
    public DbSet<MeetingDocumentLinkEntity> MeetingDocumentLinks => Set<MeetingDocumentLinkEntity>();
    public DbSet<MeetingAttendeeEntity> MeetingAttendees => Set<MeetingAttendeeEntity>();
    public DbSet<MeetingAgendaItemEntity> MeetingAgendaItems => Set<MeetingAgendaItemEntity>();
    public DbSet<MeetingActionItemEntity> MeetingActionItems => Set<MeetingActionItemEntity>();

    // AI Analysis Module
    public DbSet<DocumentAnalysisEntity> DocumentAnalyses => Set<DocumentAnalysisEntity>();
    public DbSet<ExtractedEntityRecord> ExtractedEntities => Set<ExtractedEntityRecord>();
    public DbSet<BatchAnalysisJobEntity> BatchAnalysisJobs => Set<BatchAnalysisJobEntity>();
    public DbSet<TranscriptionResultEntity> TranscriptionResults => Set<TranscriptionResultEntity>();
    public DbSet<ContentRecommendationEntity> ContentRecommendations => Set<ContentRecommendationEntity>();

    // Security/Encryption Module
    public DbSet<EncryptionKeyEntity> EncryptionKeys => Set<EncryptionKeyEntity>();
    public DbSet<EncryptionAuditLogEntity> EncryptionAuditLogs => Set<EncryptionAuditLogEntity>();
    public DbSet<EncryptedFieldReferenceEntity> EncryptedFieldReferences => Set<EncryptedFieldReferenceEntity>();

    // Service Catalog Module
    public DbSet<ServiceCategoryEntity> ServiceCategories => Set<ServiceCategoryEntity>();
    public DbSet<CatalogServiceEntity> CatalogServices => Set<CatalogServiceEntity>();
    public DbSet<ServiceRequestEntity> ServiceRequests => Set<ServiceRequestEntity>();
    public DbSet<RequestApprovalEntity> RequestApprovals => Set<RequestApprovalEntity>();
    public DbSet<RequestCommentEntity> RequestComments => Set<RequestCommentEntity>();
    public DbSet<RequestAttachmentEntity> RequestAttachments => Set<RequestAttachmentEntity>();
    public DbSet<RequestStatusHistoryEntity> RequestStatusHistory => Set<RequestStatusHistoryEntity>();

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

        // Configure ContentBlock entity
        modelBuilder.Entity<ContentBlock>(entity =>
        {
            entity.ToTable("ContentBlocks");
            entity.HasKey(b => b.Id);

            entity.Property(b => b.Content).IsRequired();
            entity.Property(b => b.ContentArabic);
            entity.Property(b => b.Metadata).HasMaxLength(4000);

            entity.HasOne(b => b.Article)
                .WithMany()
                .HasForeignKey(b => b.ArticleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(b => b.ParentBlock)
                .WithMany(b => b.ChildBlocks)
                .HasForeignKey(b => b.ParentBlockId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(b => new { b.ArticleId, b.Order });

            entity.Ignore(b => b.DomainEvents);
        });

        // Configure CollaborationSession entity
        modelBuilder.Entity<CollaborationSession>(entity =>
        {
            entity.ToTable("CollaborationSessions");
            entity.HasKey(s => s.Id);

            entity.Property(s => s.SessionId).HasMaxLength(50).IsRequired();
            entity.Property(s => s.CrdtState);

            entity.HasOne(s => s.Article)
                .WithMany()
                .HasForeignKey(s => s.ArticleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(s => new { s.ArticleId, s.IsActive });
            entity.HasIndex(s => s.SessionId);

            entity.Ignore(s => s.DomainEvents);
        });

        // Configure CollaborationParticipant entity
        modelBuilder.Entity<CollaborationParticipant>(entity =>
        {
            entity.ToTable("CollaborationParticipants");
            entity.HasKey(p => p.Id);

            entity.Property(p => p.UserName).HasMaxLength(256).IsRequired();
            entity.Property(p => p.AvatarUrl).HasMaxLength(500);
            entity.Property(p => p.Color).HasMaxLength(50).IsRequired();
            entity.Property(p => p.ConnectionId).HasMaxLength(100).IsRequired();

            entity.HasOne(p => p.Session)
                .WithMany(s => s.Participants)
                .HasForeignKey(p => p.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(p => new { p.SessionId, p.UserId });
            entity.HasIndex(p => p.ConnectionId);

            entity.Ignore(p => p.DomainEvents);
        });

        // Configure Documents Module entities
        modelBuilder.Entity<DocumentLibrary>(entity =>
        {
            entity.ToTable("DocumentLibraries");
            entity.HasKey(l => l.Id);

            entity.OwnsOne(l => l.Name, name =>
            {
                name.Property(n => n.English).HasColumnName("NameEn").HasMaxLength(256).IsRequired();
                name.Property(n => n.Arabic).HasColumnName("NameAr").HasMaxLength(256);
            });

            entity.OwnsOne(l => l.Description, desc =>
            {
                desc.Property(d => d.English).HasColumnName("DescriptionEn").HasMaxLength(1000);
                desc.Property(d => d.Arabic).HasColumnName("DescriptionAr").HasMaxLength(1000);
            });

            entity.Property(l => l.IconName).HasMaxLength(100);
            entity.Property(l => l.Color).HasMaxLength(50);
            entity.Property(l => l.OwnerName).HasMaxLength(256);
            entity.Property(l => l.AllowedExtensions).HasMaxLength(500);

            entity.Ignore(l => l.DomainEvents);
        });

        modelBuilder.Entity<Folder>(entity =>
        {
            entity.ToTable("Folders");
            entity.HasKey(f => f.Id);

            entity.OwnsOne(f => f.Name, name =>
            {
                name.Property(n => n.English).HasColumnName("NameEn").HasMaxLength(256).IsRequired();
                name.Property(n => n.Arabic).HasColumnName("NameAr").HasMaxLength(256);
            });

            entity.OwnsOne(f => f.Description, desc =>
            {
                desc.Property(d => d.English).HasColumnName("DescriptionEn").HasMaxLength(1000);
                desc.Property(d => d.Arabic).HasColumnName("DescriptionAr").HasMaxLength(1000);
            });

            entity.Property(f => f.Path).HasMaxLength(1000).IsRequired();
            entity.Property(f => f.IconName).HasMaxLength(100);
            entity.Property(f => f.Color).HasMaxLength(50);

            entity.HasOne(f => f.Library)
                .WithMany(l => l.Folders)
                .HasForeignKey(f => f.LibraryId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(f => f.Parent)
                .WithMany(f => f.Children)
                .HasForeignKey(f => f.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Ignore(f => f.DomainEvents);
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.ToTable("Documents");
            entity.HasKey(d => d.Id);

            entity.Property(d => d.Name).HasMaxLength(500).IsRequired();
            entity.Property(d => d.NameArabic).HasMaxLength(500);
            entity.Property(d => d.Description).HasMaxLength(2000);
            entity.Property(d => d.DescriptionArabic).HasMaxLength(2000);
            entity.Property(d => d.FileName).HasMaxLength(500).IsRequired();
            entity.Property(d => d.FileExtension).HasMaxLength(50);
            entity.Property(d => d.ContentType).HasMaxLength(256);
            entity.Property(d => d.StoragePath).HasMaxLength(1000).IsRequired();
            entity.Property(d => d.ThumbnailPath).HasMaxLength(500);
            entity.Property(d => d.CheckedOutByName).HasMaxLength(256);
            entity.Property(d => d.ContentHash).HasMaxLength(256);

            entity.HasOne(d => d.Library)
                .WithMany(l => l.Documents)
                .HasForeignKey(d => d.LibraryId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Folder)
                .WithMany(f => f.Documents)
                .HasForeignKey(d => d.FolderId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.Ignore(d => d.DomainEvents);
        });

        modelBuilder.Entity<Documents.Domain.Entities.DocumentVersion>(entity =>
        {
            entity.ToTable("DocumentVersionHistory");
            entity.HasKey(v => v.Id);

            entity.Property(v => v.FileName).HasMaxLength(500).IsRequired();
            entity.Property(v => v.StoragePath).HasMaxLength(1000).IsRequired();
            entity.Property(v => v.ContentHash).HasMaxLength(256);
            entity.Property(v => v.ChangeNotes).HasMaxLength(2000);
            entity.Property(v => v.CreatedByName).HasMaxLength(256);

            entity.HasOne(v => v.Document)
                .WithMany(d => d.Versions)
                .HasForeignKey(v => v.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DocumentAudit>(entity =>
        {
            entity.ToTable("DocumentAudits");
            entity.HasKey(a => a.Id);

            entity.Property(a => a.Action).HasMaxLength(100).IsRequired();
            entity.Property(a => a.Details).HasMaxLength(2000);
            entity.Property(a => a.PerformedByName).HasMaxLength(256);
            entity.Property(a => a.IpAddress).HasMaxLength(50);

            entity.HasIndex(a => a.DocumentId);
            entity.HasIndex(a => a.PerformedAt);
        });

        modelBuilder.Entity<LibraryPermission>(entity =>
        {
            entity.ToTable("LibraryPermissions");
            entity.HasKey(p => p.Id);

            entity.HasOne(p => p.Library)
                .WithMany(l => l.Permissions)
                .HasForeignKey(p => p.LibraryId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(p => new { p.LibraryId, p.UserId, p.GroupId, p.RoleId });
        });

        modelBuilder.Entity<FolderPermission>(entity =>
        {
            entity.ToTable("FolderPermissions");
            entity.HasKey(p => p.Id);

            entity.Property(p => p.GrantedByName).HasMaxLength(256);

            entity.HasOne(p => p.Folder)
                .WithMany()
                .HasForeignKey(p => p.FolderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(p => new { p.FolderId, p.UserId, p.GroupId });

            entity.Ignore(p => p.DomainEvents);
        });

        modelBuilder.Entity<DocumentPermission>(entity =>
        {
            entity.ToTable("DocumentPermissions");
            entity.HasKey(p => p.Id);

            entity.Property(p => p.GrantedByName).HasMaxLength(256);

            entity.HasOne(p => p.Document)
                .WithMany()
                .HasForeignKey(p => p.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(p => new { p.DocumentId, p.UserId, p.GroupId });

            entity.Ignore(p => p.DomainEvents);
        });

        // Configure Admin Module entities
        modelBuilder.Entity<AuditLogEntry>(entity =>
        {
            entity.ToTable("AuditLogs");
            entity.HasKey(a => a.Id);

            entity.Property(a => a.Action).HasMaxLength(100).IsRequired();
            entity.Property(a => a.Category).HasMaxLength(50).IsRequired();
            entity.Property(a => a.EntityType).HasMaxLength(100);
            entity.Property(a => a.UserName).HasMaxLength(256);
            entity.Property(a => a.IpAddress).HasMaxLength(50);
            entity.Property(a => a.UserAgent).HasMaxLength(500);
            entity.Property(a => a.OldValues);
            entity.Property(a => a.NewValues);
            entity.Property(a => a.AdditionalData);

            entity.HasIndex(a => a.Timestamp);
            entity.HasIndex(a => a.UserId);
            entity.HasIndex(a => a.Category);
            entity.HasIndex(a => new { a.EntityType, a.EntityId });
        });

        modelBuilder.Entity<LegalHold>(entity =>
        {
            entity.ToTable("LegalHolds");
            entity.HasKey(h => h.Id);

            entity.Property(h => h.Name).HasMaxLength(256).IsRequired();
            entity.Property(h => h.CaseReference).HasMaxLength(100);
            entity.Property(h => h.Description).HasMaxLength(2000);
            entity.Property(h => h.Notes).HasMaxLength(2000);
            entity.Property(h => h.CreatedByName).HasMaxLength(256);

            entity.HasIndex(h => h.CaseReference);
            entity.HasIndex(h => h.Status);
        });

        modelBuilder.Entity<LegalHoldDocument>(entity =>
        {
            entity.ToTable("LegalHoldDocuments");
            entity.HasKey(d => d.Id);

            entity.Property(d => d.DocumentName).HasMaxLength(256);

            entity.HasOne(d => d.LegalHold)
                .WithMany(h => h.Documents)
                .HasForeignKey(d => d.LegalHoldId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(d => new { d.LegalHoldId, d.DocumentId });
        });

        modelBuilder.Entity<LegalHoldCustodian>(entity =>
        {
            entity.ToTable("LegalHoldCustodians");
            entity.HasKey(c => c.Id);

            entity.Property(c => c.UserName).HasMaxLength(256).IsRequired();
            entity.Property(c => c.Email).HasMaxLength(256);

            entity.HasOne(c => c.LegalHold)
                .WithMany(h => h.Custodians)
                .HasForeignKey(c => c.LegalHoldId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(c => new { c.LegalHoldId, c.UserId });
        });

        modelBuilder.Entity<QuarantinedDocument>(entity =>
        {
            entity.ToTable("QuarantinedDocuments");
            entity.HasKey(q => q.Id);

            entity.Property(q => q.ReasonDetails).HasMaxLength(2000).IsRequired();
            entity.Property(q => q.QuarantinedByName).HasMaxLength(256);
            entity.Property(q => q.ReviewNotes).HasMaxLength(2000);
            entity.Property(q => q.ReviewedByName).HasMaxLength(256);
            entity.Property(q => q.OriginalPath).HasMaxLength(1000);
            entity.Property(q => q.DocumentName).HasMaxLength(256);

            entity.HasIndex(q => q.DocumentId);
            entity.HasIndex(q => q.Status);
        });

        modelBuilder.Entity<ImpersonationSession>(entity =>
        {
            entity.ToTable("ImpersonationSessions");
            entity.HasKey(s => s.Id);

            entity.Property(s => s.AdminUserName).HasMaxLength(256).IsRequired();
            entity.Property(s => s.ImpersonatedUserName).HasMaxLength(256).IsRequired();
            entity.Property(s => s.Reason).HasMaxLength(2000).IsRequired();
            entity.Property(s => s.IpAddress).HasMaxLength(50);
            entity.Property(s => s.Actions);

            entity.HasIndex(s => s.AdminUserId);
            entity.HasIndex(s => s.ImpersonatedUserId);
            entity.HasIndex(s => s.IsActive);
        });

        // Configure AI Module entities
        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.ToTable("Conversations");
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Title).HasMaxLength(500).IsRequired();

            entity.HasMany(c => c.Messages)
                .WithOne(m => m.Conversation)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(c => c.UserId);
            entity.HasIndex(c => c.LastMessageAt);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("Messages");
            entity.HasKey(m => m.Id);

            entity.Property(m => m.Role).HasMaxLength(50).IsRequired();
            entity.Property(m => m.Content).IsRequired();
            entity.Property(m => m.CitationsJson);

            entity.HasIndex(m => m.ConversationId);
            entity.HasIndex(m => m.CreatedAt);
        });

        modelBuilder.Entity<DocumentIndex>(entity =>
        {
            entity.ToTable("DocumentIndexes");
            entity.HasKey(d => d.Id);

            entity.Property(d => d.ChunkId).HasMaxLength(100).IsRequired();
            entity.Property(d => d.Content).IsRequired();
            entity.Property(d => d.Embedding).IsRequired();
            entity.Property(d => d.MetadataJson).HasMaxLength(4000);

            entity.HasIndex(d => d.DocumentId);
            entity.HasIndex(d => d.ChunkId);
        });

        modelBuilder.Entity<AIUsageLog>(entity =>
        {
            entity.ToTable("AIUsageLogs");
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Operation).HasMaxLength(50).IsRequired();
            entity.Property(u => u.Model).HasMaxLength(100);
            entity.Property(u => u.ErrorMessage).HasMaxLength(2000);

            entity.HasIndex(u => u.UserId);
            entity.HasIndex(u => u.CreatedAt);
            entity.HasIndex(u => u.Operation);
        });

        // Configure Meeting Management entities
        modelBuilder.Entity<ExternalMeetingLinkEntity>(entity =>
        {
            entity.ToTable("ExternalMeetingLinks");
            entity.HasKey(m => m.Id);

            entity.Property(m => m.ExternalMeetingId).HasMaxLength(256).IsRequired();
            entity.Property(m => m.ExternalSystem).HasMaxLength(50).IsRequired();
            entity.Property(m => m.Title).HasMaxLength(500).IsRequired();
            entity.Property(m => m.TitleAr).HasMaxLength(500);
            entity.Property(m => m.Description).HasMaxLength(2000);
            entity.Property(m => m.Location).HasMaxLength(500);
            entity.Property(m => m.OnlineMeetingUrl).HasMaxLength(1000);
            entity.Property(m => m.RecurrencePattern).HasMaxLength(500);
            entity.Property(m => m.OrganizerEmail).HasMaxLength(256);
            entity.Property(m => m.OrganizerName).HasMaxLength(256);
            entity.Property(m => m.ExternalData);

            entity.HasIndex(m => new { m.ExternalSystem, m.ExternalMeetingId }).IsUnique();
            entity.HasIndex(m => m.StartTime);
            entity.HasIndex(m => m.Status);

            entity.Ignore(m => m.DomainEvents);
        });

        modelBuilder.Entity<MeetingDocumentLinkEntity>(entity =>
        {
            entity.ToTable("MeetingDocumentLinks");
            entity.HasKey(l => l.Id);

            entity.Property(l => l.Notes).HasMaxLength(1000);
            entity.Property(l => l.LinkedByName).HasMaxLength(256);

            entity.HasOne(l => l.Meeting)
                .WithMany(m => m.DocumentLinks)
                .HasForeignKey(l => l.MeetingId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(l => new { l.MeetingId, l.DocumentId });
            entity.HasIndex(l => l.DocumentId);

            entity.Ignore(l => l.DomainEvents);
        });

        modelBuilder.Entity<MeetingAttendeeEntity>(entity =>
        {
            entity.ToTable("MeetingAttendees");
            entity.HasKey(a => a.Id);

            entity.Property(a => a.Email).HasMaxLength(256).IsRequired();
            entity.Property(a => a.Name).HasMaxLength(256).IsRequired();

            entity.HasOne(a => a.Meeting)
                .WithMany(m => m.Attendees)
                .HasForeignKey(a => a.MeetingId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(a => new { a.MeetingId, a.Email });

            entity.Ignore(a => a.DomainEvents);
        });

        modelBuilder.Entity<MeetingAgendaItemEntity>(entity =>
        {
            entity.ToTable("MeetingAgendaItems");
            entity.HasKey(a => a.Id);

            entity.Property(a => a.Title).HasMaxLength(500).IsRequired();
            entity.Property(a => a.TitleAr).HasMaxLength(500);
            entity.Property(a => a.Description).HasMaxLength(2000);
            entity.Property(a => a.PresenterName).HasMaxLength(256);
            entity.Property(a => a.Notes).HasMaxLength(2000);

            entity.HasOne(a => a.Meeting)
                .WithMany(m => m.AgendaItems)
                .HasForeignKey(a => a.MeetingId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(a => new { a.MeetingId, a.SortOrder });

            entity.Ignore(a => a.DomainEvents);
        });

        modelBuilder.Entity<MeetingActionItemEntity>(entity =>
        {
            entity.ToTable("MeetingActionItems");
            entity.HasKey(a => a.Id);

            entity.Property(a => a.Title).HasMaxLength(500).IsRequired();
            entity.Property(a => a.Description).HasMaxLength(2000);
            entity.Property(a => a.AssignedToName).HasMaxLength(256);

            entity.HasOne(a => a.Meeting)
                .WithMany(m => m.ActionItems)
                .HasForeignKey(a => a.MeetingId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(a => a.AssignedToUserId);
            entity.HasIndex(a => a.DueDate);
            entity.HasIndex(a => a.Status);

            entity.Ignore(a => a.DomainEvents);
        });

        // Configure AI Analysis entities
        modelBuilder.Entity<DocumentAnalysisEntity>(entity =>
        {
            entity.ToTable("DocumentAnalyses");
            entity.HasKey(a => a.Id);

            entity.Property(a => a.Summary).HasMaxLength(4000);
            entity.Property(a => a.SummaryAr).HasMaxLength(4000);
            entity.Property(a => a.Language).HasMaxLength(50);
            entity.Property(a => a.SentimentLabel).HasMaxLength(50);
            entity.Property(a => a.TopicsJson);
            entity.Property(a => a.KeyPhrasesJson);
            entity.Property(a => a.SuggestedTagsJson);
            entity.Property(a => a.AnalysisVersion).HasMaxLength(50);
            entity.Property(a => a.ErrorMessage).HasMaxLength(2000);

            entity.HasIndex(a => a.DocumentId).IsUnique();
            entity.HasIndex(a => a.AnalyzedAt);

            entity.Ignore(a => a.DomainEvents);
        });

        modelBuilder.Entity<ExtractedEntityRecord>(entity =>
        {
            entity.ToTable("ExtractedEntities");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.EntityType).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Value).HasMaxLength(500).IsRequired();
            entity.Property(e => e.NormalizedValue).HasMaxLength(500);
            entity.Property(e => e.Context).HasMaxLength(1000);
            entity.Property(e => e.MetadataJson).HasMaxLength(2000);

            entity.HasOne(e => e.DocumentAnalysis)
                .WithMany(a => a.ExtractedEntities)
                .HasForeignKey(e => e.DocumentAnalysisId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.DocumentId);
            entity.HasIndex(e => e.EntityType);
            entity.HasIndex(e => new { e.EntityType, e.NormalizedValue });

            entity.Ignore(e => e.DomainEvents);
        });

        modelBuilder.Entity<BatchAnalysisJobEntity>(entity =>
        {
            entity.ToTable("BatchAnalysisJobs");
            entity.HasKey(b => b.Id);

            entity.Property(b => b.JobName).HasMaxLength(256).IsRequired();
            entity.Property(b => b.DocumentIdsJson);
            entity.Property(b => b.InitiatedByName).HasMaxLength(256);
            entity.Property(b => b.ErrorMessage).HasMaxLength(2000);

            entity.HasIndex(b => b.Status);
            entity.HasIndex(b => b.InitiatedById);

            entity.Ignore(b => b.DomainEvents);
        });

        modelBuilder.Entity<TranscriptionResultEntity>(entity =>
        {
            entity.ToTable("TranscriptionResults");
            entity.HasKey(t => t.Id);

            entity.Property(t => t.FullText).IsRequired();
            entity.Property(t => t.Language).HasMaxLength(50);
            entity.Property(t => t.SegmentsJson);
            entity.Property(t => t.SpeakersJson);
            entity.Property(t => t.ErrorMessage).HasMaxLength(2000);

            entity.HasIndex(t => t.DocumentId).IsUnique();

            entity.Ignore(t => t.DomainEvents);
        });

        modelBuilder.Entity<ContentRecommendationEntity>(entity =>
        {
            entity.ToTable("ContentRecommendations");
            entity.HasKey(r => r.Id);

            entity.Property(r => r.RecommendationType).HasMaxLength(100).IsRequired();
            entity.Property(r => r.ReasonJson).HasMaxLength(2000);

            entity.HasIndex(r => new { r.UserId, r.IsDismissed, r.ExpiresAt });
            entity.HasIndex(r => r.RecommendedDocumentId);

            entity.Ignore(r => r.DomainEvents);
        });

        // Configure Security/Encryption entities
        modelBuilder.Entity<EncryptionKeyEntity>(entity =>
        {
            entity.ToTable("EncryptionKeys");
            entity.HasKey(k => k.Id);

            entity.Property(k => k.KeyId).HasMaxLength(100).IsRequired();
            entity.Property(k => k.Purpose).HasMaxLength(100).IsRequired();
            entity.Property(k => k.Algorithm).HasMaxLength(50).IsRequired();
            entity.Property(k => k.EncryptedKeyMaterial).IsRequired();
            entity.Property(k => k.RotatedFromKeyId).HasMaxLength(100);
            entity.Property(k => k.RotatedByName).HasMaxLength(256);
            entity.Property(k => k.MetadataJson).HasMaxLength(2000);

            entity.HasIndex(k => k.KeyId).IsUnique();
            entity.HasIndex(k => new { k.Purpose, k.IsActive });

            entity.Ignore(k => k.DomainEvents);
        });

        modelBuilder.Entity<EncryptionAuditLogEntity>(entity =>
        {
            entity.ToTable("EncryptionAuditLogs");
            entity.HasKey(a => a.Id);

            entity.Property(a => a.KeyId).HasMaxLength(100).IsRequired();
            entity.Property(a => a.EntityType).HasMaxLength(100);
            entity.Property(a => a.FieldName).HasMaxLength(100);
            entity.Property(a => a.PerformedByName).HasMaxLength(256);
            entity.Property(a => a.IpAddress).HasMaxLength(50);
            entity.Property(a => a.ErrorMessage).HasMaxLength(2000);

            entity.HasIndex(a => a.KeyId);
            entity.HasIndex(a => a.PerformedAt);
            entity.HasIndex(a => new { a.EntityType, a.EntityId });

            entity.Ignore(a => a.DomainEvents);
        });

        modelBuilder.Entity<EncryptedFieldReferenceEntity>(entity =>
        {
            entity.ToTable("EncryptedFieldReferences");
            entity.HasKey(r => r.Id);

            entity.Property(r => r.EntityType).HasMaxLength(100).IsRequired();
            entity.Property(r => r.FieldName).HasMaxLength(100).IsRequired();
            entity.Property(r => r.KeyId).HasMaxLength(100).IsRequired();

            entity.HasIndex(r => new { r.EntityType, r.EntityId, r.FieldName }).IsUnique();
            entity.HasIndex(r => r.KeyId);
            entity.HasIndex(r => r.NeedsReEncryption);

            entity.Ignore(r => r.DomainEvents);
        });

        // Configure Service Catalog entities
        modelBuilder.Entity<ServiceCategoryEntity>(entity =>
        {
            entity.ToTable("ServiceCategories");
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Name).HasMaxLength(256).IsRequired();
            entity.Property(c => c.NameAr).HasMaxLength(256).IsRequired();
            entity.Property(c => c.Description).HasMaxLength(2000);
            entity.Property(c => c.DescriptionAr).HasMaxLength(2000);
            entity.Property(c => c.Icon).HasMaxLength(100);

            entity.HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(c => c.ParentCategoryId);
            entity.HasIndex(c => c.IsActive);

            entity.Ignore(c => c.DomainEvents);
        });

        modelBuilder.Entity<CatalogServiceEntity>(entity =>
        {
            entity.ToTable("CatalogServices");
            entity.HasKey(s => s.Id);

            entity.Property(s => s.Name).HasMaxLength(256).IsRequired();
            entity.Property(s => s.NameAr).HasMaxLength(256).IsRequired();
            entity.Property(s => s.Description).HasMaxLength(2000).IsRequired();
            entity.Property(s => s.DescriptionAr).HasMaxLength(2000);
            entity.Property(s => s.Icon).HasMaxLength(100);
            entity.Property(s => s.SlaEscalationEmailsJson).HasMaxLength(2000);
            entity.Property(s => s.FieldsJson);
            entity.Property(s => s.ApproverIdsJson).HasMaxLength(2000);
            entity.Property(s => s.WorkflowId).HasMaxLength(100);

            entity.HasOne(s => s.Category)
                .WithMany(c => c.Services)
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(s => s.CategoryId);
            entity.HasIndex(s => s.IsActive);

            entity.Ignore(s => s.DomainEvents);
        });

        modelBuilder.Entity<ServiceRequestEntity>(entity =>
        {
            entity.ToTable("ServiceRequests");
            entity.HasKey(r => r.Id);

            entity.Property(r => r.RequestNumber).HasMaxLength(50).IsRequired();
            entity.Property(r => r.RequesterName).HasMaxLength(256).IsRequired();
            entity.Property(r => r.RequesterEmail).HasMaxLength(256).IsRequired();
            entity.Property(r => r.RequesterDepartment).HasMaxLength(256);
            entity.Property(r => r.FormDataJson);
            entity.Property(r => r.Notes).HasMaxLength(2000);
            entity.Property(r => r.AssignedToName).HasMaxLength(256);

            entity.HasOne(r => r.Service)
                .WithMany(s => s.Requests)
                .HasForeignKey(r => r.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(r => r.RequestNumber).IsUnique();
            entity.HasIndex(r => r.RequesterId);
            entity.HasIndex(r => r.Status);
            entity.HasIndex(r => r.SubmittedAt);
            entity.HasIndex(r => new { r.IsSlaBreached, r.Status });

            entity.Ignore(r => r.DomainEvents);
        });

        modelBuilder.Entity<RequestApprovalEntity>(entity =>
        {
            entity.ToTable("RequestApprovals");
            entity.HasKey(a => a.Id);

            entity.Property(a => a.ApproverName).HasMaxLength(256).IsRequired();
            entity.Property(a => a.Comments).HasMaxLength(2000);

            entity.HasOne(a => a.Request)
                .WithMany(r => r.Approvals)
                .HasForeignKey(a => a.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(a => new { a.RequestId, a.ApprovalOrder });
            entity.HasIndex(a => new { a.ApproverId, a.Status });

            entity.Ignore(a => a.DomainEvents);
        });

        modelBuilder.Entity<RequestCommentEntity>(entity =>
        {
            entity.ToTable("RequestComments");
            entity.HasKey(c => c.Id);

            entity.Property(c => c.AuthorName).HasMaxLength(256).IsRequired();
            entity.Property(c => c.Content).HasMaxLength(4000).IsRequired();

            entity.HasOne(c => c.Request)
                .WithMany(r => r.Comments)
                .HasForeignKey(c => c.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(c => c.RequestId);

            entity.Ignore(c => c.DomainEvents);
        });

        modelBuilder.Entity<RequestAttachmentEntity>(entity =>
        {
            entity.ToTable("RequestAttachments");
            entity.HasKey(a => a.Id);

            entity.Property(a => a.FileName).HasMaxLength(500).IsRequired();
            entity.Property(a => a.ContentType).HasMaxLength(256).IsRequired();
            entity.Property(a => a.StoragePath).HasMaxLength(1000).IsRequired();

            entity.HasOne(a => a.Request)
                .WithMany(r => r.Attachments)
                .HasForeignKey(a => a.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(a => a.RequestId);

            entity.Ignore(a => a.DomainEvents);
        });

        modelBuilder.Entity<RequestStatusHistoryEntity>(entity =>
        {
            entity.ToTable("RequestStatusHistory");
            entity.HasKey(h => h.Id);

            entity.Property(h => h.ChangedByName).HasMaxLength(256).IsRequired();
            entity.Property(h => h.Reason).HasMaxLength(2000);

            entity.HasOne(h => h.Request)
                .WithMany(r => r.StatusHistory)
                .HasForeignKey(h => h.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(h => h.RequestId);

            entity.Ignore(h => h.DomainEvents);
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
