namespace UnionTypeExp.Server.UnitTests;

using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;
using UnionTypeExp.Server;

public class PhoneNumberTests
{
    [Test]
    [Arguments("+46701234567")]
    [Arguments("46701234567")]
    [Arguments("12025550123")]
    [Arguments("358401234567")]
    [Arguments("9876543210")]
    public async Task Constructor_Accepts_ValidPhoneNumbers(string phoneNumber) =>
        await Assert.That(new PhoneNumber(phoneNumber)).IsEqualTo(phoneNumber);

    [Test]
    [Arguments("123456")]
    [Arguments("1234567890123456")]
    [Arguments("+0123456789")]
    [Arguments("+46 70 123 45 67")]
    [Arguments("070-1234567")]
    [Arguments("(070)1234567")]
    [Arguments("abc1234567")]
    [Arguments("12345abcde")]
    [Arguments("+46-701234567")]
    [Arguments("+")]
    public async Task Constructor_Rejects_InvalidFormat(string phoneNumber) =>
        await Assert.That(() => new PhoneNumber(phoneNumber)).ThrowsException();

    [Test]
    public async Task Constructor_Rejects_Null() =>
        await Assert.That(() => new PhoneNumber(null!)).ThrowsException();

    [Test]
    [Arguments("")]
    [Arguments(" ")]
    [Arguments("\t")]
    [Arguments("\n")]
    public async Task Constructor_Rejects_Whitespace(string phoneNumber) =>
        await Assert.That(() => new PhoneNumber(phoneNumber)).ThrowsException();
}
