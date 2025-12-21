using System;
using System.Collections.Generic;

namespace AFC27.KMS.WebApi.Features.Polling.Models;

/// <summary>
/// Poll entity
/// </summary>
public class Poll
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public PollType Type { get; set; }
    public PollStatus Status { get; set; } = PollStatus.Draft;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<PollOption> Options { get; set; } = new();
    public PollSettings Settings { get; set; } = new();
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? ModifiedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? Category { get; set; }
    public List<string> Tags { get; set; } = new();
    public Guid? RelatedDocumentId { get; set; }
    public Guid? RelatedMeetingId { get; set; }
}

public enum PollType
{
    SingleChoice,
    MultipleChoice,
    Rating,
    RankedChoice,
    YesNo,
    OpenEnded
}

public enum PollStatus
{
    Draft,
    Scheduled,
    Active,
    Closed,
    Cancelled,
    Archived
}

/// <summary>
/// Poll option/answer choice
/// </summary>
public class PollOption
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public string? ImageUrl { get; set; }
    public int VoteCount { get; set; }
    public double? RatingAverage { get; set; }
}

/// <summary>
/// Poll settings
/// </summary>
public class PollSettings
{
    public bool AllowAnonymousVoting { get; set; }
    public bool RequireAuthentication { get; set; } = true;
    public bool ShowResultsBeforeEnd { get; set; }
    public bool ShowVoterNames { get; set; }
    public bool AllowVoteChange { get; set; }
    public bool AllowComments { get; set; }
    public int? MaxSelectionsAllowed { get; set; }
    public int? MinSelectionsRequired { get; set; }
    public List<Guid>? RestrictToUserIds { get; set; }
    public List<string>? RestrictToRoles { get; set; }
    public List<Guid>? RestrictToTeamIds { get; set; }
    public bool NotifyOnVote { get; set; }
    public bool SendReminders { get; set; }
    public int? ReminderDaysBeforeEnd { get; set; }
}

/// <summary>
/// Vote record
/// </summary>
public class Vote
{
    public Guid Id { get; set; }
    public Guid PollId { get; set; }
    public Guid? UserId { get; set; }
    public string? AnonymousId { get; set; }
    public List<Guid> SelectedOptionIds { get; set; } = new();
    public Dictionary<Guid, int>? Rankings { get; set; }
    public int? RatingValue { get; set; }
    public string? OpenEndedResponse { get; set; }
    public string? Comment { get; set; }
    public DateTime VotedAt { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}

/// <summary>
/// Poll results summary
/// </summary>
public class PollResults
{
    public Guid PollId { get; set; }
    public string PollTitle { get; set; } = string.Empty;
    public PollType Type { get; set; }
    public int TotalVotes { get; set; }
    public int UniqueVoters { get; set; }
    public double ParticipationRate { get; set; }
    public List<OptionResult> OptionResults { get; set; } = new();
    public Guid? WinningOptionId { get; set; }
    public string? WinningOptionText { get; set; }
    public DateTime? LastVoteAt { get; set; }
    public List<VoteTimeline>? VoteTimeline { get; set; }
}

public class OptionResult
{
    public Guid OptionId { get; set; }
    public string OptionText { get; set; } = string.Empty;
    public int VoteCount { get; set; }
    public double Percentage { get; set; }
    public double? AverageRating { get; set; }
    public double? AverageRank { get; set; }
}

public class VoteTimeline
{
    public DateTime Date { get; set; }
    public int CumulativeVotes { get; set; }
}

/// <summary>
/// Request to create a poll
/// </summary>
public class CreatePollRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public PollType Type { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<CreatePollOptionRequest> Options { get; set; } = new();
    public PollSettings? Settings { get; set; }
    public string? Category { get; set; }
    public List<string>? Tags { get; set; }
    public Guid? RelatedDocumentId { get; set; }
    public Guid? RelatedMeetingId { get; set; }
}

public class CreatePollOptionRequest
{
    public string Text { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
}

/// <summary>
/// Request to cast a vote
/// </summary>
public class CastVoteRequest
{
    public Guid PollId { get; set; }
    public List<Guid>? SelectedOptionIds { get; set; }
    public Dictionary<Guid, int>? Rankings { get; set; }
    public int? RatingValue { get; set; }
    public string? OpenEndedResponse { get; set; }
    public string? Comment { get; set; }
}

/// <summary>
/// Poll summary for listings
/// </summary>
public class PollSummary
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public PollType Type { get; set; }
    public PollStatus Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int OptionCount { get; set; }
    public int VoteCount { get; set; }
    public bool HasVoted { get; set; }
    public string? Category { get; set; }
}
