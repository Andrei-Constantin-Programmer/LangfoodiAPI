using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Commands;

public class CreateGroupHandlerTests
{
    private readonly Mock<IGroupPersistenceRepository> _groupPersistenceRepositoryMock;
    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;

    private readonly CreateGroupHandler _groupHandlerSUT;

    public CreateGroupHandlerTests()
    {
        _groupPersistenceRepositoryMock = new Mock<IGroupPersistenceRepository>();
        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();

        _groupHandlerSUT = new(_groupPersistenceRepositoryMock.Object, _userQueryRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUsersExist_CreateAndReturnGroup()
    {
        // Given
        TestUserAccount userAccount1 = new()
        {
            Id = "user1",
            Handler = "user1",
            UserName = "UserName 1",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        TestUserAccount userAccount2 = new()
        {
            Id = "user2",
            Handler = "user2",
            UserName = "UserName 2",
            AccountCreationDate = new(2023, 2, 2, 0, 0, 0, TimeSpan.Zero)
        };
        TestUserAccount userAccount3 = new()
        {
            Id = "user3",
            Handler = "user3",
            UserName = "UserName 3",
            AccountCreationDate = new(2023, 3, 3, 0, 0, 0, TimeSpan.Zero)
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(userAccount1.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TestUserCredentials() { Account = userAccount1, Email = "test1@mail.com", Password = "TestPass" });
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(userAccount2.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TestUserCredentials() { Account = userAccount2, Email = "test2@mail.com", Password = "TestPass" });
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(userAccount3.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TestUserCredentials() { Account = userAccount3, Email = "test3@mail.com", Password = "TestPass" });

        List<string> userIds = new() { userAccount1.Id, userAccount2.Id, userAccount3.Id };
        List<IUserAccount> users = new() { userAccount1, userAccount2, userAccount3 };

        NewGroupContract testContract = new("name", "description", userIds);
        Group testGroup = new("1","name","description", users);

        _groupPersistenceRepositoryMock
            .Setup(repo => repo.CreateGroupAsync("name", "description", users, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testGroup);

        // When
        var result = await _groupHandlerSUT.Handle(new CreateGroupCommand(testContract), CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.UserIds.Should().NotBeNull();
        result.UserIds[0].Should().Be(testGroup.Users[0].Id);
        result.UserIds[1].Should().Be(testGroup.Users[1].Id);
        result.UserIds[2].Should().Be(testGroup.Users[2].Id);
        result.Id.Should().Be(testGroup.GroupId);
        result.Description.Should().Be(testContract.Description);
        result.Name.Should().Be(testContract.Name);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    [InlineData(false, true, true)]
    [InlineData(true, false, true)]
    [InlineData(true, true, false)]
    public async Task Handle_WhenUserDoesNotExist_ThrowsUserNotFoundException(bool user1Exists, bool user2Exists, bool user3Exists)
    {
        // Given
        TestUserAccount userAccount1 = new()
        {
            Id = "user1",
            Handler = "user1",
            UserName = "UserName 1",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        TestUserAccount userAccount2 = new()
        {
            Id = "user2",
            Handler = "user2",
            UserName = "UserName 2",
            AccountCreationDate = new(2023, 2, 2, 0, 0, 0, TimeSpan.Zero)
        };
        TestUserAccount userAccount3 = new()
        {
            Id = "user3",
            Handler = "user3",
            UserName = "UserName 3",
            AccountCreationDate = new(2023, 3, 3, 0, 0, 0, TimeSpan.Zero)
        };

        if (user1Exists)
        {
            _userQueryRepositoryMock
                .Setup(repo => repo.GetUserByIdAsync(userAccount1.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TestUserCredentials() { Account = userAccount1, Email = "test1@mail.com", Password = "TestPass" });
        }

        if (user2Exists) 
        {
            _userQueryRepositoryMock
                .Setup(repo => repo.GetUserByIdAsync(userAccount2.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TestUserCredentials() { Account = userAccount2, Email = "test2@mail.com", Password = "TestPass" });
        }

        if (user3Exists)
        {
            _userQueryRepositoryMock
                .Setup(repo => repo.GetUserByIdAsync(userAccount3.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TestUserCredentials() { Account = userAccount3, Email = "test3@mail.com", Password = "TestPass" });
        }

        List<string> userIds = new() { userAccount1.Id, userAccount2.Id, userAccount3.Id };
        List<IUserAccount> users = new() { userAccount1, userAccount2, userAccount3 };

        NewGroupContract testContract = new("name", "description", userIds);
        Group testGroup = new("1", "name", "description", users);

        _groupPersistenceRepositoryMock
            .Setup(repo => repo.CreateGroupAsync("name", "description", users, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testGroup);

        // When
        var testAction = async () => await _groupHandlerSUT.Handle(new CreateGroupCommand(testContract), CancellationToken.None);

        // Then
        await testAction.Should()
           .ThrowAsync<UserNotFoundException>();
    }
}
