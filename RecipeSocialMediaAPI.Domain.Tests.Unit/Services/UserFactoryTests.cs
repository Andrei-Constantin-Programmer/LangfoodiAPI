using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Moq;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Services;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.Domain.Utilities;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Domain.Tests.Unit.Services;

public class UserFactoryTests
{
    private readonly UserFactory _userFactorySUT;

    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;

    public UserFactoryTests()
    {
        _dateTimeProviderMock = new Mock<IDateTimeProvider>();

        _userFactorySUT = new(_dateTimeProviderMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void CreateUserAccount_ReturnsExpectedUserAccount()
    {
        // Given
        string id = "TestId";
        string handler = "TestHandler";
        string username = "TestUsername";
        DateTimeOffset creationDate = new(2023, 10, 10, 12, 30, 0, TimeSpan.Zero);

        // When
        var account = _userFactorySUT.CreateUserAccount(id, handler, username, creationDate);

        // Then
        account.Id.Should().Be(id);
        account.Handler.Should().Be(handler);
        account.UserName.Should().Be(username);
        account.AccountCreationDate.Should().Be(creationDate);

        _dateTimeProviderMock
            .Verify(provider => provider.Now, Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void CreateUserAccount_WhenAccountCreationDateIsNotProvided_UseDateTimeProvider()
    {
        // Given
        var testDate = new DateTimeOffset(2023, 10, 10, 12, 30, 0, TimeSpan.Zero);
        _dateTimeProviderMock
            .Setup(provider => provider.Now)
            .Returns(testDate);

        // When
        var account = _userFactorySUT.CreateUserAccount("TestId", "TestHandler", "TestUsername", null);

        // Then
        account.AccountCreationDate.Should().Be(testDate);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void CreateUserCredentials_WithExistingAccount_ReturnsExpectedUserCredentials()
    {
        // Given
        string email = "TestEmail";
        string password = "TestPassword";
        
        TestUserAccount testAccount = new()
        {
            Id = "TestId",
            Handler = "TestHandler",
            UserName = "TestUsername",
            AccountCreationDate = new(2023, 10, 10, 12, 30, 0, TimeSpan.Zero)
        };

        // When
        var account = _userFactorySUT.CreateUserCredentials(testAccount, email, password);

        // Then
        account.Account.Should().Be(testAccount);
        account.Email.Should().Be(email);
        account.Password.Should().Be(password);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void CreateUserCredentials_WithoutExistingAccount_ReturnsExpectedUserCredentials()
    {
        // Given
        string id = "TestId";
        string handler = "TestHandler";
        string username = "TestUsername";
        DateTimeOffset creationDate = new(2023, 10, 10, 12, 30, 0, TimeSpan.Zero);

        string email = "TestEmail";
        string password = "TestPassword";

        // When
        var account = _userFactorySUT.CreateUserCredentials(id, handler, username, email, password, creationDate);

        // Then
        account.Account.Id.Should().Be(id);
        account.Account.Handler.Should().Be(handler);
        account.Account.UserName.Should().Be(username);
        account.Account.AccountCreationDate.Should().Be(creationDate);
        account.Email.Should().Be(email);
        account.Password.Should().Be(password);

        _dateTimeProviderMock
            .Verify(provider => provider.Now, Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void CreateUserCredentials_WithoutExistingAccountAndWhenAccountCreationDateIsNotProvided_UseDateTimeProvider()
    {
        // Given
        string id = "TestId";
        string handler = "TestHandler";
        string username = "TestUsername";

        string email = "TestEmail";
        string password = "TestPassword";

        var testDate = new DateTimeOffset(2023, 10, 10, 12, 30, 0, TimeSpan.Zero);
        _dateTimeProviderMock
            .Setup(provider => provider.Now)
            .Returns(testDate);

        // When
        var account = _userFactorySUT.CreateUserCredentials(id, handler, username, email, password);

        // Then
        account.Account.Id.Should().Be(id);
        account.Account.Handler.Should().Be(handler);
        account.Account.UserName.Should().Be(username);
        account.Account.AccountCreationDate.Should().Be(testDate);
        account.Email.Should().Be(email);
        account.Password.Should().Be(password);
    }
}
