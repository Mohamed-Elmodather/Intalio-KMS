using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AFC27.KMS.WebApi.Features.KpiManagement.Models;

namespace AFC27.KMS.WebApi.Features.KpiManagement.Services;

/// <summary>
/// Implementation of KPI management service
/// </summary>
public class KpiService : IKpiService
{
    private readonly ILogger<KpiService> _logger;

    // In-memory storage for demo
    private static readonly List<KpiDefinition> _kpis = new();
    private static readonly List<KpiValue> _values = new();
    private static readonly List<KpiAssignment> _assignments = new();
    private static readonly List<Team> _teams = new();

    public KpiService(ILogger<KpiService> logger)
    {
        _logger = logger;

        if (!_kpis.Any())
        {
            SeedDemoKpis();
        }
    }

    public Task<KpiDefinition> CreateKpiAsync(
        CreateKpiRequest request,
        Guid createdBy,
        CancellationToken cancellationToken = default)
    {
        var kpi = new KpiDefinition
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Category = request.Category,
            Type = request.Type,
            Scope = request.Scope,
            Unit = request.Unit,
            Frequency = request.Frequency,
            TargetValue = request.TargetValue,
            DesiredTrend = request.DesiredTrend,
            Formula = request.Formula,
            IsAutoCalculated = request.IsAutoCalculated,
            Thresholds = request.Thresholds ?? GetDefaultThresholds(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy,
            IsActive = true
        };

        _kpis.Add(kpi);

        _logger.LogInformation("Created KPI {KpiId}: {KpiName}", kpi.Id, kpi.Name);

        return Task.FromResult(kpi);
    }

    public Task<KpiDefinition?> GetKpiAsync(
        Guid kpiId,
        CancellationToken cancellationToken = default)
    {
        var kpi = _kpis.FirstOrDefault(k => k.Id == kpiId);
        return Task.FromResult(kpi);
    }

    public Task<List<KpiDefinition>> GetKpisAsync(
        string? category = null,
        KpiScope? scope = null,
        bool activeOnly = true,
        CancellationToken cancellationToken = default)
    {
        var query = _kpis.AsEnumerable();

        if (activeOnly)
            query = query.Where(k => k.IsActive);

        if (!string.IsNullOrEmpty(category))
            query = query.Where(k => k.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

        if (scope.HasValue)
            query = query.Where(k => k.Scope == scope.Value);

        return Task.FromResult(query.OrderBy(k => k.DisplayOrder).ToList());
    }

    public Task<KpiDefinition> UpdateKpiAsync(
        Guid kpiId,
        CreateKpiRequest request,
        CancellationToken cancellationToken = default)
    {
        var kpi = _kpis.FirstOrDefault(k => k.Id == kpiId)
            ?? throw new KeyNotFoundException($"KPI {kpiId} not found");

        kpi.Name = request.Name;
        kpi.Description = request.Description;
        kpi.Category = request.Category;
        kpi.Type = request.Type;
        kpi.Scope = request.Scope;
        kpi.Unit = request.Unit;
        kpi.Frequency = request.Frequency;
        kpi.TargetValue = request.TargetValue;
        kpi.DesiredTrend = request.DesiredTrend;
        kpi.Formula = request.Formula;
        kpi.IsAutoCalculated = request.IsAutoCalculated;
        kpi.Thresholds = request.Thresholds ?? kpi.Thresholds;

        return Task.FromResult(kpi);
    }

    public Task DeactivateKpiAsync(
        Guid kpiId,
        CancellationToken cancellationToken = default)
    {
        var kpi = _kpis.FirstOrDefault(k => k.Id == kpiId);
        if (kpi != null)
            kpi.IsActive = false;

        return Task.CompletedTask;
    }

    public Task<KpiValue> RecordValueAsync(
        RecordKpiValueRequest request,
        Guid recordedBy,
        CancellationToken cancellationToken = default)
    {
        var kpi = _kpis.FirstOrDefault(k => k.Id == request.KpiDefinitionId)
            ?? throw new KeyNotFoundException($"KPI {request.KpiDefinitionId} not found");

        var previousValue = _values
            .Where(v => v.KpiDefinitionId == request.KpiDefinitionId)
            .Where(v => v.UserId == request.UserId)
            .Where(v => v.TeamId == request.TeamId)
            .OrderByDescending(v => v.PeriodEnd)
            .FirstOrDefault();

        var value = new KpiValue
        {
            Id = Guid.NewGuid(),
            KpiDefinitionId = request.KpiDefinitionId,
            UserId = request.UserId,
            TeamId = request.TeamId,
            Value = request.Value,
            Notes = request.Notes,
            PeriodStart = request.PeriodStart ?? GetPeriodStart(kpi.Frequency),
            PeriodEnd = request.PeriodEnd ?? DateTime.UtcNow,
            RecordedAt = DateTime.UtcNow,
            RecordedBy = recordedBy,
            TargetValue = kpi.TargetValue,
            PreviousValue = previousValue?.Value,
            PercentageChange = previousValue != null
                ? (request.Value - previousValue.Value) / previousValue.Value * 100
                : null,
            Status = CalculateStatus(request.Value, kpi)
        };

        _values.Add(value);

        _logger.LogInformation(
            "Recorded KPI value for {KpiId}: {Value}",
            request.KpiDefinitionId, request.Value);

        return Task.FromResult(value);
    }

    public Task<List<KpiValue>> GetValuesAsync(
        Guid kpiId,
        Guid? userId = null,
        Guid? teamId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = _values.Where(v => v.KpiDefinitionId == kpiId);

        if (userId.HasValue)
            query = query.Where(v => v.UserId == userId.Value);

        if (teamId.HasValue)
            query = query.Where(v => v.TeamId == teamId.Value);

        if (fromDate.HasValue)
            query = query.Where(v => v.PeriodStart >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(v => v.PeriodEnd <= toDate.Value);

        return Task.FromResult(query.OrderByDescending(v => v.PeriodEnd).ToList());
    }

    public Task<KpiValue?> GetLatestValueAsync(
        Guid kpiId,
        Guid? userId = null,
        Guid? teamId = null,
        CancellationToken cancellationToken = default)
    {
        var value = _values
            .Where(v => v.KpiDefinitionId == kpiId)
            .Where(v => userId == null || v.UserId == userId)
            .Where(v => teamId == null || v.TeamId == teamId)
            .OrderByDescending(v => v.PeriodEnd)
            .FirstOrDefault();

        return Task.FromResult(value);
    }

    public Task<KpiAssignment> AssignKpiAsync(
        AssignKpiRequest request,
        Guid assignedBy,
        CancellationToken cancellationToken = default)
    {
        var kpi = _kpis.FirstOrDefault(k => k.Id == request.KpiDefinitionId)
            ?? throw new KeyNotFoundException($"KPI {request.KpiDefinitionId} not found");

        var assignment = new KpiAssignment
        {
            Id = Guid.NewGuid(),
            KpiDefinitionId = request.KpiDefinitionId,
            KpiName = kpi.Name,
            UserId = request.UserId,
            TeamId = request.TeamId,
            TargetValue = request.TargetValue,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Weight = request.Weight,
            IsActive = true,
            AssignedAt = DateTime.UtcNow,
            AssignedBy = assignedBy
        };

        _assignments.Add(assignment);

        return Task.FromResult(assignment);
    }

    public Task<List<KpiAssignment>> GetUserAssignmentsAsync(
        Guid userId,
        bool activeOnly = true,
        CancellationToken cancellationToken = default)
    {
        var query = _assignments.Where(a => a.UserId == userId);

        if (activeOnly)
            query = query.Where(a => a.IsActive && a.EndDate >= DateTime.UtcNow);

        return Task.FromResult(query.ToList());
    }

    public Task<List<KpiAssignment>> GetTeamAssignmentsAsync(
        Guid teamId,
        bool activeOnly = true,
        CancellationToken cancellationToken = default)
    {
        var query = _assignments.Where(a => a.TeamId == teamId);

        if (activeOnly)
            query = query.Where(a => a.IsActive && a.EndDate >= DateTime.UtcNow);

        return Task.FromResult(query.ToList());
    }

    public async Task<PersonalKpiSummary> GetPersonalSummaryAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var assignments = await GetUserAssignmentsAsync(userId, true, cancellationToken);
        var cards = new List<KpiDashboardCard>();

        foreach (var assignment in assignments)
        {
            var latestValue = await GetLatestValueAsync(assignment.KpiDefinitionId, userId, null, cancellationToken);
            var kpi = await GetKpiAsync(assignment.KpiDefinitionId, cancellationToken);

            if (kpi != null)
            {
                cards.Add(CreateDashboardCard(kpi, latestValue, assignment.TargetValue));
            }
        }

        return new PersonalKpiSummary
        {
            UserId = userId,
            UserName = "Current User",
            TotalKpis = cards.Count,
            OnTrack = cards.Count(c => c.Status == KpiValueStatus.OnTrack),
            AtRisk = cards.Count(c => c.Status == KpiValueStatus.AtRisk),
            BelowTarget = cards.Count(c => c.Status == KpiValueStatus.BelowTarget),
            ExceedsTarget = cards.Count(c => c.Status == KpiValueStatus.ExceedsTarget),
            OverallScore = cards.Any() ? cards.Average(c => c.PercentageOfTarget) : 0,
            TopKpis = cards.OrderByDescending(c => c.PercentageOfTarget).Take(3).ToList(),
            NeedsAttention = cards.Where(c => c.Status == KpiValueStatus.BelowTarget || c.Status == KpiValueStatus.AtRisk).ToList()
        };
    }

    public async Task<TeamKpiSummary> GetTeamSummaryAsync(
        Guid teamId,
        CancellationToken cancellationToken = default)
    {
        var team = _teams.FirstOrDefault(t => t.Id == teamId);
        var assignments = await GetTeamAssignmentsAsync(teamId, true, cancellationToken);
        var cards = new List<KpiDashboardCard>();

        foreach (var assignment in assignments)
        {
            var latestValue = await GetLatestValueAsync(assignment.KpiDefinitionId, null, teamId, cancellationToken);
            var kpi = await GetKpiAsync(assignment.KpiDefinitionId, cancellationToken);

            if (kpi != null)
            {
                cards.Add(CreateDashboardCard(kpi, latestValue, assignment.TargetValue));
            }
        }

        return new TeamKpiSummary
        {
            TeamId = teamId,
            TeamName = team?.Name ?? "Unknown Team",
            MemberCount = team?.MemberIds.Count ?? 0,
            TotalKpis = cards.Count,
            TeamOverallScore = cards.Any() ? cards.Average(c => c.PercentageOfTarget) : 0,
            TeamKpis = cards
        };
    }

    public async Task<List<KpiDashboardCard>> GetDashboardCardsAsync(
        Guid? userId = null,
        Guid? teamId = null,
        CancellationToken cancellationToken = default)
    {
        var cards = new List<KpiDashboardCard>();

        if (userId.HasValue)
        {
            var assignments = await GetUserAssignmentsAsync(userId.Value, true, cancellationToken);
            foreach (var assignment in assignments)
            {
                var latestValue = await GetLatestValueAsync(assignment.KpiDefinitionId, userId, null, cancellationToken);
                var kpi = await GetKpiAsync(assignment.KpiDefinitionId, cancellationToken);
                if (kpi != null)
                {
                    cards.Add(CreateDashboardCard(kpi, latestValue, assignment.TargetValue));
                }
            }
        }
        else if (teamId.HasValue)
        {
            var assignments = await GetTeamAssignmentsAsync(teamId.Value, true, cancellationToken);
            foreach (var assignment in assignments)
            {
                var latestValue = await GetLatestValueAsync(assignment.KpiDefinitionId, null, teamId, cancellationToken);
                var kpi = await GetKpiAsync(assignment.KpiDefinitionId, cancellationToken);
                if (kpi != null)
                {
                    cards.Add(CreateDashboardCard(kpi, latestValue, assignment.TargetValue));
                }
            }
        }

        return cards;
    }

    public Task<KpiReport> GenerateReportAsync(
        Guid? userId = null,
        Guid? teamId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var periodStart = fromDate ?? DateTime.UtcNow.AddMonths(-1);
        var periodEnd = toDate ?? DateTime.UtcNow;

        var report = new KpiReport
        {
            ReportTitle = userId.HasValue ? "Personal KPI Report" : "Team KPI Report",
            GeneratedAt = DateTime.UtcNow,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            Scope = userId.HasValue ? KpiScope.Personal : KpiScope.Team,
            TotalKpisTracked = 0,
            OverallPerformanceScore = 0
        };

        return Task.FromResult(report);
    }

    public Task<List<string>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var categories = _kpis
            .Where(k => k.IsActive)
            .Select(k => k.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToList();

        return Task.FromResult(categories);
    }

    public Task CalculateAutoKpisAsync(CancellationToken cancellationToken = default)
    {
        // In production, this would calculate auto-calculated KPIs based on formulas
        _logger.LogInformation("Auto-calculating KPIs");
        return Task.CompletedTask;
    }

    private static KpiDashboardCard CreateDashboardCard(KpiDefinition kpi, KpiValue? value, double target)
    {
        var currentValue = value?.Value ?? 0;
        var percentageOfTarget = target > 0 ? (currentValue / target) * 100 : 0;

        return new KpiDashboardCard
        {
            KpiId = kpi.Id,
            Name = kpi.Name,
            Category = kpi.Category,
            CurrentValue = currentValue,
            TargetValue = target,
            PercentageOfTarget = percentageOfTarget,
            Status = value?.Status ?? KpiValueStatus.BelowTarget,
            StatusColor = GetStatusColor(value?.Status ?? KpiValueStatus.BelowTarget),
            TrendPercentage = value?.PercentageChange,
            TrendDirection = kpi.DesiredTrend,
            Unit = kpi.Unit
        };
    }

    private static string GetStatusColor(KpiValueStatus status) => status switch
    {
        KpiValueStatus.ExceedsTarget => "#22c55e",
        KpiValueStatus.OnTrack => "#3b82f6",
        KpiValueStatus.AtRisk => "#f59e0b",
        KpiValueStatus.BelowTarget => "#ef4444",
        _ => "#6b7280"
    };

    private static KpiValueStatus CalculateStatus(double value, KpiDefinition kpi)
    {
        if (!kpi.TargetValue.HasValue)
            return KpiValueStatus.OnTrack;

        var target = kpi.TargetValue.Value;
        var percentage = target > 0 ? (value / target) * 100 : 0;

        if (kpi.DesiredTrend == TrendDirection.Higher)
        {
            return percentage switch
            {
                >= 100 => KpiValueStatus.ExceedsTarget,
                >= 80 => KpiValueStatus.OnTrack,
                >= 60 => KpiValueStatus.AtRisk,
                _ => KpiValueStatus.BelowTarget
            };
        }
        else if (kpi.DesiredTrend == TrendDirection.Lower)
        {
            return percentage switch
            {
                <= 100 => KpiValueStatus.ExceedsTarget,
                <= 120 => KpiValueStatus.OnTrack,
                <= 140 => KpiValueStatus.AtRisk,
                _ => KpiValueStatus.BelowTarget
            };
        }

        return KpiValueStatus.OnTrack;
    }

    private static DateTime GetPeriodStart(MeasurementFrequency frequency)
    {
        var now = DateTime.UtcNow;
        return frequency switch
        {
            MeasurementFrequency.Daily => now.Date,
            MeasurementFrequency.Weekly => now.AddDays(-(int)now.DayOfWeek),
            MeasurementFrequency.Monthly => new DateTime(now.Year, now.Month, 1),
            MeasurementFrequency.Quarterly => new DateTime(now.Year, ((now.Month - 1) / 3) * 3 + 1, 1),
            MeasurementFrequency.Yearly => new DateTime(now.Year, 1, 1),
            _ => now.Date
        };
    }

    private static List<KpiThreshold> GetDefaultThresholds() => new()
    {
        new() { Status = "Excellent", Color = "#22c55e", MinValue = 100, MaxValue = double.MaxValue },
        new() { Status = "Good", Color = "#3b82f6", MinValue = 80, MaxValue = 100 },
        new() { Status = "Warning", Color = "#f59e0b", MinValue = 60, MaxValue = 80 },
        new() { Status = "Critical", Color = "#ef4444", MinValue = 0, MaxValue = 60 }
    };

    private void SeedDemoKpis()
    {
        _kpis.AddRange(new[]
        {
            new KpiDefinition
            {
                Id = Guid.NewGuid(),
                Name = "Documents Published",
                Description = "Number of documents published this period",
                Category = "Productivity",
                Type = KpiType.Count,
                Scope = KpiScope.Personal,
                Unit = "documents",
                Frequency = MeasurementFrequency.Monthly,
                TargetValue = 10,
                DesiredTrend = TrendDirection.Higher,
                IsActive = true,
                DisplayOrder = 1,
                CreatedAt = DateTime.UtcNow,
                Thresholds = GetDefaultThresholds()
            },
            new KpiDefinition
            {
                Id = Guid.NewGuid(),
                Name = "Document Quality Score",
                Description = "Average quality score of published documents",
                Category = "Quality",
                Type = KpiType.Percentage,
                Scope = KpiScope.Personal,
                Unit = "%",
                Frequency = MeasurementFrequency.Monthly,
                TargetValue = 85,
                DesiredTrend = TrendDirection.Higher,
                IsActive = true,
                DisplayOrder = 2,
                CreatedAt = DateTime.UtcNow,
                Thresholds = GetDefaultThresholds()
            },
            new KpiDefinition
            {
                Id = Guid.NewGuid(),
                Name = "Knowledge Articles Created",
                Description = "Number of knowledge articles created",
                Category = "Knowledge Sharing",
                Type = KpiType.Count,
                Scope = KpiScope.Team,
                Unit = "articles",
                Frequency = MeasurementFrequency.Monthly,
                TargetValue = 20,
                DesiredTrend = TrendDirection.Higher,
                IsActive = true,
                DisplayOrder = 3,
                CreatedAt = DateTime.UtcNow,
                Thresholds = GetDefaultThresholds()
            },
            new KpiDefinition
            {
                Id = Guid.NewGuid(),
                Name = "Training Completion Rate",
                Description = "Percentage of assigned training completed",
                Category = "Learning",
                Type = KpiType.Percentage,
                Scope = KpiScope.Personal,
                Unit = "%",
                Frequency = MeasurementFrequency.Quarterly,
                TargetValue = 100,
                DesiredTrend = TrendDirection.Higher,
                IsActive = true,
                DisplayOrder = 4,
                CreatedAt = DateTime.UtcNow,
                Thresholds = GetDefaultThresholds()
            }
        });
    }
}
