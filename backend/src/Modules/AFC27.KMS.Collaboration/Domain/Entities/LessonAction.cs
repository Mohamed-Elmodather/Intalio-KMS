using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Collaboration.Domain.Entities;

/// <summary>
/// Action item associated with a lesson learned.
/// Tracks corrective/preventive actions from identification through verification.
/// </summary>
public class LessonAction : AuditableEntity
{
    public Guid LessonLearnedId { get; private set; }

    // Action content (bilingual)
    public string Description { get; private set; } = string.Empty;
    public string? DescriptionArabic { get; private set; }

    // Assignment
    public Guid AssigneeId { get; private set; }
    public string AssigneeName { get; private set; } = string.Empty;
    public Guid? DelegatedToId { get; private set; }
    public string? DelegatedToName { get; private set; }

    // Tracking
    public LessonActionStatus Status { get; private set; }
    public LessonActionPriority Priority { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public Guid? CompletedById { get; private set; }
    public string? CompletionNotes { get; private set; }
    public string? CompletionNotesArabic { get; private set; }

    // Verification
    public DateTime? VerifiedAt { get; private set; }
    public Guid? VerifiedById { get; private set; }
    public string? VerifiedByName { get; private set; }
    public string? VerificationNotes { get; private set; }

    // Affected document/procedure
    public Guid? AffectedDocumentId { get; private set; }
    public string? AffectedDocumentTitle { get; private set; }
    public string? AffectedDocumentType { get; private set; }

    // Escalation
    public DateTime? EscalatedAt { get; private set; }
    public Guid? EscalatedToId { get; private set; }
    public string? EscalatedToName { get; private set; }
    public int ReminderCount { get; private set; }

    // Ordering
    public int SortOrder { get; private set; }

    // Navigation
    public virtual LessonLearned LessonLearned { get; private set; } = null!;

    private LessonAction() { }

    public static LessonAction Create(
        Guid lessonLearnedId,
        string description,
        string? descriptionArabic,
        Guid assigneeId,
        string assigneeName,
        DateTime dueDate,
        LessonActionPriority priority,
        int sortOrder = 0,
        Guid? affectedDocumentId = null,
        string? affectedDocumentTitle = null,
        string? affectedDocumentType = null)
    {
        return new LessonAction
        {
            Id = Guid.NewGuid(),
            LessonLearnedId = lessonLearnedId,
            Description = description,
            DescriptionArabic = descriptionArabic,
            AssigneeId = assigneeId,
            AssigneeName = assigneeName,
            DueDate = dueDate,
            Priority = priority,
            Status = LessonActionStatus.Open,
            SortOrder = sortOrder,
            ReminderCount = 0,
            AffectedDocumentId = affectedDocumentId,
            AffectedDocumentTitle = affectedDocumentTitle,
            AffectedDocumentType = affectedDocumentType
        };
    }

    public void Update(string description, string? descriptionArabic, DateTime dueDate, LessonActionPriority priority)
    {
        Description = description;
        DescriptionArabic = descriptionArabic;
        DueDate = dueDate;
        Priority = priority;
    }

    public void StartProgress()
    {
        if (Status != LessonActionStatus.Open) return;
        Status = LessonActionStatus.InProgress;
        StartedAt = DateTime.UtcNow;
    }

    public void Complete(Guid completedById, string? notes, string? notesArabic)
    {
        Status = LessonActionStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        CompletedById = completedById;
        CompletionNotes = notes;
        CompletionNotesArabic = notesArabic;
    }

    public void Verify(Guid verifiedById, string verifiedByName, string? notes)
    {
        if (Status != LessonActionStatus.Completed) return;
        Status = LessonActionStatus.Verified;
        VerifiedAt = DateTime.UtcNow;
        VerifiedById = verifiedById;
        VerifiedByName = verifiedByName;
        VerificationNotes = notes;
    }

    public void Cancel()
    {
        Status = LessonActionStatus.Cancelled;
    }

    public void Escalate(Guid escalatedToId, string escalatedToName)
    {
        EscalatedAt = DateTime.UtcNow;
        EscalatedToId = escalatedToId;
        EscalatedToName = escalatedToName;
    }

    public void IncrementReminder() => ReminderCount++;

    public void Delegate(Guid delegatedToId, string delegatedToName)
    {
        DelegatedToId = delegatedToId;
        DelegatedToName = delegatedToName;
    }

    public void UpdateDueDate(DateTime newDueDate) => DueDate = newDueDate;

    public void LinkDocument(Guid documentId, string documentTitle, string documentType)
    {
        AffectedDocumentId = documentId;
        AffectedDocumentTitle = documentTitle;
        AffectedDocumentType = documentType;
    }

    public void SetSortOrder(int order) => SortOrder = order;

    public bool IsOverdue => Status is LessonActionStatus.Open or LessonActionStatus.InProgress
                             && DueDate < DateTime.UtcNow;
}

/// <summary>
/// Status of a lesson action item.
/// </summary>
public enum LessonActionStatus
{
    Open,
    InProgress,
    Completed,
    Verified,
    Cancelled
}

/// <summary>
/// Priority of a lesson action item.
/// </summary>
public enum LessonActionPriority
{
    Low,
    Normal,
    High,
    Urgent
}
