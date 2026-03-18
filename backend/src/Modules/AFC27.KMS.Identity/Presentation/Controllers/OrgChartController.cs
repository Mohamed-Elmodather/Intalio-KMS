using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Identity.Application.DTOs;
using AFC27.KMS.Contracts.Common;

namespace AFC27.KMS.Identity.Presentation.Controllers;

/// <summary>
/// Org chart visualization controller (Phase 8F).
/// Provides hierarchical tree structures for organizational chart rendering,
/// leveraging existing User.ManagerId, User.DirectReports, and Department hierarchy.
/// </summary>
[ApiController]
[Route("api/identity/org-chart")]
[Authorize]
public class OrgChartController : ControllerBase
{
    private readonly ILogger<OrgChartController> _logger;

    public OrgChartController(ILogger<OrgChartController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get the org chart tree structure, optionally rooted at a specific department.
    /// Returns a hierarchical tree of departments and their members with reporting relationships.
    /// </summary>
    /// <param name="rootDepartmentId">Optional department ID to use as the root. If omitted, returns the full org tree.</param>
    /// <param name="depth">Maximum depth of the tree to return. Default is 5.</param>
    /// <param name="includeMembers">Whether to include department members in the response. Default true.</param>
    [HttpGet]
    public ActionResult<ApiResponse<OrgChartDto>> GetOrgChart(
        [FromQuery] Guid? rootDepartmentId = null,
        [FromQuery] int depth = 5,
        [FromQuery] bool includeMembers = true)
    {
        _logger.LogInformation(
            "Fetching org chart, rootDepartmentId={RootDepartmentId}, depth={Depth}",
            rootDepartmentId, depth);

        // In a real implementation, this would:
        // 1. Fetch Department tree from repository (optionally rooted at rootDepartmentId)
        // 2. For each department, fetch members with their ManagerId relationships
        // 3. Build a hierarchical tree structure
        // 4. Respect depth limit

        var orgChart = new OrgChartDto
        {
            RootDepartmentId = rootDepartmentId,
            GeneratedAt = DateTime.UtcNow,
            TotalDepartments = 12,
            TotalMembers = 485,
            Departments = new List<OrgChartDepartmentDto>
            {
                new()
                {
                    Id = rootDepartmentId ?? Guid.NewGuid(),
                    Name = "Executive Office",
                    NameArabic = "المكتب التنفيذي",
                    Manager = new OrgChartNodeDto
                    {
                        Id = Guid.NewGuid(),
                        Name = "Khalid Al-Faisal",
                        NameArabic = "خالد الفيصل",
                        AvatarUrl = "/avatars/khalid.jpg",
                        JobTitle = "Chief Executive Officer",
                        DepartmentName = "Executive Office",
                        Children = includeMembers ? new List<OrgChartNodeDto>
                        {
                            new()
                            {
                                Id = Guid.NewGuid(),
                                Name = "Mohammed Al-Rashid",
                                NameArabic = "محمد الراشد",
                                AvatarUrl = "/avatars/mohammed.jpg",
                                JobTitle = "Operations Director",
                                DepartmentName = "Operations",
                                Children = new List<OrgChartNodeDto>
                                {
                                    new()
                                    {
                                        Id = Guid.NewGuid(),
                                        Name = "Ahmed Hassan",
                                        NameArabic = "أحمد حسن",
                                        JobTitle = "Venue Manager",
                                        DepartmentName = "Venue Management"
                                    },
                                    new()
                                    {
                                        Id = Guid.NewGuid(),
                                        Name = "Fatima Al-Zahra",
                                        NameArabic = "فاطمة الزهراء",
                                        JobTitle = "Logistics Coordinator",
                                        DepartmentName = "Logistics"
                                    }
                                }
                            },
                            new()
                            {
                                Id = Guid.NewGuid(),
                                Name = "Sara Ali",
                                NameArabic = "سارة علي",
                                AvatarUrl = "/avatars/sara.jpg",
                                JobTitle = "Technology Director",
                                DepartmentName = "Technology",
                                Children = new List<OrgChartNodeDto>
                                {
                                    new()
                                    {
                                        Id = Guid.NewGuid(),
                                        Name = "Omar Khaled",
                                        NameArabic = "عمر خالد",
                                        JobTitle = "Senior Developer",
                                        DepartmentName = "Software Development"
                                    },
                                    new()
                                    {
                                        Id = Guid.NewGuid(),
                                        Name = "Noor Abdullah",
                                        NameArabic = "نور عبدالله",
                                        JobTitle = "Infrastructure Lead",
                                        DepartmentName = "Infrastructure"
                                    }
                                }
                            },
                            new()
                            {
                                Id = Guid.NewGuid(),
                                Name = "Layla Mohammed",
                                NameArabic = "ليلى محمد",
                                JobTitle = "HR Director",
                                DepartmentName = "Human Resources",
                                Children = new List<OrgChartNodeDto>()
                            }
                        } : new List<OrgChartNodeDto>()
                    },
                    MemberCount = 10,
                    Children = new List<OrgChartDepartmentDto>
                    {
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Name = "Operations",
                            NameArabic = "العمليات",
                            MemberCount = 150,
                            Manager = new OrgChartNodeDto
                            {
                                Id = Guid.NewGuid(),
                                Name = "Mohammed Al-Rashid",
                                NameArabic = "محمد الراشد",
                                AvatarUrl = "/avatars/mohammed.jpg",
                                JobTitle = "Operations Director",
                                DepartmentName = "Operations"
                            },
                            Children = new List<OrgChartDepartmentDto>
                            {
                                new()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Venue Management",
                                    NameArabic = "إدارة الملاعب",
                                    MemberCount = 45
                                },
                                new()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Logistics",
                                    NameArabic = "اللوجستيات",
                                    MemberCount = 60
                                },
                                new()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Security",
                                    NameArabic = "الأمن",
                                    MemberCount = 45
                                }
                            }
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Name = "Technology",
                            NameArabic = "التقنية",
                            MemberCount = 80,
                            Manager = new OrgChartNodeDto
                            {
                                Id = Guid.NewGuid(),
                                Name = "Sara Ali",
                                NameArabic = "سارة علي",
                                AvatarUrl = "/avatars/sara.jpg",
                                JobTitle = "Technology Director",
                                DepartmentName = "Technology"
                            },
                            Children = new List<OrgChartDepartmentDto>
                            {
                                new()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Software Development",
                                    NameArabic = "تطوير البرمجيات",
                                    MemberCount = 30
                                },
                                new()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Infrastructure",
                                    NameArabic = "البنية التحتية",
                                    MemberCount = 25
                                }
                            }
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Name = "Human Resources",
                            NameArabic = "الموارد البشرية",
                            MemberCount = 25,
                            Manager = new OrgChartNodeDto
                            {
                                Id = Guid.NewGuid(),
                                Name = "Layla Mohammed",
                                NameArabic = "ليلى محمد",
                                JobTitle = "HR Director",
                                DepartmentName = "Human Resources"
                            }
                        }
                    }
                }
            }
        };

        return Ok(ApiResponse<OrgChartDto>.Ok(orgChart));
    }

    /// <summary>
    /// Get the reporting chain (manager hierarchy) for a specific user.
    /// Returns the path from the user up to the top-level executive.
    /// </summary>
    [HttpGet("reporting-chain/{userId:guid}")]
    public ActionResult<ApiResponse<IReadOnlyList<OrgChartNodeDto>>> GetReportingChain(Guid userId)
    {
        _logger.LogInformation("Fetching reporting chain for user {UserId}", userId);

        // In a real implementation, this would walk up User.ManagerId until null
        var chain = new List<OrgChartNodeDto>
        {
            new()
            {
                Id = userId,
                Name = "Ahmed Hassan",
                NameArabic = "أحمد حسن",
                JobTitle = "Venue Manager",
                DepartmentName = "Venue Management"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Mohammed Al-Rashid",
                NameArabic = "محمد الراشد",
                AvatarUrl = "/avatars/mohammed.jpg",
                JobTitle = "Operations Director",
                DepartmentName = "Operations"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Khalid Al-Faisal",
                NameArabic = "خالد الفيصل",
                AvatarUrl = "/avatars/khalid.jpg",
                JobTitle = "Chief Executive Officer",
                DepartmentName = "Executive Office"
            }
        };

        return Ok(ApiResponse<IReadOnlyList<OrgChartNodeDto>>.Ok(chain));
    }

    /// <summary>
    /// Get the direct reports tree for a specific user.
    /// </summary>
    [HttpGet("direct-reports/{userId:guid}")]
    public ActionResult<ApiResponse<OrgChartNodeDto>> GetDirectReports(
        Guid userId,
        [FromQuery] int depth = 3)
    {
        _logger.LogInformation("Fetching direct reports for user {UserId}, depth={Depth}", userId, depth);

        var node = new OrgChartNodeDto
        {
            Id = userId,
            Name = "Mohammed Al-Rashid",
            NameArabic = "محمد الراشد",
            AvatarUrl = "/avatars/mohammed.jpg",
            JobTitle = "Operations Director",
            DepartmentName = "Operations",
            Children = new List<OrgChartNodeDto>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Ahmed Hassan",
                    NameArabic = "أحمد حسن",
                    JobTitle = "Venue Manager",
                    DepartmentName = "Venue Management",
                    Children = new List<OrgChartNodeDto>
                    {
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Name = "Youssef Ibrahim",
                            NameArabic = "يوسف إبراهيم",
                            JobTitle = "Venue Coordinator",
                            DepartmentName = "Venue Management"
                        }
                    }
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Fatima Al-Zahra",
                    NameArabic = "فاطمة الزهراء",
                    JobTitle = "Logistics Coordinator",
                    DepartmentName = "Logistics"
                }
            }
        };

        return Ok(ApiResponse<OrgChartNodeDto>.Ok(node));
    }
}
