using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Calendar.Application.DTOs;
using AFC27.KMS.Calendar.Domain.Entities;

namespace AFC27.KMS.Calendar.Presentation.Controllers;

/// <summary>
/// Controller for holiday management
/// </summary>
[ApiController]
[Route("api/calendar/holidays")]
[Authorize]
public class HolidaysController : ControllerBase
{
    /// <summary>
    /// Get holidays for a year
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<HolidayDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<HolidayDto>>> GetHolidays(
        [FromQuery] int? year = null,
        [FromQuery] string? country = null)
    {
        // TODO: Return holidays
        var holidays = new List<HolidayDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Saudi National Day",
                NameAr = "اليوم الوطني السعودي",
                Date = new DateTime(year ?? DateTime.Now.Year, 9, 23),
                Type = HolidayType.National,
                Country = "SA",
                IsPublic = true,
                AffectsWorkingHours = true,
                Year = year ?? DateTime.Now.Year
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Founding Day",
                NameAr = "يوم التأسيس",
                Date = new DateTime(year ?? DateTime.Now.Year, 2, 22),
                Type = HolidayType.National,
                Country = "SA",
                IsPublic = true,
                AffectsWorkingHours = true,
                Year = year ?? DateTime.Now.Year
            }
        };
        return Ok(holidays);
    }

    /// <summary>
    /// Get holiday by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(HolidayDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HolidayDto>> GetHoliday(Guid id)
    {
        // TODO: Return holiday
        return NotFound();
    }

    /// <summary>
    /// Create a holiday
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "CanManageCalendar")]
    [ProducesResponseType(typeof(HolidayDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<HolidayDto>> CreateHoliday([FromBody] CreateHolidayRequest request)
    {
        // TODO: Create holiday
        var holiday = new HolidayDto
        {
            Id = Guid.NewGuid(),
            Name = request.NameEn,
            NameAr = request.NameAr,
            Description = request.DescriptionEn,
            Date = request.Date,
            EndDate = request.EndDate,
            IsRecurring = request.IsRecurring,
            Type = request.Type,
            Country = request.Country,
            Region = request.Region,
            IsPublic = request.IsPublic,
            AffectsWorkingHours = request.AffectsWorkingHours,
            Year = request.Date.Year
        };

        return CreatedAtAction(nameof(GetHoliday), new { id = holiday.Id }, holiday);
    }

    /// <summary>
    /// Update a holiday
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "CanManageCalendar")]
    [ProducesResponseType(typeof(HolidayDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HolidayDto>> UpdateHoliday(
        Guid id,
        [FromBody] CreateHolidayRequest request)
    {
        // TODO: Update holiday
        return NotFound();
    }

    /// <summary>
    /// Delete a holiday
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "CanManageCalendar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteHoliday(Guid id)
    {
        // TODO: Delete holiday
        return NoContent();
    }

    /// <summary>
    /// Get holiday types
    /// </summary>
    [HttpGet("types")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<object>> GetHolidayTypes()
    {
        var types = Enum.GetValues<HolidayType>()
            .Select(t => new { Value = (int)t, Name = t.ToString() });
        return Ok(types);
    }

    /// <summary>
    /// Import holidays from external source
    /// </summary>
    [HttpPost("import")]
    [Authorize(Policy = "CanManageCalendar")]
    [ProducesResponseType(typeof(ImportResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ImportResultDto>> ImportHolidays(
        [FromQuery] string country,
        [FromQuery] int year)
    {
        // TODO: Import holidays
        var result = new ImportResultDto
        {
            EventsImported = 0
        };
        return Ok(result);
    }
}

/// <summary>
/// Controller for time off / out of office management
/// </summary>
[ApiController]
[Route("api/calendar/time-off")]
[Authorize]
public class TimeOffController : ControllerBase
{
    /// <summary>
    /// Get my time off requests
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TimeOffDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TimeOffDto>>> GetMyTimeOff(
        [FromQuery] int? year = null,
        [FromQuery] TimeOffStatus? status = null)
    {
        // TODO: Return time off requests
        var timeOffs = new List<TimeOffDto>();
        return Ok(timeOffs);
    }

    /// <summary>
    /// Get team time off requests (manager view)
    /// </summary>
    [HttpGet("team")]
    [ProducesResponseType(typeof(IEnumerable<TimeOffDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TimeOffDto>>> GetTeamTimeOff(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] TimeOffStatus? status = null)
    {
        // TODO: Return team time off requests
        var timeOffs = new List<TimeOffDto>();
        return Ok(timeOffs);
    }

    /// <summary>
    /// Get pending approvals
    /// </summary>
    [HttpGet("pending-approvals")]
    [ProducesResponseType(typeof(IEnumerable<TimeOffDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TimeOffDto>>> GetPendingApprovals()
    {
        // TODO: Return pending approvals
        var timeOffs = new List<TimeOffDto>();
        return Ok(timeOffs);
    }

    /// <summary>
    /// Get time off by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TimeOffDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TimeOffDto>> GetTimeOff(Guid id)
    {
        // TODO: Return time off
        return NotFound();
    }

    /// <summary>
    /// Request time off
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TimeOffDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<TimeOffDto>> RequestTimeOff([FromBody] CreateTimeOffRequest request)
    {
        // TODO: Create time off request
        var timeOff = new TimeOffDto
        {
            Id = Guid.NewGuid(),
            Type = request.Type,
            Reason = request.ReasonEn,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            IsAllDay = request.IsAllDay,
            TotalDays = (int)(request.EndDate - request.StartDate).TotalDays + 1,
            Status = TimeOffStatus.Pending,
            DelegateToId = request.DelegateToId,
            CreatedAt = DateTime.UtcNow
        };

        return CreatedAtAction(nameof(GetTimeOff), new { id = timeOff.Id }, timeOff);
    }

    /// <summary>
    /// Update time off request
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TimeOffDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TimeOffDto>> UpdateTimeOff(
        Guid id,
        [FromBody] CreateTimeOffRequest request)
    {
        // TODO: Update time off
        return NotFound();
    }

    /// <summary>
    /// Cancel time off request
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CancelTimeOff(Guid id)
    {
        // TODO: Cancel time off
        return NoContent();
    }

    /// <summary>
    /// Approve or reject time off request
    /// </summary>
    [HttpPost("{id:guid}/approve")]
    [ProducesResponseType(typeof(TimeOffDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<TimeOffDto>> ApproveTimeOff(
        Guid id,
        [FromBody] TimeOffApprovalRequest request)
    {
        // TODO: Approve/reject time off
        var timeOff = new TimeOffDto
        {
            Id = id,
            Status = request.Approve ? TimeOffStatus.Approved : TimeOffStatus.Rejected,
            ApprovedAt = DateTime.UtcNow,
            ApprovalComment = request.Comment
        };
        return Ok(timeOff);
    }

    /// <summary>
    /// Get time off types
    /// </summary>
    [HttpGet("types")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<object>> GetTimeOffTypes()
    {
        var types = Enum.GetValues<TimeOffType>()
            .Select(t => new { Value = (int)t, Name = t.ToString() });
        return Ok(types);
    }

    /// <summary>
    /// Get time off balance
    /// </summary>
    [HttpGet("balance")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> GetTimeOffBalance([FromQuery] int? year = null)
    {
        // TODO: Return time off balance
        var balance = new
        {
            Year = year ?? DateTime.Now.Year,
            VacationTotal = 21,
            VacationUsed = 5,
            VacationRemaining = 16,
            SickTotal = 10,
            SickUsed = 2,
            SickRemaining = 8
        };
        return Ok(balance);
    }
}

/// <summary>
/// Controller for working hours management
/// </summary>
[ApiController]
[Route("api/calendar/working-hours")]
[Authorize]
public class WorkingHoursController : ControllerBase
{
    /// <summary>
    /// Get my working hours
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<WorkingHoursDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<WorkingHoursDto>>> GetWorkingHours()
    {
        // TODO: Return working hours
        var hours = new List<WorkingHoursDto>
        {
            new() { DayOfWeek = DayOfWeek.Sunday, StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(17, 0, 0), IsWorkingDay = true },
            new() { DayOfWeek = DayOfWeek.Monday, StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(17, 0, 0), IsWorkingDay = true },
            new() { DayOfWeek = DayOfWeek.Tuesday, StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(17, 0, 0), IsWorkingDay = true },
            new() { DayOfWeek = DayOfWeek.Wednesday, StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(17, 0, 0), IsWorkingDay = true },
            new() { DayOfWeek = DayOfWeek.Thursday, StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(17, 0, 0), IsWorkingDay = true },
            new() { DayOfWeek = DayOfWeek.Friday, IsWorkingDay = false },
            new() { DayOfWeek = DayOfWeek.Saturday, IsWorkingDay = false }
        };
        return Ok(hours);
    }

    /// <summary>
    /// Update my working hours
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(IEnumerable<WorkingHoursDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<WorkingHoursDto>>> UpdateWorkingHours(
        [FromBody] UpdateWorkingHoursRequest request)
    {
        // TODO: Update working hours
        return Ok(request.WorkingHours);
    }

    /// <summary>
    /// Get organization default working hours
    /// </summary>
    [HttpGet("organization")]
    [ProducesResponseType(typeof(IEnumerable<WorkingHoursDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<WorkingHoursDto>>> GetOrganizationWorkingHours()
    {
        // TODO: Return org working hours
        var hours = new List<WorkingHoursDto>();
        return Ok(hours);
    }

    /// <summary>
    /// Update organization working hours
    /// </summary>
    [HttpPut("organization")]
    [Authorize(Policy = "CanManageCalendar")]
    [ProducesResponseType(typeof(IEnumerable<WorkingHoursDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<WorkingHoursDto>>> UpdateOrganizationWorkingHours(
        [FromBody] UpdateWorkingHoursRequest request)
    {
        // TODO: Update org working hours
        return Ok(request.WorkingHours);
    }
}
