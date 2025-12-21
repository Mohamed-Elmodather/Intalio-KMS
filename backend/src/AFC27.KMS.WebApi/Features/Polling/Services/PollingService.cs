using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AFC27.KMS.WebApi.Features.Polling.Models;

namespace AFC27.KMS.WebApi.Features.Polling.Services;

/// <summary>
/// Implementation of polling and voting service
/// </summary>
public class PollingService : IPollingService
{
    private readonly ILogger<PollingService> _logger;
    private static readonly List<Poll> _polls = new();
    private static readonly List<Vote> _votes = new();

    public PollingService(ILogger<PollingService> logger)
    {
        _logger = logger;
        if (!_polls.Any()) SeedDemoPolls();
    }

    public Task<Poll> CreatePollAsync(CreatePollRequest request, Guid createdBy, CancellationToken cancellationToken = default)
    {
        var poll = new Poll
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Type = request.Type,
            Status = PollStatus.Draft,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Options = request.Options.Select((o, i) => new PollOption
            {
                Id = Guid.NewGuid(),
                Text = o.Text,
                Description = o.Description,
                ImageUrl = o.ImageUrl,
                DisplayOrder = i
            }).ToList(),
            Settings = request.Settings ?? new PollSettings(),
            Category = request.Category,
            Tags = request.Tags ?? new List<string>(),
            RelatedDocumentId = request.RelatedDocumentId,
            RelatedMeetingId = request.RelatedMeetingId,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        _polls.Add(poll);
        _logger.LogInformation("Created poll {PollId}: {Title}", poll.Id, poll.Title);
        return Task.FromResult(poll);
    }

    public Task<Poll?> GetPollAsync(Guid pollId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_polls.FirstOrDefault(p => p.Id == pollId));
    }

    public Task<Poll> UpdatePollAsync(Guid pollId, CreatePollRequest request, Guid modifiedBy, CancellationToken cancellationToken = default)
    {
        var poll = _polls.FirstOrDefault(p => p.Id == pollId) ?? throw new KeyNotFoundException($"Poll {pollId} not found");
        poll.Title = request.Title;
        poll.Description = request.Description;
        poll.Type = request.Type;
        poll.StartDate = request.StartDate;
        poll.EndDate = request.EndDate;
        poll.Category = request.Category;
        poll.Tags = request.Tags ?? poll.Tags;
        poll.Settings = request.Settings ?? poll.Settings;
        poll.ModifiedBy = modifiedBy;
        poll.ModifiedAt = DateTime.UtcNow;
        return Task.FromResult(poll);
    }

    public Task DeletePollAsync(Guid pollId, CancellationToken cancellationToken = default)
    {
        _polls.RemoveAll(p => p.Id == pollId);
        _votes.RemoveAll(v => v.PollId == pollId);
        return Task.CompletedTask;
    }

    public Task<Poll> PublishPollAsync(Guid pollId, Guid publishedBy, CancellationToken cancellationToken = default)
    {
        var poll = _polls.FirstOrDefault(p => p.Id == pollId) ?? throw new KeyNotFoundException($"Poll {pollId} not found");
        poll.Status = poll.StartDate <= DateTime.UtcNow ? PollStatus.Active : PollStatus.Scheduled;
        poll.ModifiedBy = publishedBy;
        poll.ModifiedAt = DateTime.UtcNow;
        return Task.FromResult(poll);
    }

    public Task<Poll> ClosePollAsync(Guid pollId, Guid closedBy, CancellationToken cancellationToken = default)
    {
        var poll = _polls.FirstOrDefault(p => p.Id == pollId) ?? throw new KeyNotFoundException($"Poll {pollId} not found");
        poll.Status = PollStatus.Closed;
        poll.EndDate = DateTime.UtcNow;
        poll.ModifiedBy = closedBy;
        poll.ModifiedAt = DateTime.UtcNow;
        return Task.FromResult(poll);
    }

    public Task<(List<PollSummary> Polls, int TotalCount)> GetPollsAsync(
        PollStatus? status = null, string? category = null, Guid? userId = null,
        int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var query = _polls.AsEnumerable();
        if (status.HasValue) query = query.Where(p => p.Status == status.Value);
        if (!string.IsNullOrEmpty(category)) query = query.Where(p => p.Category == category);

        var totalCount = query.Count();
        var polls = query.Skip((page - 1) * pageSize).Take(pageSize)
            .Select(p => ToSummary(p, userId ?? Guid.Empty)).ToList();

        return Task.FromResult((polls, totalCount));
    }

    public Task<List<PollSummary>> GetActivePollsAsync(Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var polls = _polls.Where(p => p.Status == PollStatus.Active)
            .Select(p => ToSummary(p, userId ?? Guid.Empty)).ToList();
        return Task.FromResult(polls);
    }

    public Task<List<PollSummary>> GetMyPollsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var polls = _polls.Where(p => p.CreatedBy == userId)
            .Select(p => ToSummary(p, userId)).ToList();
        return Task.FromResult(polls);
    }

    public Task<Vote> CastVoteAsync(CastVoteRequest request, Guid? userId, CancellationToken cancellationToken = default)
    {
        var poll = _polls.FirstOrDefault(p => p.Id == request.PollId) ?? throw new KeyNotFoundException($"Poll {request.PollId} not found");
        if (poll.Status != PollStatus.Active) throw new InvalidOperationException("Poll is not active");

        // Check if user already voted
        if (userId.HasValue && !poll.Settings.AllowVoteChange)
        {
            var existingVote = _votes.FirstOrDefault(v => v.PollId == request.PollId && v.UserId == userId);
            if (existingVote != null) throw new InvalidOperationException("User has already voted");
        }

        var vote = new Vote
        {
            Id = Guid.NewGuid(),
            PollId = request.PollId,
            UserId = userId,
            SelectedOptionIds = request.SelectedOptionIds ?? new List<Guid>(),
            Rankings = request.Rankings,
            RatingValue = request.RatingValue,
            OpenEndedResponse = request.OpenEndedResponse,
            Comment = request.Comment,
            VotedAt = DateTime.UtcNow
        };

        _votes.Add(vote);

        // Update vote counts
        foreach (var optionId in vote.SelectedOptionIds)
        {
            var option = poll.Options.FirstOrDefault(o => o.Id == optionId);
            if (option != null) option.VoteCount++;
        }

        _logger.LogInformation("Vote cast on poll {PollId}", request.PollId);
        return Task.FromResult(vote);
    }

    public Task<Vote?> GetUserVoteAsync(Guid pollId, Guid userId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_votes.FirstOrDefault(v => v.PollId == pollId && v.UserId == userId));
    }

    public Task<bool> HasUserVotedAsync(Guid pollId, Guid userId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_votes.Any(v => v.PollId == pollId && v.UserId == userId));
    }

    public Task<PollResults> GetResultsAsync(Guid pollId, CancellationToken cancellationToken = default)
    {
        var poll = _polls.FirstOrDefault(p => p.Id == pollId) ?? throw new KeyNotFoundException($"Poll {pollId} not found");
        var votes = _votes.Where(v => v.PollId == pollId).ToList();
        var totalVotes = votes.Sum(v => v.SelectedOptionIds.Count);

        var results = new PollResults
        {
            PollId = pollId,
            PollTitle = poll.Title,
            Type = poll.Type,
            TotalVotes = totalVotes,
            UniqueVoters = votes.Select(v => v.UserId ?? Guid.Empty).Distinct().Count(),
            OptionResults = poll.Options.Select(o => new OptionResult
            {
                OptionId = o.Id,
                OptionText = o.Text,
                VoteCount = o.VoteCount,
                Percentage = totalVotes > 0 ? (double)o.VoteCount / totalVotes * 100 : 0
            }).OrderByDescending(r => r.VoteCount).ToList(),
            LastVoteAt = votes.MaxBy(v => v.VotedAt)?.VotedAt
        };

        var winner = results.OptionResults.FirstOrDefault();
        if (winner != null)
        {
            results.WinningOptionId = winner.OptionId;
            results.WinningOptionText = winner.OptionText;
        }

        return Task.FromResult(results);
    }

    public Task<List<Vote>> GetVotesAsync(Guid pollId, int page = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var votes = _votes.Where(v => v.PollId == pollId)
            .OrderByDescending(v => v.VotedAt)
            .Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult(votes);
    }

    public Task<List<string>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var categories = _polls.Where(p => !string.IsNullOrEmpty(p.Category))
            .Select(p => p.Category!).Distinct().OrderBy(c => c).ToList();
        return Task.FromResult(categories);
    }

    private PollSummary ToSummary(Poll poll, Guid userId) => new()
    {
        Id = poll.Id,
        Title = poll.Title,
        Type = poll.Type,
        Status = poll.Status,
        StartDate = poll.StartDate,
        EndDate = poll.EndDate,
        OptionCount = poll.Options.Count,
        VoteCount = poll.Options.Sum(o => o.VoteCount),
        HasVoted = _votes.Any(v => v.PollId == poll.Id && v.UserId == userId),
        Category = poll.Category
    };

    private void SeedDemoPolls()
    {
        _polls.Add(new Poll
        {
            Id = Guid.NewGuid(),
            Title = "Tournament Mascot Design Selection",
            Description = "Vote for your favorite mascot design for AFC Asian Cup 2027",
            Type = PollType.SingleChoice,
            Status = PollStatus.Active,
            StartDate = DateTime.UtcNow.AddDays(-5),
            EndDate = DateTime.UtcNow.AddDays(10),
            Options = new List<PollOption>
            {
                new() { Id = Guid.NewGuid(), Text = "Design A - Arabian Oryx", DisplayOrder = 0, VoteCount = 45 },
                new() { Id = Guid.NewGuid(), Text = "Design B - Falcon", DisplayOrder = 1, VoteCount = 38 },
                new() { Id = Guid.NewGuid(), Text = "Design C - Desert Fox", DisplayOrder = 2, VoteCount = 22 }
            },
            Category = "Tournament Planning",
            CreatedBy = Guid.Empty,
            CreatedAt = DateTime.UtcNow.AddDays(-7)
        });
    }
}
