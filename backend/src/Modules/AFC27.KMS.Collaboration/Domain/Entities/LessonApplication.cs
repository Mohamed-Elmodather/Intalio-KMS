using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Collaboration.Domain.Entities;

/// <summary>
/// Tracks when a lesson learned is applied/adopted by another project.
/// Enables cross-project knowledge reuse measurement (ISO 30401 Clause 8.5).
/// </summary>
public class LessonApplication : Entity
{
    public Guid LessonLearnedId { get; private set; }
    public Guid AppliedByProjectId { get; private set; }
    public string AppliedByProjectName { get; private set; } = string.Empty;
    public Guid AppliedById { get; private set; }
    public string AppliedByName { get; private set; } = string.Empty;
    public DateTime AppliedAt { get; private set; }
    public string? Notes { get; private set; }
    public string? NotesArabic { get; private set; }
    public ApplicationOutcome? Outcome { get; private set; }
    public string? OutcomeNotes { get; private set; }

    // Navigation
    public virtual LessonLearned LessonLearned { get; private set; } = null!;

    private LessonApplication() { }

    public static LessonApplication Create(
        Guid lessonLearnedId,
        Guid appliedByProjectId,
        string appliedByProjectName,
        Guid appliedById,
        string appliedByName,
        string? notes = null,
        string? notesArabic = null)
    {
        return new LessonApplication
        {
            Id = Guid.NewGuid(),
            LessonLearnedId = lessonLearnedId,
            AppliedByProjectId = appliedByProjectId,
            AppliedByProjectName = appliedByProjectName,
            AppliedById = appliedById,
            AppliedByName = appliedByName,
            AppliedAt = DateTime.UtcNow,
            Notes = notes,
            NotesArabic = notesArabic
        };
    }

    public void RecordOutcome(ApplicationOutcome outcome, string? notes)
    {
        Outcome = outcome;
        OutcomeNotes = notes;
    }
}

/// <summary>
/// Outcome of applying a lesson to a new project.
/// </summary>
public enum ApplicationOutcome
{
    Effective,
    PartiallyEffective,
    NotEffective,
    NotApplicable
}
