using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Workflow.Application.DTOs;
using AFC27.KMS.Workflow.Domain.Entities;

namespace AFC27.KMS.Workflow.Presentation.Controllers;

/// <summary>
/// Controller for service catalog management
/// </summary>
[ApiController]
[Route("api/workflow/catalog")]
[Authorize]
public class ServiceCatalogController : ControllerBase
{
    #region Categories

    /// <summary>
    /// Get all service categories
    /// </summary>
    [HttpGet("categories")]
    [ProducesResponseType(typeof(IEnumerable<ServiceCategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ServiceCategoryDto>>> GetCategories(
        [FromQuery] bool includeInactive = false)
    {
        // TODO: Return categories from database
        var categories = new List<ServiceCategoryDto>();
        return Ok(categories);
    }

    /// <summary>
    /// Get category by ID
    /// </summary>
    [HttpGet("categories/{id:guid}")]
    [ProducesResponseType(typeof(ServiceCategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceCategoryDto>> GetCategory(Guid id)
    {
        // TODO: Return category
        return NotFound();
    }

    /// <summary>
    /// Create a new category
    /// </summary>
    [HttpPost("categories")]
    [Authorize(Policy = "CanManageWorkflow")]
    [ProducesResponseType(typeof(ServiceCategoryDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<ServiceCategoryDto>> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        // TODO: Create category
        var category = new ServiceCategoryDto
        {
            Id = Guid.NewGuid(),
            Name = request.NameEn,
            Description = request.DescriptionEn,
            Icon = request.Icon,
            Color = request.Color,
            IsActive = true
        };

        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
    }

    /// <summary>
    /// Update a category
    /// </summary>
    [HttpPut("categories/{id:guid}")]
    [Authorize(Policy = "CanManageWorkflow")]
    [ProducesResponseType(typeof(ServiceCategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceCategoryDto>> UpdateCategory(
        Guid id,
        [FromBody] UpdateCategoryRequest request)
    {
        // TODO: Update category
        return NotFound();
    }

    /// <summary>
    /// Delete a category
    /// </summary>
    [HttpDelete("categories/{id:guid}")]
    [Authorize(Policy = "CanManageWorkflow")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        // TODO: Delete category
        return NoContent();
    }

    /// <summary>
    /// Reorder categories
    /// </summary>
    [HttpPost("categories/reorder")]
    [Authorize(Policy = "CanManageWorkflow")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ReorderCategories([FromBody] List<Guid> categoryIds)
    {
        // TODO: Reorder categories
        return NoContent();
    }

    #endregion

    #region Services

    /// <summary>
    /// Get all services
    /// </summary>
    [HttpGet("services")]
    [ProducesResponseType(typeof(IEnumerable<ServiceSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ServiceSummaryDto>>> GetServices([FromQuery] ServiceFilterRequest filter)
    {
        // TODO: Return services from database
        var services = new List<ServiceSummaryDto>();
        return Ok(services);
    }

    /// <summary>
    /// Get featured services
    /// </summary>
    [HttpGet("services/featured")]
    [ProducesResponseType(typeof(IEnumerable<ServiceSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ServiceSummaryDto>>> GetFeaturedServices([FromQuery] int limit = 6)
    {
        // TODO: Return featured services
        var services = new List<ServiceSummaryDto>();
        return Ok(services);
    }

    /// <summary>
    /// Get services by category
    /// </summary>
    [HttpGet("categories/{categoryId:guid}/services")]
    [ProducesResponseType(typeof(IEnumerable<ServiceSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ServiceSummaryDto>>> GetServicesByCategory(Guid categoryId)
    {
        // TODO: Return services for category
        var services = new List<ServiceSummaryDto>();
        return Ok(services);
    }

    /// <summary>
    /// Get service by ID
    /// </summary>
    [HttpGet("services/{id:guid}")]
    [ProducesResponseType(typeof(ServiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceDto>> GetService(Guid id)
    {
        // TODO: Return service
        return NotFound();
    }

    /// <summary>
    /// Create a new service
    /// </summary>
    [HttpPost("services")]
    [Authorize(Policy = "CanManageWorkflow")]
    [ProducesResponseType(typeof(ServiceDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<ServiceDto>> CreateService([FromBody] CreateServiceRequest request)
    {
        // TODO: Create service
        var service = new ServiceDto
        {
            Id = Guid.NewGuid(),
            Name = request.NameEn,
            Description = request.DescriptionEn,
            Icon = request.Icon,
            CategoryId = request.CategoryId,
            Type = request.Type,
            DefaultPriority = request.DefaultPriority,
            Status = ServiceStatus.Draft,
            RequiresApproval = request.RequiresApproval,
            CreatedAt = DateTime.UtcNow
        };

        return CreatedAtAction(nameof(GetService), new { id = service.Id }, service);
    }

    /// <summary>
    /// Update a service
    /// </summary>
    [HttpPut("services/{id:guid}")]
    [Authorize(Policy = "CanManageWorkflow")]
    [ProducesResponseType(typeof(ServiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceDto>> UpdateService(
        Guid id,
        [FromBody] UpdateServiceRequest request)
    {
        // TODO: Update service
        return NotFound();
    }

    /// <summary>
    /// Publish a service
    /// </summary>
    [HttpPost("services/{id:guid}/publish")]
    [Authorize(Policy = "CanManageWorkflow")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PublishService(Guid id)
    {
        // TODO: Publish service
        return NoContent();
    }

    /// <summary>
    /// Unpublish a service
    /// </summary>
    [HttpPost("services/{id:guid}/unpublish")]
    [Authorize(Policy = "CanManageWorkflow")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UnpublishService(Guid id)
    {
        // TODO: Unpublish service
        return NoContent();
    }

    /// <summary>
    /// Archive a service
    /// </summary>
    [HttpPost("services/{id:guid}/archive")]
    [Authorize(Policy = "CanManageWorkflow")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ArchiveService(Guid id)
    {
        // TODO: Archive service
        return NoContent();
    }

    /// <summary>
    /// Set service as featured
    /// </summary>
    [HttpPost("services/{id:guid}/featured")]
    [Authorize(Policy = "CanManageWorkflow")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SetFeatured(Guid id, [FromQuery] bool featured = true)
    {
        // TODO: Set featured status
        return NoContent();
    }

    /// <summary>
    /// Configure service access
    /// </summary>
    [HttpPut("services/{id:guid}/access")]
    [Authorize(Policy = "CanManageWorkflow")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ConfigureAccess(
        Guid id,
        [FromBody] ServiceAccessRequest request)
    {
        // TODO: Configure access
        return NoContent();
    }

    /// <summary>
    /// Delete a service
    /// </summary>
    [HttpDelete("services/{id:guid}")]
    [Authorize(Policy = "CanManageWorkflow")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteService(Guid id)
    {
        // TODO: Delete service
        return NoContent();
    }

    /// <summary>
    /// Get service types
    /// </summary>
    [HttpGet("service-types")]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<object>> GetServiceTypes()
    {
        var types = Enum.GetValues<ServiceType>()
            .Select(t => new { Value = (int)t, Name = t.ToString() });
        return Ok(types);
    }

    #endregion
}
