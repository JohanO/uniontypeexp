namespace UnionTypeExp.Server.UnitTests;

using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;
using UnionTypeExp.Server;

public class EmailAddressTests
{
    [Test]
    [Arguments("john.doe@example.com")]
    [Arguments("john_doe@example.co.uk")]
    [Arguments("john+promo@example.io")]
    [Arguments("a@bq.se")]
    public async Task Constructor_Accepts_ValidEmails(string email) =>
        await Assert.That(new EmailAddress(email)).IsEqualTo(email);

    [Test]
    [Arguments("plainaddress")]
    [Arguments("missing-at.example.com")]
    [Arguments("user@localhost")]
    [Arguments(".user@example.com")]
    [Arguments("user.@example.com")]
    [Arguments("user..name@example.com")]
    [Arguments("user@-example.com")]
    [Arguments("user@example-.com")]
    [Arguments("user@example.c")]
    [Arguments("user @example.com")]
    [Arguments(" user@example.com")]
    [Arguments("user@example.com ")]
    public async Task Constructor_Rejects_InvalidFormat(string email) =>
        await Assert.That(() => new EmailAddress(email)).ThrowsException();

    [Test]
    public async Task Constructor_Rejects_Null() =>
        await Assert.That(() => new EmailAddress(null!)).ThrowsException();

    [Test]
    [Arguments("")]
    [Arguments(" ")]
    [Arguments("\t")]
    [Arguments("\n")]
    public async Task Constructor_Rejects_Whitespace(string email) =>
        await Assert.That(() => new EmailAddress(email)).ThrowsException();
}
