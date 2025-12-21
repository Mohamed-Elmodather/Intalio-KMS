namespace AFC27.KMS.SharedKernel.Domain;

/// <summary>
/// Base class for value objects.
/// Value objects are immutable and compared by their values, not identity.
/// </summary>
public abstract class ValueObject
{
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
            return false;

        var other = (ValueObject)obj;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }

    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !(left == right);
    }
}

/// <summary>
/// Value object representing an email address.
/// </summary>
public sealed class Email : ValueObject
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        email = email.Trim().ToLowerInvariant();

        if (!email.Contains('@') || !email.Contains('.'))
            throw new ArgumentException("Invalid email format", nameof(email));

        return new Email(email);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(Email email) => email.Value;
}

/// <summary>
/// Value object representing a localized string (English and Arabic).
/// </summary>
public sealed class LocalizedString : ValueObject
{
    public string English { get; private set; }
    public string Arabic { get; private set; }

    // Parameterless constructor for EF Core
    public LocalizedString()
    {
        English = string.Empty;
        Arabic = string.Empty;
    }

    private LocalizedString(string english, string arabic)
    {
        English = english ?? string.Empty;
        Arabic = arabic ?? string.Empty;
    }

    public static LocalizedString Create(string english, string? arabic = null)
    {
        return new LocalizedString(
            english ?? string.Empty,
            arabic ?? english ?? string.Empty
        );
    }

    public static LocalizedString Empty => new LocalizedString();

    public string GetValue(string language)
    {
        return language?.ToLowerInvariant() switch
        {
            "ar" or "arabic" => Arabic,
            _ => English
        };
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return English;
        yield return Arabic;
    }

    public override string ToString() => English;

    public static implicit operator string(LocalizedString? value) => value?.English ?? string.Empty;
}
