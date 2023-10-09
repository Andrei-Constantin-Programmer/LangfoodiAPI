using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Utilities;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Domain.Tests.Unit.Models.Users;

public class UserAccountTests
{
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;

    public UserAccountTests()
    {
        _dateTimeProviderMock = new Mock<IDateTimeProvider>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void Constructor_WhenAccountCreationDateIsProvided_DoNotUseDateTimeProvider()
    {
        // Given
        DateTimeOffset creationDate = new(2023, 10, 10, 12, 30, 0, TimeSpan.Zero);

        // When
        var account = new UserAccount(_dateTimeProviderMock.Object, "TestId", "TestHandler", "TestUsername", creationDate);

        // Then
        account.AccountCreationDate.Should().Be(creationDate);
        _dateTimeProviderMock.Verify(provider => provider.Now, Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void Constructor_WhenAccountCreationDateIsNotProvided_UseDateTimeProvider()
    {
        // Given
        var testDate = new DateTimeOffset(2023, 10, 10, 12, 30, 0, TimeSpan.Zero);
        _dateTimeProviderMock
            .Setup(provider => provider.Now)
            .Returns(testDate);

        // When
        var account = new UserAccount(_dateTimeProviderMock.Object, "TestId", "TestHandler", "TestUsername", null);

        // Then
        account.AccountCreationDate.Should().Be(testDate);
    }
}
