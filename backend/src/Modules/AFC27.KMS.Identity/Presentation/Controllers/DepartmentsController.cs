using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Identity.Application.DTOs;
using AFC27.KMS.Contracts.Common;

namespace AFC27.KMS.Identity.Presentation.Controllers;

/// <summary>
/// Department management controller.
/// </summary>
[ApiController]
[Route("api/identity/departments")]
[Authorize]
public class DepartmentsController : ControllerBase
{
    private readonly ILogger<DepartmentsController> _logger;

    public DepartmentsController(ILogger<DepartmentsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all departments as a tree structure.
    /// </summary>
    [HttpGet]
    public ActionResult<ApiResponse<IReadOnlyList<DepartmentDto>>> GetDepartments()
    {
        var departments = new List<DepartmentDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Executive Office",
                NameArabic = "المكتب التنفيذي",
                Description = "Executive leadership and management",
                DescriptionArabic = "القيادة التنفيذية والإدارة",
                SortOrder = 1,
                MemberCount = 10,
                Children = new List<DepartmentSummaryDto>
                {
                    new() { Id = Guid.NewGuid(), Name = "CEO Office", NameArabic = "مكتب الرئيس التنفيذي", MemberCount = 5 },
                    new() { Id = Guid.NewGuid(), Name = "Legal Affairs", NameArabic = "الشؤون القانونية", MemberCount = 8 }
                },
                CreatedAt = DateTime.UtcNow.AddYears(-1)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Operations",
                NameArabic = "العمليات",
                Description = "Event operations and logistics",
                DescriptionArabic = "عمليات الفعاليات واللوجستيات",
                SortOrder = 2,
                MemberCount = 150,
                Children = new List<DepartmentSummaryDto>
                {
                    new() { Id = Guid.NewGuid(), Name = "Venue Management", NameArabic = "إدارة الملاعب", MemberCount = 45 },
                    new() { Id = Guid.NewGuid(), Name = "Logistics", NameArabic = "اللوجستيات", MemberCount = 60 },
                    new() { Id = Guid.NewGuid(), Name = "Security", NameArabic = "الأمن", MemberCount = 45 }
                },
                CreatedAt = DateTime.UtcNow.AddYears(-1)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Technology",
                NameArabic = "التقنية",
                Description = "IT and technology services",
                DescriptionArabic = "خدمات تقنية المعلومات",
                SortOrder = 3,
                MemberCount = 80,
                Children = new List<DepartmentSummaryDto>
                {
                    new() { Id = Guid.NewGuid(), Name = "Software Development", NameArabic = "تطوير البرمجيات", MemberCount = 30 },
                    new() { Id = Guid.NewGuid(), Name = "Infrastructure", NameArabic = "البنية التحتية", MemberCount = 25 },
                    new() { Id = Guid.NewGuid(), Name = "Support", NameArabic = "الدعم الفني", MemberCount = 25 }
                },
                CreatedAt = DateTime.UtcNow.AddYears(-1)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Marketing & Communications",
                NameArabic = "التسويق والاتصالات",
                Description = "Marketing, PR, and communications",
                DescriptionArabic = "التسويق والعلاقات العامة والاتصالات",
                SortOrder = 4,
                MemberCount = 50,
                CreatedAt = DateTime.UtcNow.AddYears(-1)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Human Resources",
                NameArabic = "الموارد البشرية",
                Description = "HR and talent management",
                DescriptionArabic = "إدارة الموارد البشرية والمواهب",
                SortOrder = 5,
                MemberCount = 25,
                CreatedAt = DateTime.UtcNow.AddYears(-1)
            }
        };

        return Ok(ApiResponse<IReadOnlyList<DepartmentDto>>.Ok(departments));
    }

    /// <summary>
    /// Get department by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public ActionResult<ApiResponse<DepartmentDto>> GetDepartment(Guid id)
    {
        var department = new DepartmentDto
        {
            Id = id,
            Name = "Technology",
            NameArabic = "التقنية",
            Description = "IT and technology services",
            DescriptionArabic = "خدمات تقنية المعلومات",
            SortOrder = 3,
            ManagerId = Guid.NewGuid(),
            ManagerName = "Mohammed Al-Rashid",
            MemberCount = 80,
            Children = new List<DepartmentSummaryDto>
            {
                new() { Id = Guid.NewGuid(), Name = "Software Development", NameArabic = "تطوير البرمجيات", MemberCount = 30 },
                new() { Id = Guid.NewGuid(), Name = "Infrastructure", NameArabic = "البنية التحتية", MemberCount = 25 }
            },
            CreatedAt = DateTime.UtcNow.AddYears(-1)
        };

        return Ok(ApiResponse<DepartmentDto>.Ok(department));
    }

    /// <summary>
    /// Create a new department.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "CanManageUsers")]
    public async Task<ActionResult<ApiResponse<DepartmentDto>>> CreateDepartment([FromBody] CreateDepartmentRequest request)
    {
        _logger.LogInformation("Creating department {DepartmentName}", request.Name);

        await Task.Delay(100);

        var department = new DepartmentDto
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            NameArabic = request.NameArabic,
            Description = request.Description,
            DescriptionArabic = request.DescriptionArabic,
            ParentId = request.ParentId,
            ManagerId = request.ManagerId,
            MemberCount = 0,
            CreatedAt = DateTime.UtcNow
        };

        return CreatedAtAction(nameof(GetDepartment), new { id = department.Id }, ApiResponse<DepartmentDto>.Ok(department));
    }

    /// <summary>
    /// Update a department.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "CanManageUsers")]
    public async Task<ActionResult<ApiResponse>> UpdateDepartment(Guid id, [FromBody] UpdateDepartmentRequest request)
    {
        _logger.LogInformation("Updating department {DepartmentId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Department updated successfully"));
    }

    /// <summary>
    /// Delete a department.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "CanManageUsers")]
    public async Task<ActionResult<ApiResponse>> DeleteDepartment(Guid id)
    {
        _logger.LogInformation("Deleting department {DepartmentId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Department deleted successfully"));
    }

    /// <summary>
    /// Get department members.
    /// </summary>
    [HttpGet("{id:guid}/members")]
    public ActionResult<ApiResponse<PagedResult<UserSummaryDto>>> GetDepartmentMembers(
        Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var members = new List<UserSummaryDto>
        {
            new() { Id = Guid.NewGuid(), DisplayName = "Ahmed Hassan", Email = "ahmed.hassan@afc27.com", JobTitle = "Senior Developer" },
            new() { Id = Guid.NewGuid(), DisplayName = "Sara Ali", Email = "sara.ali@afc27.com", JobTitle = "Developer" },
            new() { Id = Guid.NewGuid(), DisplayName = "Mohammed Omar", Email = "mohammed.omar@afc27.com", JobTitle = "Tech Lead" }
        };

        var result = PagedResult<UserSummaryDto>.Create(members, 25, page, pageSize);
        return Ok(ApiResponse<PagedResult<UserSummaryDto>>.Ok(result));
    }

    /// <summary>
    /// Add member to department.
    /// </summary>
    [HttpPost("{id:guid}/members/{userId:guid}")]
    [Authorize(Policy = "CanManageUsers")]
    public async Task<ActionResult<ApiResponse>> AddMember(Guid id, Guid userId)
    {
        _logger.LogInformation("Adding user {UserId} to department {DepartmentId}", userId, id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Member added to department"));
    }

    /// <summary>
    /// Remove member from department.
    /// </summary>
    [HttpDelete("{id:guid}/members/{userId:guid}")]
    [Authorize(Policy = "CanManageUsers")]
    public async Task<ActionResult<ApiResponse>> RemoveMember(Guid id, Guid userId)
    {
        _logger.LogInformation("Removing user {UserId} from department {DepartmentId}", userId, id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Member removed from department"));
    }

    /// <summary>
    /// Set department manager.
    /// </summary>
    [HttpPut("{id:guid}/manager/{userId:guid}")]
    [Authorize(Policy = "CanManageUsers")]
    public async Task<ActionResult<ApiResponse>> SetManager(Guid id, Guid userId)
    {
        _logger.LogInformation("Setting user {UserId} as manager of department {DepartmentId}", userId, id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Department manager updated"));
    }
}
