using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Users.Commands;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Users.Commands;

public class ChangeUserRoleHandlerTests
{
    private readonly Mock<IUserPersistenceRepository> _userPersistenceRepositoryMock;
    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;

    private readonly ChangeUserRoleHandler _changeUserRoleHandlerSUT;

    public ChangeUserRoleHandlerTests()
    {
        _userPersistenceRepositoryMock = new Mock<IUserPersistenceRepository>();
        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();

        _changeUserRoleHandlerSUT = new(_userPersistenceRepositoryMock.Object, _userQueryRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUserDoesNotExist_ThrowsUserNotFoundException()
    {
        // Given
        ChangeUserRoleCommand command = new("userId", "role");

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IUserCredentials?)null);

        // When
        var testAction = async () => await _changeUserRoleHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<UserNotFoundException>();
        _userPersistenceRepositoryMock
            .Verify(repo => repo.UpdateUserAsync(It.IsAny<IUserCredentials>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenRoleIsInvalid_ThrowsInvalidUserRoleException()
    {
        // Given
        TestUserCredentials user = new()
        {
            Account = new TestUserAccount()
            {
                Id = "u1",
                Handler = "user_1",
                UserName = "User 1"
            },
            Email = "user1@mail.com",
            Password = "Password123!"
        };

        ChangeUserRoleCommand command = new(user.Account.Id, "InvalidRole");

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // When
        var testAction = async () => await _changeUserRoleHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<InvalidUserRoleException>();
        _userPersistenceRepositoryMock
            .Verify(repo => repo.UpdateUserAsync(It.IsAny<IUserCredentials>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    [InlineData(UserRole.Admin)]
    [InlineData(UserRole.Developer)]
    [InlineData(UserRole.User)]
    public async Task Handle_WhenRoleIsValid_UpdatesUser(UserRole userRole)
    {
        // Given
        TestUserCredentials user = new()
        {
            Account = new TestUserAccount()
            {
                Id = "u1",
                Handler = "user_1",
                UserName = "User 1"
            },
            Email = "user1@mail.com",
            Password = "Password123!"
        };

        ChangeUserRoleCommand command = new(user.Account.Id, userRole.ToString());

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // When
        await _changeUserRoleHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        user.Account.Role.Should().Be(userRole);
        _userPersistenceRepositoryMock
            .Verify(repo => repo.UpdateUserAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }
}
