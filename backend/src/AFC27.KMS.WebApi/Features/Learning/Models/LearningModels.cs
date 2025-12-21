using System;
using System.Collections.Generic;

namespace AFC27.KMS.WebApi.Features.Learning.Models;

/// <summary>
/// Learning path entity
/// </summary>
public class LearningPath
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string Category { get; set; } = string.Empty;
    public DifficultyLevel Difficulty { get; set; }
    public int EstimatedHours { get; set; }
    public List<LearningPathItem> Items { get; set; } = new();
    public List<string> LearningObjectives { get; set; } = new();
    public List<string> Prerequisites { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public bool IsPublished { get; set; }
    public bool IsMandatory { get; set; }
    public DateTime? DueDate { get; set; }
    public Guid? CertificateTemplateId { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public int EnrollmentCount { get; set; }
    public double AverageRating { get; set; }
}

public enum DifficultyLevel { Beginner, Intermediate, Advanced, Expert }

/// <summary>
/// Learning path item (course, quiz, document)
/// </summary>
public class LearningPathItem
{
    public Guid Id { get; set; }
    public int Order { get; set; }
    public LearningItemType Type { get; set; }
    public Guid? CourseId { get; set; }
    public Guid? QuizId { get; set; }
    public Guid? DocumentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DurationMinutes { get; set; }
    public bool IsRequired { get; set; } = true;
    public double? PassingScore { get; set; }
}

public enum LearningItemType { Course, Quiz, Document, Video, ExternalLink }

/// <summary>
/// Course entity
/// </summary>
public class Course
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string Category { get; set; } = string.Empty;
    public DifficultyLevel Difficulty { get; set; }
    public int DurationMinutes { get; set; }
    public List<CourseModule> Modules { get; set; } = new();
    public string? InstructorName { get; set; }
    public bool IsPublished { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public int EnrollmentCount { get; set; }
    public double CompletionRate { get; set; }
    public double AverageRating { get; set; }
}

/// <summary>
/// Course module
/// </summary>
public class CourseModule
{
    public Guid Id { get; set; }
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<CourseLesson> Lessons { get; set; } = new();
    public Guid? QuizId { get; set; }
}

/// <summary>
/// Course lesson
/// </summary>
public class CourseLesson
{
    public Guid Id { get; set; }
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public LessonType Type { get; set; }
    public string? Content { get; set; }
    public string? VideoUrl { get; set; }
    public Guid? DocumentId { get; set; }
    public int DurationMinutes { get; set; }
}

public enum LessonType { Text, Video, Document, Interactive }

/// <summary>
/// Quiz entity
/// </summary>
public class Quiz
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public QuizType Type { get; set; }
    public int TimeLimitMinutes { get; set; }
    public double PassingScore { get; set; } = 70;
    public int MaxAttempts { get; set; } = 3;
    public bool ShuffleQuestions { get; set; }
    public bool ShuffleAnswers { get; set; }
    public bool ShowCorrectAnswers { get; set; }
    public List<QuizQuestion> Questions { get; set; } = new();
    public bool IsPublished { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

public enum QuizType { Practice, Assessment, Certification }

/// <summary>
/// Quiz question
/// </summary>
public class QuizQuestion
{
    public Guid Id { get; set; }
    public int Order { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public List<QuizAnswer> Answers { get; set; } = new();
    public string? Explanation { get; set; }
    public int Points { get; set; } = 1;
    public string? MediaUrl { get; set; }
}

public enum QuestionType { SingleChoice, MultipleChoice, TrueFalse, ShortAnswer, Essay }

/// <summary>
/// Quiz answer
/// </summary>
public class QuizAnswer
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int Order { get; set; }
}

/// <summary>
/// Quiz attempt
/// </summary>
public class QuizAttempt
{
    public Guid Id { get; set; }
    public Guid QuizId { get; set; }
    public Guid UserId { get; set; }
    public int AttemptNumber { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public double Score { get; set; }
    public bool Passed { get; set; }
    public List<QuestionResponse> Responses { get; set; } = new();
}

public class QuestionResponse
{
    public Guid QuestionId { get; set; }
    public List<Guid>? SelectedAnswerIds { get; set; }
    public string? TextResponse { get; set; }
    public bool IsCorrect { get; set; }
    public int PointsEarned { get; set; }
}

/// <summary>
/// Certificate entity
/// </summary>
public class Certificate
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public Guid LearningPathId { get; set; }
    public string LearningPathTitle { get; set; } = string.Empty;
    public string CertificateNumber { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? DownloadUrl { get; set; }
    public string? VerificationUrl { get; set; }
    public CertificateStatus Status { get; set; } = CertificateStatus.Active;
}

public enum CertificateStatus { Active, Expired, Revoked }

/// <summary>
/// User enrollment in learning path
/// </summary>
public class LearningEnrollment
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid LearningPathId { get; set; }
    public string LearningPathTitle { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public double ProgressPercentage { get; set; }
    public List<ItemProgress> ItemProgress { get; set; } = new();
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.InProgress;
    public Guid? CertificateId { get; set; }
}

public class ItemProgress
{
    public Guid ItemId { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public double? Score { get; set; }
    public int TimeSpentMinutes { get; set; }
}

public enum EnrollmentStatus { NotStarted, InProgress, Completed, Expired }

/// <summary>
/// Learning path summary for listings
/// </summary>
public class LearningPathSummary
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string Category { get; set; } = string.Empty;
    public DifficultyLevel Difficulty { get; set; }
    public int EstimatedHours { get; set; }
    public int ItemCount { get; set; }
    public int EnrollmentCount { get; set; }
    public double AverageRating { get; set; }
    public bool IsEnrolled { get; set; }
    public double? Progress { get; set; }
    public bool IsMandatory { get; set; }
}

/// <summary>
/// Request to create a learning path
/// </summary>
public class CreateLearningPathRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string Category { get; set; } = string.Empty;
    public DifficultyLevel Difficulty { get; set; }
    public List<string>? LearningObjectives { get; set; }
    public List<string>? Prerequisites { get; set; }
    public List<string>? Tags { get; set; }
    public bool IsMandatory { get; set; }
    public DateTime? DueDate { get; set; }
}

/// <summary>
/// User learning dashboard
/// </summary>
public class LearningDashboard
{
    public int EnrolledPaths { get; set; }
    public int CompletedPaths { get; set; }
    public int CertificatesEarned { get; set; }
    public int TotalLearningHours { get; set; }
    public double OverallProgress { get; set; }
    public List<LearningEnrollment> CurrentEnrollments { get; set; } = new();
    public List<Certificate> RecentCertificates { get; set; } = new();
    public List<LearningPathSummary> RecommendedPaths { get; set; } = new();
    public List<LearningPathSummary> MandatoryPaths { get; set; } = new();
}
