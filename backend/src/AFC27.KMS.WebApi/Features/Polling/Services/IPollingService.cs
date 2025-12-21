using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AFC27.KMS.WebApi.Features.Polling.Models;

namespace AFC27.KMS.WebApi.Features.Polling.Services;

/// <summary>
/// Interface for polling and voting service
/// </summary>
public interface IPollingService
{
    // Poll Management
    Task<Poll> CreatePollAsync(CreatePollRequest request, Guid createdBy, CancellationToken cancellationToken = default);
    Task<Poll?> GetPollAsync(Guid pollId, CancellationToken cancellationToken = default);
    Task<Poll> UpdatePollAsync(Guid pollId, CreatePollRequest request, Guid modifiedBy, CancellationToken cancellationToken = default);
    Task DeletePollAsync(Guid pollId, CancellationToken cancellationToken = default);
    Task<Poll> PublishPollAsync(Guid pollId, Guid publishedBy, CancellationToken cancellationToken = default);
    Task<Poll> ClosePollAsync(Guid pollId, Guid closedBy, CancellationToken cancellationToken = default);

    // Poll Listings
    Task<(List<PollSummary> Polls, int TotalCount)> GetPollsAsync(
        PollStatus? status = null, string? category = null, Guid? userId = null,
        int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<List<PollSummary>> GetActivePollsAsync(Guid? userId = null, CancellationToken cancellationToken = default);
    Task<List<PollSummary>> GetMyPollsAsync(Guid userId, CancellationToken cancellationToken = default);

    // Voting
    Task<Vote> CastVoteAsync(CastVoteRequest request, Guid? userId, CancellationToken cancellationToken = default);
    Task<Vote?> GetUserVoteAsync(Guid pollId, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> HasUserVotedAsync(Guid pollId, Guid userId, CancellationToken cancellationToken = default);

    // Results
    Task<PollResults> GetResultsAsync(Guid pollId, CancellationToken cancellationToken = default);
    Task<List<Vote>> GetVotesAsync(Guid pollId, int page = 1, int pageSize = 50, CancellationToken cancellationToken = default);

    // Categories
    Task<List<string>> GetCategoriesAsync(CancellationToken cancellationToken = default);
}
