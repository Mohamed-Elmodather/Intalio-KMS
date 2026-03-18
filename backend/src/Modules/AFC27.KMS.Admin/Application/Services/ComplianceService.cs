using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Admin.Application.DTOs;
using AFC27.KMS.Admin.Application.Interfaces;
using AFC27.KMS.Admin.Domain.Entities;

namespace AFC27.KMS.Admin.Application.Services;

/// <summary>
/// Service interface for the compliance dashboard.
/// </summary>
public interface IComplianceService
{
    Task<ComplianceDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default);
    Task<PolicyComplianceDto> GetPolicyComplianceAsync(CancellationToken cancellationToken = default);
    Task<ContentReviewStatusDto> GetContentReviewStatusAsync(CancellationToken cancellationToken = default);
    Task<AccessAuditSummaryDto> GetAccessAuditSummaryAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<DataRetentionStatusDto> GetDataRetentionStatusAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Service providing compliance dashboard data by aggregating audit logs, policies, and content state.
/// </summary>
public class ComplianceService : IComplianceService
{
    private readonly DbContext _dbContext;
    private readonly ILogger<ComplianceService> _logger;

    public ComplianceService(
        DbContext dbContext,
        ILogger<ComplianceService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<ComplianceDashboardDto> GetDashboardAsync(
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating compliance dashboard");

        var now = DateTime.UtcNow;
        var thirtyDaysAgo = now.AddDays(-30);

        var policyCompliance = await GetPolicyComplianceAsync(cancellationToken);
        var contentReview = await GetContentReviewStatusAsync(cancellationToken);
        var accessAudit = await GetAccessAuditSummaryAsync(thirtyDaysAgo, now, cancellationToken);
        var dataRetention = await GetDataRetentionStatusAsync(cancellationToken);

        // Calculate overall score
        var areaScores = new List<ComplianceAreaScoreDto>
        {
            new ComplianceAreaScoreDto
            {
                Area = "Policy Compliance",
                Score = policyCompliance.AcknowledgmentRate,
                TotalChecks = policyCompliance.TotalPolicies,
                PassedChecks = policyCompliance.ActivePolicies,
                FailedChecks = policyCompliance.ExpiredPolicies
            },
            new ComplianceAreaScoreDto
            {
                Area = "Content Review",
                Score = contentReview.ReviewCompletionRate,
                TotalChecks = contentReview.TotalContentItems,
                PassedChecks = contentReview.ApprovedContent,
                FailedChecks = contentReview.RejectedContent + contentReview.QuarantinedContent
            },
            new ComplianceAreaScoreDto
            {
                Area = "Access Control",
                Score = accessAudit.TotalAccessEvents > 0
                    ? 100.0 - ((double)accessAudit.UnauthorizedAttempts / accessAudit.TotalAccessEvents * 100)
                    : 100.0,
                TotalChecks = accessAudit.TotalAccessEvents,
                PassedChecks = accessAudit.TotalAccessEvents - accessAudit.UnauthorizedAttempts,
                FailedChecks = accessAudit.UnauthorizedAttempts
            },
            new ComplianceAreaScoreDto
            {
                Area = "Data Retention",
                Score = dataRetention.ActiveRetentionPolicies > 0 ? 85.0 : 0,
                TotalChecks = dataRetention.TotalRetentionPolicies,
                PassedChecks = dataRetention.ActiveRetentionPolicies,
                FailedChecks = dataRetention.TotalRetentionPolicies - dataRetention.ActiveRetentionPolicies
            }
        };

        var overallScore = areaScores.Any() ? areaScores.Average(a => a.Score) : 0;

        // Gather recent alerts from critical audit events
        var recentAlerts = await _dbContext.Set<AuditLogEntry>().AsNoTracking()
            .Where(l => l.Timestamp >= thirtyDaysAgo && l.Severity >= AuditSeverity.Warning && !l.IsSuccess)
            .OrderByDescending(l => l.Timestamp)
            .Take(10)
            .Select(l => new ComplianceAlertDto
            {
                Id = l.Id,
                Title = $"{l.Action} failure",
                Description = l.ErrorMessage ?? $"Failed {l.Action} on {l.EntityType}",
                Severity = l.Severity >= AuditSeverity.Critical ? "critical" : "warning",
                Category = l.Category,
                CreatedAt = l.Timestamp,
                IsResolved = false
            })
            .ToListAsync(cancellationToken);

        return new ComplianceDashboardDto
        {
            OverallScore = new ComplianceScoreDto
            {
                Score = overallScore,
                Grade = GetGrade(overallScore),
                CalculatedAt = DateTime.UtcNow,
                AreaScores = areaScores
            },
            PolicyCompliance = policyCompliance,
            ContentReviewStatus = contentReview,
            AccessAuditSummary = accessAudit,
            DataRetentionStatus = dataRetention,
            RecentAlerts = recentAlerts
        };
    }

    public async Task<PolicyComplianceDto> GetPolicyComplianceAsync(
        CancellationToken cancellationToken = default)
    {
        var policies = await _dbContext.Set<KMPolicy>().AsNoTracking()
            .Include(p => p.Acknowledgments)
            .ToListAsync(cancellationToken);

        var totalPolicies = policies.Count;
        var activePolicies = policies.Count(p => p.Status == KMPolicyStatus.Active);
        var expiredPolicies = policies.Count(p => p.IsExpired || p.Status == KMPolicyStatus.Expired);
        var requiresAck = policies.Where(p => p.RequiresAcknowledgment && p.Status == KMPolicyStatus.Active).ToList();
        var totalAcknowledgments = requiresAck.Sum(p => p.Acknowledgments.Count);
        // Simplified: assume 100 users total for rate calculation
        var ackRate = requiresAck.Count > 0 ? (double)totalAcknowledgments / (requiresAck.Count * 100) * 100 : 100;

        var policyItems = policies.Select(p => new PolicyComplianceItemDto
        {
            PolicyId = p.Id,
            Title = p.Title?.English ?? "Untitled",
            Status = p.Status.ToString(),
            TotalUsers = 100, // Placeholder
            AcknowledgedUsers = p.Acknowledgments.Count,
            ComplianceRate = p.RequiresAcknowledgment ? (double)p.Acknowledgments.Count / 100 * 100 : 100,
            ExpiryDate = p.ExpiryDate
        }).ToList();

        return new PolicyComplianceDto
        {
            TotalPolicies = totalPolicies,
            ActivePolicies = activePolicies,
            ExpiredPolicies = expiredPolicies,
            PoliciesRequiringAcknowledgment = requiresAck.Count,
            AcknowledgmentRate = Math.Min(100, ackRate),
            Policies = policyItems
        };
    }

    public async Task<ContentReviewStatusDto> GetContentReviewStatusAsync(
        CancellationToken cancellationToken = default)
    {
        var quarantined = await _dbContext.Set<QuarantinedDocument>().AsNoTracking()
            .ToListAsync(cancellationToken);

        var legalHoldDocs = await _dbContext.Set<LegalHoldDocument>().AsNoTracking()
            .CountAsync(cancellationToken);

        return new ContentReviewStatusDto
        {
            TotalContentItems = quarantined.Count + legalHoldDocs,
            PendingReview = quarantined.Count(q => q.Status == QuarantineStatus.Pending),
            ApprovedContent = quarantined.Count(q => q.Status == QuarantineStatus.Approved),
            RejectedContent = quarantined.Count(q => q.Status == QuarantineStatus.Rejected),
            QuarantinedContent = quarantined.Count(q => q.Status == QuarantineStatus.Pending),
            ContentUnderLegalHold = legalHoldDocs,
            ReviewCompletionRate = quarantined.Count > 0
                ? (double)(quarantined.Count - quarantined.Count(q => q.Status == QuarantineStatus.Pending)) / quarantined.Count * 100
                : 100,
            PendingItems = quarantined
                .Where(q => q.Status == QuarantineStatus.Pending)
                .Select(q => new ContentReviewItemDto
                {
                    ContentId = q.DocumentId,
                    Title = q.DocumentName,
                    ContentType = "Document",
                    Status = "Pending Review",
                    SubmittedAt = q.QuarantinedAt,
                    SubmittedBy = q.QuarantinedByName,
                    DaysPending = (int)(DateTime.UtcNow - q.QuarantinedAt).TotalDays
                })
                .OrderByDescending(i => i.DaysPending)
                .Take(10)
                .ToList()
        };
    }

    public async Task<AccessAuditSummaryDto> GetAccessAuditSummaryAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        var logs = _dbContext.Set<AuditLogEntry>().AsNoTracking()
            .Where(l => l.Timestamp >= from && l.Timestamp <= to);

        var totalEvents = await logs.CountAsync(
            l => l.Category == AuditCategories.Authentication || l.Category == AuditCategories.Authorization,
            cancellationToken);

        var unauthorizedAttempts = await logs.CountAsync(
            l => l.Category == AuditCategories.Authorization && !l.IsSuccess,
            cancellationToken);

        var suspiciousActivities = await logs.CountAsync(
            l => l.Severity >= AuditSeverity.Warning &&
                 (l.Category == AuditCategories.SecurityEvent || l.Category == AuditCategories.Authentication),
            cancellationToken);

        var privilegedEvents = await logs.CountAsync(
            l => l.Action == AuditActions.ImpersonationStarted ||
                 l.Action == AuditActions.RoleAssigned ||
                 l.Action == AuditActions.UserSuspended,
            cancellationToken);

        var recentViolations = await logs
            .Where(l => l.Category == AuditCategories.Authorization && !l.IsSuccess)
            .OrderByDescending(l => l.Timestamp)
            .Take(10)
            .Select(l => new AccessViolationDto
            {
                Timestamp = l.Timestamp,
                UserId = l.UserId,
                UserName = l.UserName,
                Action = l.Action,
                Resource = l.EntityType,
                IpAddress = l.IpAddress,
                Reason = l.ErrorMessage ?? "Unauthorized"
            })
            .ToListAsync(cancellationToken);

        return new AccessAuditSummaryDto
        {
            TotalAccessEvents = totalEvents,
            UnauthorizedAttempts = unauthorizedAttempts,
            SuspiciousActivities = suspiciousActivities,
            PrivilegedAccessEvents = privilegedEvents,
            RecentViolations = recentViolations,
            RecentPrivilegedAccess = Array.Empty<PrivilegedAccessDto>()
        };
    }

    public Task<DataRetentionStatusDto> GetDataRetentionStatusAsync(
        CancellationToken cancellationToken = default)
    {
        // Data retention policies are configuration-driven; return defaults
        return Task.FromResult(new DataRetentionStatusDto
        {
            TotalRetentionPolicies = 3,
            ActiveRetentionPolicies = 3,
            ItemsPendingDeletion = 0,
            ItemsDeletedThisPeriod = 0,
            StorageRecoveredBytes = 0,
            Policies = new List<RetentionPolicyStatusDto>
            {
                new RetentionPolicyStatusDto
                {
                    PolicyName = "Audit Log Retention",
                    RetentionDays = 365,
                    AffectedItems = 0,
                    NextRunAt = DateTime.UtcNow.Date.AddDays(1),
                    LastRunAt = DateTime.UtcNow.Date.AddDays(-1)
                },
                new RetentionPolicyStatusDto
                {
                    PolicyName = "Deleted Content Purge",
                    RetentionDays = 90,
                    AffectedItems = 0,
                    NextRunAt = DateTime.UtcNow.Date.AddDays(1),
                    LastRunAt = DateTime.UtcNow.Date.AddDays(-1)
                },
                new RetentionPolicyStatusDto
                {
                    PolicyName = "Temporary File Cleanup",
                    RetentionDays = 7,
                    AffectedItems = 0,
                    NextRunAt = DateTime.UtcNow.Date.AddDays(1),
                    LastRunAt = DateTime.UtcNow.Date.AddDays(-1)
                }
            }
        });
    }

    private static string GetGrade(double score)
    {
        return score switch
        {
            >= 90 => "A",
            >= 80 => "B",
            >= 70 => "C",
            >= 60 => "D",
            _ => "F"
        };
    }
}
