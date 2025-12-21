namespace AFC27.KMS.Contracts.Common;

/// <summary>
/// Standard API response wrapper.
/// </summary>
/// <typeparam name="T">The type of the data payload.</typeparam>
public sealed record ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<ApiError>? Errors { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    public static ApiResponse<T> Ok(T data, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    public static ApiResponse<T> Fail(string message, IEnumerable<ApiError>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors?.ToList()
        };
    }

    public static ApiResponse<T> Fail(IEnumerable<ApiError> errors)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = "One or more validation errors occurred.",
            Errors = errors.ToList()
        };
    }
}

/// <summary>
/// API response without data payload.
/// </summary>
public sealed record ApiResponse
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<ApiError>? Errors { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    public static ApiResponse Ok(string? message = null)
    {
        return new ApiResponse
        {
            Success = true,
            Message = message
        };
    }

    public static ApiResponse Fail(string message, IEnumerable<ApiError>? errors = null)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            Errors = errors?.ToList()
        };
    }
}

/// <summary>
/// Represents an API error.
/// </summary>
public sealed record ApiError(string Code, string Message, string? Field = null);
