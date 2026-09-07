namespace UnionTypeExp.Server;

using System.Text.RegularExpressions;

public record NonEmptyString
{
    public string Value { get; }

    public NonEmptyString(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public static implicit operator string(NonEmptyString nonEmptyString) => nonEmptyString.Value;
}

public partial record Email
{
    [GeneratedRegex(
        @"^(?=.{1,254}$)(?=.{1,64}@)(?!.*\.\.)[A-Za-z0-9](?:[A-Za-z0-9._%+-]{0,62}[A-Za-z0-9])?@(?:[A-Za-z0-9](?:[A-Za-z0-9-]{0,61}[A-Za-z0-9])?\.)+[A-Za-z]{2,63}$",
         RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex EmailRegex { get; }

    public string Address { get; }

    public Email(string address)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(address);

        if (!EmailRegex.IsMatch(address))
        {
            throw new ArgumentException("Invalid email address format.", nameof(address));
        }

        Address = address;
    }

    public static implicit operator string(Email email) => email.Address;
}

public partial record Phone
{
    [GeneratedRegex(
        @"^\+?[1-9]\d{6,14}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex PhoneNumberRegex { get; }

    public string Number { get; }

    public Phone(string number)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(number);

        if (!PhoneNumberRegex.IsMatch(number))
        {
            throw new ArgumentException("Invalid phone number format.", nameof(number));
        }

        Number = number;
    }

    public static implicit operator string(Phone phone) => phone.Number;
}

public record EmailAndPhone(Email Email, Phone Phone);

public readonly union ContactInfo(Email, Phone, EmailAndPhone); // NOSONAR

public record Customer(NonEmptyString Name, ContactInfo ContactInfo);

public static class CustomerExtensions
{
    public static string GetContactInfoDescription(this Customer customer) =>
        customer.ContactInfo switch
        {
            Email email => $"Email: {email}",
            Phone phone => $"Phone: {phone}",
            EmailAndPhone emailAndPhone => $"Email: {emailAndPhone.Email}, Phone: {emailAndPhone.Phone}",
        };
}

// Adding a new contact info type is easy and does not require changes to existing code. For example, we can add a new contact info with two email addresses.
public record TwoEmailAddresses(Email Email1, Email Email2);

// Then changing the union type to include the new contact info type is also easy and does not require changes to existing code. The compiler will complain about missing cases in the switch expressions, which is a good thing because it forces us to handle the new contact info type.
// public readonly union ContactInfo(Email, Phone, EmailAndPhone, TwoEmailAddresses); // NOSONAR


// -----------------------------

// Version without union types
// It is possible to have a customer without an email and phone number which is against the business rules and is considered invalid. This is just for demonstration purposes.
public record CustomerWithoutUnionTypes(NonEmptyString Name, Email? Email, Phone? Phone);

public static class CustomerWithoutUnionTypesExtensions
{
    public static string GetContactInfoDescription(this CustomerWithoutUnionTypes customer) =>
        (customer.Email, customer.Phone) switch
        {
            (Email email, null) => $"Email: {email}",
            (null, Phone phone) => $"Phone: {phone}",
            (Email email, Phone phone) => $"Email: {email}, Phone: {phone}",
            (null, null) => throw new InvalidOperationException("Illegal: Customer has no contact information. This should not happen.")
        };
}

// Adding another contact info type without union types is also possible but requires more code and is less elegant.
// We need to add another field to the existing CustomerWithoutUnionTypes record and then update all switch expressions to handle the new contact info type.
// This is more error-prone and requires more code changes, which is not ideal.

// ----------------------------

// Version using classical inheritance
public interface IContactInfo;

public record EmailContactInfo(Email Email) : IContactInfo;
public record PhoneContactInfo(Phone Phone) : IContactInfo;
public record EmailAndPhoneContactInfo(Email Email, Phone Phone) : IContactInfo;

public record CustomerWithInheritance(NonEmptyString Name, IContactInfo ContactInfo);

public static class CustomerWithInheritanceExtensions
{
    public static string GetContactInfoDescription(this CustomerWithInheritance customer) =>
        customer.ContactInfo switch
        {
            EmailContactInfo email => $"Email: {email.Email}",
            PhoneContactInfo phone => $"Phone: {phone.Phone}",
            EmailAndPhoneContactInfo emailAndPhone => $"Email: {emailAndPhone.Email}, Phone: {emailAndPhone.Phone}",
            not null => throw new InvalidOperationException($"Internal server error: Unknown contact info type: {customer.ContactInfo.GetType().Name}. This should not happen."),
            null => throw new InvalidOperationException("Internal server error: Customer has no contact information. This should not happen.")
        };
}

// Adding a new contact info type is easy and does not require changes to existing code. For example, we can add a new contact info with two email addresses.
public record TwoEmailAddressesContactInfo(Email Email1, Email Email2) : IContactInfo; // NOSONAR

// The existing switch expression will have to be updated to handle the new contact info type.
// The compiler will NOT complain about missing cases in the switch expressions as we have already covered the 'not null' case,
// which is a bad thing because it allows us to forget to handle the new contact info type.
// This is one of the main disadvantages of using classical inheritance over union types.

