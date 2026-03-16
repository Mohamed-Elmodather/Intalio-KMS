using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Admin.Domain.Entities;

/// <summary>
/// Critical knowledge asset registered for ISO 30401 compliance.
/// Tracks knowledge holders, risk levels, and succession planning.
/// </summary>
public class KnowledgeAsset : AuditableEntity
{
    public LocalizedString Name { get; private set; } = null!;
    public string Description { get; private set; } = string.Empty;
    public string? DescriptionArabic { get; private set; }
    public KnowledgeDomain Domain { get; private set; }
    public KnowledgeCriticality Criticality { get; private set; }

    // Knowledge holders
    public Guid? PrimaryHolderId { get; private set; }
    public string? PrimaryHolderName { get; private set; }
    public List<Guid> SecondaryHolderIds { get; private set; } = new();

    // Risk assessment
    public KnowledgeRiskLevel RiskLevel { get; private set; }
    public string? RiskDescription { get; private set; }
    public string? RiskDescriptionArabic { get; private set; }
    public string? MitigationPlan { get; private set; }
    public string? MitigationPlanArabic { get; private set; }

    // Documentation status
    public KnowledgeDocumentationLevel DocumentationLevel { get; private set; }
    public Guid? RelatedSpaceId { get; private set; }

    // Assessment tracking
    public DateTime LastAssessedAt { get; private set; }
    public DateTime NextAssessmentDue { get; private set; }
    public Guid? AssessedById { get; private set; }
    public string? AssessedByName { get; private set; }

    private KnowledgeAsset() { }

    public static KnowledgeAsset Create(
        LocalizedString name,
        string description,
        string? descriptionArabic,
        KnowledgeDomain domain,
        KnowledgeCriticality criticality,
        KnowledgeRiskLevel riskLevel,
        int assessmentIntervalDays = 180)
    {
        return new KnowledgeAsset
        {
            Name = name,
            Description = description,
            DescriptionArabic = descriptionArabic,
            Domain = domain,
            Criticality = criticality,
            RiskLevel = riskLevel,
            DocumentationLevel = KnowledgeDocumentationLevel.None,
            LastAssessedAt = DateTime.UtcNow,
            NextAssessmentDue = DateTime.UtcNow.AddDays(assessmentIntervalDays)
        };
    }

    public void AssignPrimaryHolder(Guid holderId, string holderName)
    {
        PrimaryHolderId = holderId;
        PrimaryHolderName = holderName;
    }

    public void AddSecondaryHolder(Guid holderId)
    {
        if (!SecondaryHolderIds.Contains(holderId))
            SecondaryHolderIds.Add(holderId);
    }

    public void RemoveSecondaryHolder(Guid holderId) => SecondaryHolderIds.Remove(holderId);

    public void UpdateRiskAssessment(KnowledgeRiskLevel level, string? description, string? descriptionArabic, string? mitigation, string? mitigationArabic)
    {
        RiskLevel = level;
        RiskDescription = description;
        RiskDescriptionArabic = descriptionArabic;
        MitigationPlan = mitigation;
        MitigationPlanArabic = mitigationArabic;
    }

    public void RecordAssessment(Guid assessedById, string assessedByName, int nextIntervalDays = 180)
    {
        LastAssessedAt = DateTime.UtcNow;
        NextAssessmentDue = DateTime.UtcNow.AddDays(nextIntervalDays);
        AssessedById = assessedById;
        AssessedByName = assessedByName;
    }

    public void SetDocumentationLevel(KnowledgeDocumentationLevel level) => DocumentationLevel = level;

    public void LinkSpace(Guid spaceId) => RelatedSpaceId = spaceId;

    public bool IsAssessmentOverdue => NextAssessmentDue < DateTime.UtcNow;
    public bool IsSinglePointOfFailure => PrimaryHolderId.HasValue && SecondaryHolderIds.Count == 0;
}

public enum KnowledgeDomain
{
    Operations,
    Technical,
    Commercial,
    Legal,
    HumanResources,
    Finance,
    Communication,
    Safety,
    Innovation,
    TournamentSpecific
}

public enum KnowledgeCriticality
{
    Critical,
    Important,
    Supporting
}

public enum KnowledgeRiskLevel
{
    High,
    Medium,
    Low
}

public enum KnowledgeDocumentationLevel
{
    None,
    Minimal,
    Partial,
    Comprehensive,
    FullyDocumented
}

/// <summary>
/// KM objective for ISO 30401 Clause 6.2 compliance.
/// Tracks measurable KM targets with current progress.
/// </summary>
public class KMObjective : AuditableEntity
{
    public LocalizedString Title { get; private set; } = null!;
    public string Description { get; private set; } = string.Empty;
    public string? DescriptionArabic { get; private set; }
    public string Metric { get; private set; } = string.Empty;
    public double TargetValue { get; private set; }
    public double? CurrentValue { get; private set; }
    public string Unit { get; private set; } = string.Empty;
    public DateTime PeriodStart { get; private set; }
    public DateTime PeriodEnd { get; private set; }
    public Guid OwnerId { get; private set; }
    public string OwnerName { get; private set; } = string.Empty;
    public KMObjectiveStatus Status { get; private set; }
    public DateTime? LastMeasuredAt { get; private set; }
    public string? Notes { get; private set; }
    public string? NotesArabic { get; private set; }

    private KMObjective() { }

    public static KMObjective Create(
        LocalizedString title,
        string description,
        string? descriptionArabic,
        string metric,
        double targetValue,
        string unit,
        DateTime periodStart,
        DateTime periodEnd,
        Guid ownerId,
        string ownerName)
    {
        return new KMObjective
        {
            Title = title,
            Description = description,
            DescriptionArabic = descriptionArabic,
            Metric = metric,
            TargetValue = targetValue,
            Unit = unit,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            OwnerId = ownerId,
            OwnerName = ownerName,
            Status = KMObjectiveStatus.NotStarted
        };
    }

    public void UpdateMeasurement(double currentValue, string? notes = null, string? notesArabic = null)
    {
        CurrentValue = currentValue;
        LastMeasuredAt = DateTime.UtcNow;
        Notes = notes;
        NotesArabic = notesArabic;

        // Auto-calculate status
        if (currentValue >= TargetValue)
            Status = KMObjectiveStatus.Achieved;
        else if (currentValue >= TargetValue * 0.8)
            Status = KMObjectiveStatus.OnTrack;
        else if (currentValue >= TargetValue * 0.5)
            Status = KMObjectiveStatus.AtRisk;
        else
            Status = KMObjectiveStatus.Behind;
    }

    public double? ProgressPercentage => TargetValue > 0 && CurrentValue.HasValue
        ? Math.Min(100, (CurrentValue.Value / TargetValue) * 100)
        : null;
}

public enum KMObjectiveStatus
{
    NotStarted,
    OnTrack,
    AtRisk,
    Behind,
    Achieved
}

/// <summary>
/// KM policy document for ISO 30401 Clause 5.2 compliance.
/// </summary>
public class KMPolicy : AuditableEntity
{
    public LocalizedString Title { get; private set; } = null!;
    public string Content { get; private set; } = string.Empty;
    public string? ContentArabic { get; private set; }
    public int Version { get; private set; }
    public DateTime EffectiveDate { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public Guid ApprovedById { get; private set; }
    public string ApprovedByName { get; private set; } = string.Empty;
    public KMPolicyStatus Status { get; private set; }
    public bool RequiresAcknowledgment { get; private set; }
    public string? Scope { get; private set; }
    public string? ScopeArabic { get; private set; }

    // Navigation
    public virtual ICollection<KMPolicyAcknowledgment> Acknowledgments { get; private set; } = new List<KMPolicyAcknowledgment>();

    private KMPolicy() { }

    public static KMPolicy Create(
        LocalizedString title,
        string content,
        string? contentArabic,
        DateTime effectiveDate,
        Guid approvedById,
        string approvedByName,
        bool requiresAcknowledgment = false)
    {
        return new KMPolicy
        {
            Title = title,
            Content = content,
            ContentArabic = contentArabic,
            Version = 1,
            EffectiveDate = effectiveDate,
            ApprovedById = approvedById,
            ApprovedByName = approvedByName,
            Status = KMPolicyStatus.Draft,
            RequiresAcknowledgment = requiresAcknowledgment
        };
    }

    public void Activate()
    {
        Status = KMPolicyStatus.Active;
    }

    public void Supersede()
    {
        Status = KMPolicyStatus.Superseded;
    }

    public void Expire()
    {
        Status = KMPolicyStatus.Expired;
    }

    public void UpdateContent(string content, string? contentArabic)
    {
        Content = content;
        ContentArabic = contentArabic;
        Version++;
    }

    public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.UtcNow;
}

public enum KMPolicyStatus
{
    Draft,
    Active,
    Superseded,
    Expired
}

/// <summary>
/// Tracks user acknowledgment of a KM policy.
/// </summary>
public class KMPolicyAcknowledgment
{
    public Guid Id { get; set; }
    public Guid PolicyId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime AcknowledgedAt { get; set; }

    public virtual KMPolicy Policy { get; set; } = null!;
}
