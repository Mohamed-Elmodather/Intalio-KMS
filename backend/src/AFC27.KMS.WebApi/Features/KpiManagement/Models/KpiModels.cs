using System;
using System.Collections.Generic;

namespace AFC27.KMS.WebApi.Features.KpiManagement.Models;

/// <summary>
/// KPI definition
/// </summary>
public class KpiDefinition
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public KpiType Type { get; set; }
    public KpiScope Scope { get; set; }
    public string Unit { get; set; } = string.Empty;
    public MeasurementFrequency Frequency { get; set; }
    public double? TargetValue { get; set; }
    public double? MinThreshold { get; set; }
    public double? MaxThreshold { get; set; }
    public TrendDirection DesiredTrend { get; set; }
    public string? Formula { get; set; }
    public string? DataSource { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsAutoCalculated { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public List<KpiThreshold> Thresholds { get; set; } = new();
}

public enum KpiType
{
    Numeric,
    Percentage,
    Currency,
    Duration,
    Count,
    Ratio,
    Boolean
}

public enum KpiScope
{
    Personal,
    Team,
    Department,
    Organization
}

public enum MeasurementFrequency
{
    Daily,
    Weekly,
    Monthly,
    Quarterly,
    Yearly
}

public enum TrendDirection
{
    Higher,
    Lower,
    Stable
}

/// <summary>
/// KPI threshold for status indicators
/// </summary>
public class KpiThreshold
{
    public string Status { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public double MinValue { get; set; }
    public double MaxValue { get; set; }
}

/// <summary>
/// KPI value entry
/// </summary>
public class KpiValue
{
    public Guid Id { get; set; }
    public Guid KpiDefinitionId { get; set; }
    public Guid? UserId { get; set; }
    public Guid? TeamId { get; set; }
    public double Value { get; set; }
    public string? Notes { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public DateTime RecordedAt { get; set; }
    public Guid RecordedBy { get; set; }
    public KpiValueStatus Status { get; set; }
    public double? TargetValue { get; set; }
    public double? PreviousValue { get; set; }
    public double? PercentageChange { get; set; }
}

public enum KpiValueStatus
{
    OnTrack,
    AtRisk,
    BelowTarget,
    ExceedsTarget
}

/// <summary>
/// KPI assignment to user or team
/// </summary>
public class KpiAssignment
{
    public Guid Id { get; set; }
    public Guid KpiDefinitionId { get; set; }
    public string KpiName { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public Guid? TeamId { get; set; }
    public double TargetValue { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double Weight { get; set; } = 1.0;
    public bool IsActive { get; set; } = true;
    public DateTime AssignedAt { get; set; }
    public Guid AssignedBy { get; set; }
}

/// <summary>
/// Team definition
/// </summary>
public class Team
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? ParentTeamId { get; set; }
    public Guid ManagerId { get; set; }
    public List<Guid> MemberIds { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// KPI dashboard card
/// </summary>
public class KpiDashboardCard
{
    public Guid KpiId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public double CurrentValue { get; set; }
    public double TargetValue { get; set; }
    public double PercentageOfTarget { get; set; }
    public KpiValueStatus Status { get; set; }
    public string StatusColor { get; set; } = string.Empty;
    public double? TrendPercentage { get; set; }
    public TrendDirection TrendDirection { get; set; }
    public string Unit { get; set; } = string.Empty;
    public List<KpiTrendPoint> TrendData { get; set; } = new();
}

public class KpiTrendPoint
{
    public DateTime Date { get; set; }
    public double Value { get; set; }
    public double? Target { get; set; }
}

/// <summary>
/// Personal KPI summary
/// </summary>
public class PersonalKpiSummary
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int TotalKpis { get; set; }
    public int OnTrack { get; set; }
    public int AtRisk { get; set; }
    public int BelowTarget { get; set; }
    public int ExceedsTarget { get; set; }
    public double OverallScore { get; set; }
    public double PreviousPeriodScore { get; set; }
    public List<KpiDashboardCard> TopKpis { get; set; } = new();
    public List<KpiDashboardCard> NeedsAttention { get; set; } = new();
}

/// <summary>
/// Team KPI summary
/// </summary>
public class TeamKpiSummary
{
    public Guid TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public int MemberCount { get; set; }
    public int TotalKpis { get; set; }
    public double TeamOverallScore { get; set; }
    public double AverageMemberScore { get; set; }
    public List<KpiDashboardCard> TeamKpis { get; set; } = new();
    public List<TeamMemberKpiStatus> MemberStatuses { get; set; } = new();
}

public class TeamMemberKpiStatus
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public double OverallScore { get; set; }
    public int OnTrack { get; set; }
    public int NeedsAttention { get; set; }
}

/// <summary>
/// Request to create a KPI definition
/// </summary>
public class CreateKpiRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public KpiType Type { get; set; }
    public KpiScope Scope { get; set; }
    public string Unit { get; set; } = string.Empty;
    public MeasurementFrequency Frequency { get; set; }
    public double? TargetValue { get; set; }
    public TrendDirection DesiredTrend { get; set; }
    public string? Formula { get; set; }
    public bool IsAutoCalculated { get; set; }
    public List<KpiThreshold>? Thresholds { get; set; }
}

/// <summary>
/// Request to record a KPI value
/// </summary>
public class RecordKpiValueRequest
{
    public Guid KpiDefinitionId { get; set; }
    public Guid? UserId { get; set; }
    public Guid? TeamId { get; set; }
    public double Value { get; set; }
    public string? Notes { get; set; }
    public DateTime? PeriodStart { get; set; }
    public DateTime? PeriodEnd { get; set; }
}

/// <summary>
/// Request to assign KPI to user or team
/// </summary>
public class AssignKpiRequest
{
    public Guid KpiDefinitionId { get; set; }
    public Guid? UserId { get; set; }
    public Guid? TeamId { get; set; }
    public double TargetValue { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double Weight { get; set; } = 1.0;
}

/// <summary>
/// KPI report
/// </summary>
public class KpiReport
{
    public string ReportTitle { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public KpiScope Scope { get; set; }
    public int TotalKpisTracked { get; set; }
    public double OverallPerformanceScore { get; set; }
    public List<CategoryPerformance> PerformanceByCategory { get; set; } = new();
    public List<KpiDetailedResult> DetailedResults { get; set; } = new();
    public List<string> Highlights { get; set; } = new();
    public List<string> AreasForImprovement { get; set; } = new();
}

public class CategoryPerformance
{
    public string Category { get; set; } = string.Empty;
    public int KpiCount { get; set; }
    public double AveragePerformance { get; set; }
    public int OnTrack { get; set; }
    public int BelowTarget { get; set; }
}

public class KpiDetailedResult
{
    public string KpiName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public double ActualValue { get; set; }
    public double TargetValue { get; set; }
    public double PerformancePercentage { get; set; }
    public KpiValueStatus Status { get; set; }
    public string Trend { get; set; } = string.Empty;
}
