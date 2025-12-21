using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AFC27.KMS.WebApi.Features.KpiManagement.Models;

namespace AFC27.KMS.WebApi.Features.KpiManagement.Services;

/// <summary>
/// Interface for KPI management service
/// </summary>
public interface IKpiService
{
    // KPI Definitions
    Task<KpiDefinition> CreateKpiAsync(
        CreateKpiRequest request,
        Guid createdBy,
        CancellationToken cancellationToken = default);

    Task<KpiDefinition?> GetKpiAsync(
        Guid kpiId,
        CancellationToken cancellationToken = default);

    Task<List<KpiDefinition>> GetKpisAsync(
        string? category = null,
        KpiScope? scope = null,
        bool activeOnly = true,
        CancellationToken cancellationToken = default);

    Task<KpiDefinition> UpdateKpiAsync(
        Guid kpiId,
        CreateKpiRequest request,
        CancellationToken cancellationToken = default);

    Task DeactivateKpiAsync(
        Guid kpiId,
        CancellationToken cancellationToken = default);

    // KPI Values
    Task<KpiValue> RecordValueAsync(
        RecordKpiValueRequest request,
        Guid recordedBy,
        CancellationToken cancellationToken = default);

    Task<List<KpiValue>> GetValuesAsync(
        Guid kpiId,
        Guid? userId = null,
        Guid? teamId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default);

    Task<KpiValue?> GetLatestValueAsync(
        Guid kpiId,
        Guid? userId = null,
        Guid? teamId = null,
        CancellationToken cancellationToken = default);

    // Assignments
    Task<KpiAssignment> AssignKpiAsync(
        AssignKpiRequest request,
        Guid assignedBy,
        CancellationToken cancellationToken = default);

    Task<List<KpiAssignment>> GetUserAssignmentsAsync(
        Guid userId,
        bool activeOnly = true,
        CancellationToken cancellationToken = default);

    Task<List<KpiAssignment>> GetTeamAssignmentsAsync(
        Guid teamId,
        bool activeOnly = true,
        CancellationToken cancellationToken = default);

    // Dashboard & Summary
    Task<PersonalKpiSummary> GetPersonalSummaryAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<TeamKpiSummary> GetTeamSummaryAsync(
        Guid teamId,
        CancellationToken cancellationToken = default);

    Task<List<KpiDashboardCard>> GetDashboardCardsAsync(
        Guid? userId = null,
        Guid? teamId = null,
        CancellationToken cancellationToken = default);

    // Reporting
    Task<KpiReport> GenerateReportAsync(
        Guid? userId = null,
        Guid? teamId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default);

    // Categories
    Task<List<string>> GetCategoriesAsync(
        CancellationToken cancellationToken = default);

    // Auto-calculation
    Task CalculateAutoKpisAsync(
        CancellationToken cancellationToken = default);
}
