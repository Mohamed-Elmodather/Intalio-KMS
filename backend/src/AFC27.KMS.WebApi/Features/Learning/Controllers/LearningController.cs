using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.WebApi.Features.Learning.Models;
using AFC27.KMS.WebApi.Features.Learning.Services;
using AFC27.KMS.SharedKernel.Interfaces;

namespace AFC27.KMS.WebApi.Features.Learning.Controllers;

/// <summary>
/// Controller for learning management operations
/// </summary>
[ApiController]
[Route("api/learning")]
[Authorize]
public class LearningController : ControllerBase
{
    private readonly ILearningService _learningService;
    private readonly ICurrentUser _currentUser;

    public LearningController(ILearningService learningService, ICurrentUser currentUser)
    {
        _learningService = learningService;
        _currentUser = currentUser;
    }

    // Learning Paths
    [HttpGet("paths")]
    public async Task<ActionResult<object>> GetPaths([FromQuery] string? category, [FromQuery] DifficultyLevel? difficulty, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var (paths, total) = await _learningService.GetLearningPathsAsync(category, difficulty, _currentUser.UserId, page, pageSize, cancellationToken);
        return Ok(new { data = paths, pagination = new { page, pageSize, totalCount = total } });
    }

    [HttpGet("paths/{id:guid}")]
    public async Task<ActionResult<LearningPath>> GetPath(Guid id, CancellationToken cancellationToken)
    {
        var path = await _learningService.GetLearningPathAsync(id, cancellationToken);
        return path == null ? NotFound() : Ok(path);
    }

    [HttpPost("paths")]
    [Authorize(Policy = "CanManageContent")]
    public async Task<ActionResult<LearningPath>> CreatePath([FromBody] CreateLearningPathRequest request, CancellationToken cancellationToken)
    {
        var path = await _learningService.CreateLearningPathAsync(request, _currentUser.UserId ?? Guid.Empty, cancellationToken);
        return CreatedAtAction(nameof(GetPath), new { id = path.Id }, path);
    }

    [HttpPost("paths/{id:guid}/publish")]
    [Authorize(Policy = "CanPublishContent")]
    public async Task<ActionResult<LearningPath>> PublishPath(Guid id, CancellationToken cancellationToken)
    {
        try { return Ok(await _learningService.PublishLearningPathAsync(id, cancellationToken)); }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    // Enrollment
    [HttpPost("paths/{id:guid}/enroll")]
    public async Task<ActionResult<LearningEnrollment>> Enroll(Guid id, CancellationToken cancellationToken)
    {
        try { return Ok(await _learningService.EnrollAsync(id, _currentUser.UserId ?? Guid.Empty, cancellationToken)); }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpGet("my-enrollments")]
    public async Task<ActionResult<List<LearningEnrollment>>> GetMyEnrollments([FromQuery] EnrollmentStatus? status, CancellationToken cancellationToken) =>
        Ok(await _learningService.GetUserEnrollmentsAsync(_currentUser.UserId ?? Guid.Empty, status, cancellationToken));

    [HttpPost("enrollments/{id:guid}/progress")]
    public async Task<ActionResult<LearningEnrollment>> UpdateProgress(Guid id, [FromBody] UpdateProgressRequest request, CancellationToken cancellationToken)
    {
        try { return Ok(await _learningService.UpdateProgressAsync(id, request.ItemId, request.Completed, request.Score, cancellationToken)); }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    // Quizzes
    [HttpGet("quizzes/{id:guid}")]
    public async Task<ActionResult<Quiz>> GetQuiz(Guid id, CancellationToken cancellationToken)
    {
        var quiz = await _learningService.GetQuizAsync(id, cancellationToken);
        return quiz == null ? NotFound() : Ok(quiz);
    }

    [HttpPost("quizzes/{id:guid}/start")]
    public async Task<ActionResult<QuizAttempt>> StartQuiz(Guid id, CancellationToken cancellationToken)
    {
        try { return Ok(await _learningService.StartQuizAttemptAsync(id, _currentUser.UserId ?? Guid.Empty, cancellationToken)); }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPost("attempts/{id:guid}/submit")]
    public async Task<ActionResult<QuizAttempt>> SubmitQuiz(Guid id, [FromBody] List<QuestionResponse> responses, CancellationToken cancellationToken)
    {
        try { return Ok(await _learningService.SubmitQuizAttemptAsync(id, responses, cancellationToken)); }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    // Certificates
    [HttpGet("my-certificates")]
    public async Task<ActionResult<List<Certificate>>> GetMyCertificates(CancellationToken cancellationToken) =>
        Ok(await _learningService.GetUserCertificatesAsync(_currentUser.UserId ?? Guid.Empty, cancellationToken));

    [HttpGet("certificates/{id:guid}")]
    public async Task<ActionResult<Certificate>> GetCertificate(Guid id, CancellationToken cancellationToken)
    {
        var cert = await _learningService.GetCertificateAsync(id, cancellationToken);
        return cert == null ? NotFound() : Ok(cert);
    }

    [HttpGet("certificates/verify/{number}")]
    [AllowAnonymous]
    public async Task<ActionResult<object>> VerifyCertificate(string number, CancellationToken cancellationToken)
    {
        var isValid = await _learningService.VerifyCertificateAsync(number, cancellationToken);
        return Ok(new { certificateNumber = number, isValid });
    }

    // Dashboard
    [HttpGet("dashboard")]
    public async Task<ActionResult<LearningDashboard>> GetDashboard(CancellationToken cancellationToken) =>
        Ok(await _learningService.GetUserDashboardAsync(_currentUser.UserId ?? Guid.Empty, cancellationToken));

    [HttpGet("categories")]
    public async Task<ActionResult<List<string>>> GetCategories(CancellationToken cancellationToken) =>
        Ok(await _learningService.GetCategoriesAsync(cancellationToken));
}

public class UpdateProgressRequest
{
    public Guid ItemId { get; set; }
    public bool Completed { get; set; }
    public double? Score { get; set; }
}
