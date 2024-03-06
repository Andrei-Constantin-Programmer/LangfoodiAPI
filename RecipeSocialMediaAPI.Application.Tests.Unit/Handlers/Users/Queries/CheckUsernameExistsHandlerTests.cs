using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Handlers.Users.Queries;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Users.Queries;

public class CheckUsernameExistsHandlerTests
{
    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;

    private readonly CheckUsernameExistsHandler _checkUsernameExistsHandlerSUT;

    public CheckUsernameExistsHandlerTests()
    {
        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();

        _checkUsernameExistsHandlerSUT = new CheckUsernameExistsHandler(_userQueryRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUserWithUsernameExists_ReturnTrue()
    {
        // Given
        IUserCredentials testUser = new TestUserCredentials
        {
            Account = new TestUserAccount
            {
                Id = "TestId",
                Handler = "ExistingHandler",
                UserName = "TestUsername",
                AccountCreationDate = new(2023, 10, 9, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "TestEmail",
            Password = "TestPassword"
        };
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByUsername(It.Is<string>(username => username == testUser.Account.UserName), It.IsAny<CancellationToken>()))
            .ReturnsAsync(testUser);

        // When
        var result = await _checkUsernameExistsHandlerSUT.Handle(new CheckUsernameExistsQuery(testUser.Account.UserName), CancellationToken.None);

        // Then
        result.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUserWithUsernameDoesNotExist_ReturnFalse()
    {
        // Given
        IUserCredentials? nullUser = null;
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByUsername(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(nullUser);

        // When
        var result = await _checkUsernameExistsHandlerSUT.Handle(new CheckUsernameExistsQuery("Inexistent username"), CancellationToken.None);

        // Then
        result.Should().BeFalse();
    }
}
