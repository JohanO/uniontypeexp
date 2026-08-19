namespace UnionTypeExp.Server;

using System.Text.RegularExpressions;

public record NonEmptyString
{
    public string Value { get; }

    public NonEmptyString(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));
        Value = value;
    }

    public static implicit operator string(NonEmptyString nonEmptyString) => nonEmptyString.Value; 
}

public partial record EmailAddress
{
    [GeneratedRegex(
        @"^(?=.{1,254}$)(?=.{1,64}@)(?!.*\.\.)[A-Za-z0-9](?:[A-Za-z0-9._%+-]{0,62}[A-Za-z0-9])?@(?:[A-Za-z0-9](?:[A-Za-z0-9-]{0,61}[A-Za-z0-9])?\.)+[A-Za-z]{2,63}$",
         RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex EmailRegex { get; }

    public string Value { get; }

    public EmailAddress(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));

        if (!EmailRegex.IsMatch(value))
        {
            throw new ArgumentException("Invalid email address format.", nameof(value));
        }

        Value = value;
    }

    public static implicit operator string(EmailAddress emailAddress) => emailAddress.Value; 
}

public partial record PhoneNumber
{
    [GeneratedRegex(
        @"^\+?[1-9]\d{6,14}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex PhoneNumberRegex { get; }

    public string Value { get; }

    public PhoneNumber(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));

        if (!PhoneNumberRegex.IsMatch(value))
        {
            throw new ArgumentException("Invalid phone number format.", nameof(value));
        }

        Value = value;
    }

    public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Value; 
}

public record Customer(NonEmptyString Name, EmailAddress Email, PhoneNumber PhoneNumber);
