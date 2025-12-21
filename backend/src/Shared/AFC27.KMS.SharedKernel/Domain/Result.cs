namespace AFC27.KMS.SharedKernel.Domain;

/// <summary>
/// Represents the result of an operation that can succeed or fail.
/// Used for explicit error handling without exceptions.
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException("Success result cannot have an error");

        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException("Failure result must have an error");

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, Error.None);

    public static Result Failure(Error error) => new(false, error);

    public static Result<T> Success<T>(T value) => new(value, true, Error.None);

    public static Result<T> Failure<T>(Error error) => new(default!, false, error);

    public static Result<T> Create<T>(T? value) =>
        value is not null ? Success(value) : Failure<T>(Error.NullValue);
}

/// <summary>
/// Represents the result of an operation that returns a value.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
public class Result<T> : Result
{
    private readonly T _value;

    public T Value => IsSuccess
        ? _value
        : throw new InvalidOperationException("Cannot access value of a failed result");

    internal Result(T value, bool isSuccess, Error error) : base(isSuccess, error)
    {
        _value = value;
    }

    public static implicit operator Result<T>(T value) => Create(value);
}

/// <summary>
/// Represents an error with a code and message.
/// </summary>
public sealed record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", "The specified result value is null");

    public static Error Validation(string message) => new("Error.Validation", message);
    public static Error NotFound(string message) => new("Error.NotFound", message);
    public static Error Conflict(string message) => new("Error.Conflict", message);
    public static Error Unauthorized(string message) => new("Error.Unauthorized", message);
    public static Error Forbidden(string message) => new("Error.Forbidden", message);
    public static Error Internal(string message) => new("Error.Internal", message);
}

/// <summary>
/// Common validation errors.
/// </summary>
public static class ValidationErrors
{
    public static Error Required(string fieldName) =>
        Error.Validation($"{fieldName} is required");

    public static Error MaxLength(string fieldName, int maxLength) =>
        Error.Validation($"{fieldName} cannot exceed {maxLength} characters");

    public static Error MinLength(string fieldName, int minLength) =>
        Error.Validation($"{fieldName} must be at least {minLength} characters");

    public static Error InvalidFormat(string fieldName) =>
        Error.Validation($"{fieldName} has an invalid format");

    public static Error MustBePositive(string fieldName) =>
        Error.Validation($"{fieldName} must be a positive number");
}
