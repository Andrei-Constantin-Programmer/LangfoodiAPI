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

public class UpdateGroupHandlerTests
{
    private readonly Mock<IGroupQueryRepository> _groupQueryRepositoryMock;
    private readonly Mock<IGroupPersistenceRepository> _groupPersistenceRepositoryMock;
    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;


    private readonly UpdateGroupHandler _updateGroupHandlerSUT;

    public UpdateGroupHandlerTests()
    {
        _groupQueryRepositoryMock = new Mock<IGroupQueryRepository>();
        _groupPersistenceRepositoryMock = new Mock<IGroupPersistenceRepository>();
        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();

        _updateGroupHandlerSUT = new(_groupQueryRepositoryMock.Object, _groupPersistenceRepositoryMock.Object, _userQueryRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenGroupMetadataIsUpdatedSuccessfully_ReturnTrue()
    {
        // Given
        Group existingGroup = new("1", "Group", "Group Desc");

        UpdateGroupCommand command = new(new UpdateGroupContract(
            existingGroup.GroupId, 
            "New Group Name", 
            "New Group Description", 
            new List<string>()));

        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupById(existingGroup.GroupId))
            .Returns(existingGroup);

        _groupPersistenceRepositoryMock
            .Setup(repo => repo.UpdateGroup(It.Is<Group>(
                group => group.GroupId == existingGroup.GroupId
                      && group.GroupName == command.UpdateGroupContract.GroupName
                      && group.GroupDescription == command.UpdateGroupContract.GroupDescription
                      && !group.Users.Any())))
            .Returns(true);

        // When
        var result = await _updateGroupHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        result.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenGroupIsNotUpdatedSuccessfully_ReturnFalse()
    {
        // Given
        Group existingGroup = new("1", "Group", "Group Desc");

        UpdateGroupCommand command = new(new UpdateGroupContract(
            existingGroup.GroupId, 
            "New Group Name", 
            "New Group Description", 
            new List<string>()));

        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupById(existingGroup.GroupId))
            .Returns(existingGroup);

        _groupPersistenceRepositoryMock
            .Setup(repo => repo.UpdateGroup(It.Is<Group>(
                group => group.GroupId == existingGroup.GroupId
                      && group.GroupName == command.UpdateGroupContract.GroupName
                      && group.GroupDescription == command.UpdateGroupContract.GroupDescription
                      && !group.Users.Any())))
            .Returns(false);

        // When
        var result = await _updateGroupHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        result.Should().BeFalse();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenGroupSuffersNoChanges_ReturnTrue()
    {
        // Given
        Group existingGroup = new("1", "Group", "Group Desc");

        UpdateGroupCommand command = new(new UpdateGroupContract(
            existingGroup.GroupId, 
            existingGroup.GroupName, 
            existingGroup.GroupDescription, 
            new List<string>()));

        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupById(existingGroup.GroupId))
            .Returns(existingGroup);

        _groupPersistenceRepositoryMock
            .Setup(repo => repo.UpdateGroup(It.IsAny<Group>()))
            .Returns(true);

        // When
        var result = await _updateGroupHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        result.Should().BeTrue();
    }
    
    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenGroupUsersAreRemoved_UpdateAndReturnTrue()
    {
        // Given
        List<IUserCredentials> users = new()
        {
            new TestUserCredentials()
            {
                Account = new TestUserAccount()
                {
                    Id = "1",
                    Handler = "user1",
                    UserName = "User 1",
                    AccountCreationDate = new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)
                },
                Email = "user1@mail.com",
                Password = "pass!123"
            },
            new TestUserCredentials()
            {
                Account = new TestUserAccount()
                {
                    Id = "2",
                    Handler = "user2",
                    UserName = "User 2",
                    AccountCreationDate = new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero)
                },
                Email = "user2@mail.com",
                Password = "pass!123"
            },
            new TestUserCredentials()
            {
                Account = new TestUserAccount()
                {
                    Id = "3",
                    Handler = "user3",
                    UserName = "User 3",
                    AccountCreationDate = new(2024, 3, 3, 0, 0, 0, TimeSpan.Zero)
                },
                Email = "user3@mail.com",
                Password = "pass!123"
            },
            new TestUserCredentials()
            {
                Account = new TestUserAccount()
                {
                    Id = "4",
                    Handler = "user4",
                    UserName = "User 4",
                    AccountCreationDate = new(2024, 4, 4, 0, 0, 0, TimeSpan.Zero)
                },
                Email = "user4@mail.com",
                Password = "pass!123"
            }
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(users[0].Account.Id))
            .Returns(users[0]);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(users[1].Account.Id))
            .Returns(users[1]);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(users[2].Account.Id))
            .Returns(users[2]);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(users[3].Account.Id))
            .Returns(users[3]);

        Group existingGroup = new("1", "Group", "Group Desc", users.Select(user => user.Account));

        UpdateGroupCommand command = new(new UpdateGroupContract(
            existingGroup.GroupId, 
            existingGroup.GroupName, 
            existingGroup.GroupDescription, 
            new List<string>()
            {
                users[0].Account.Id,
                users[1].Account.Id,
            }));

        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupById(existingGroup.GroupId))
            .Returns(existingGroup);

        _groupPersistenceRepositoryMock
            .Setup(repo => repo.UpdateGroup(It.Is<Group>(
                group => group.GroupId == existingGroup.GroupId
                      && group.GroupName == existingGroup.GroupName
                      && group.GroupDescription == existingGroup.GroupDescription
                      && group.Users.Count == 2
                      && group.Users.Contains(users[0].Account)
                      && group.Users.Contains(users[1].Account))))
            .Returns(true);

        // When
        var result = await _updateGroupHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        result.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenGroupUsersAreAdded_UpdateAndReturnTrue()
    {
        // Given
        List<IUserCredentials> users = new()
        {
            new TestUserCredentials()
            {
                Account = new TestUserAccount()
                {
                    Id = "1",
                    Handler = "user1",
                    UserName = "User 1",
                    AccountCreationDate = new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)
                },
                Email = "user1@mail.com",
                Password = "pass!123"
            },
            new TestUserCredentials()
            {
                Account = new TestUserAccount()
                {
                    Id = "2",
                    Handler = "user2",
                    UserName = "User 2",
                    AccountCreationDate = new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero)
                },
                Email = "user2@mail.com",
                Password = "pass!123"
            },
            new TestUserCredentials()
            {
                Account = new TestUserAccount()
                {
                    Id = "3",
                    Handler = "user3",
                    UserName = "User 3",
                    AccountCreationDate = new(2024, 3, 3, 0, 0, 0, TimeSpan.Zero)
                },
                Email = "user3@mail.com",
                Password = "pass!123"
            },
            new TestUserCredentials()
            {
                Account = new TestUserAccount()
                {
                    Id = "4",
                    Handler = "user4",
                    UserName = "User 4",
                    AccountCreationDate = new(2024, 4, 4, 0, 0, 0, TimeSpan.Zero)
                },
                Email = "user4@mail.com",
                Password = "pass!123"
            }
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(users[0].Account.Id))
            .Returns(users[0]);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(users[1].Account.Id))
            .Returns(users[1]);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(users[2].Account.Id))
            .Returns(users[2]);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(users[3].Account.Id))
            .Returns(users[3]);

        Group existingGroup = new("1", "Group", "Group Desc", users.Take(3).Select(user => user.Account));

        UpdateGroupCommand command = new(new UpdateGroupContract(
            existingGroup.GroupId, 
            existingGroup.GroupName, 
            existingGroup.GroupDescription, 
            new List<string>()
            {
                users[0].Account.Id,
                users[1].Account.Id,
                users[2].Account.Id,
                users[3].Account.Id,
            }));

        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupById(existingGroup.GroupId))
            .Returns(existingGroup);

        _groupPersistenceRepositoryMock
            .Setup(repo => repo.UpdateGroup(It.Is<Group>(
                group => group.GroupId == existingGroup.GroupId
                      && group.GroupName == existingGroup.GroupName
                      && group.GroupDescription == existingGroup.GroupDescription
                      && group.Users.Count == 4
                      && group.Users.Contains(users[0].Account)
                      && group.Users.Contains(users[1].Account)
                      && group.Users.Contains(users[2].Account)
                      && group.Users.Contains(users[3].Account))))
            .Returns(true);

        // When
        var result = await _updateGroupHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        result.Should().BeTrue();
    }
    
    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenGroupUsersAreBothAddedAndRemoved_UpdateAndReturnTrue()
    {
        // Given
        List<IUserCredentials> users = new()
        {
            new TestUserCredentials()
            {
                Account = new TestUserAccount()
                {
                    Id = "1",
                    Handler = "user1",
                    UserName = "User 1",
                    AccountCreationDate = new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)
                },
                Email = "user1@mail.com",
                Password = "pass!123"
            },
            new TestUserCredentials()
            {
                Account = new TestUserAccount()
                {
                    Id = "2",
                    Handler = "user2",
                    UserName = "User 2",
                    AccountCreationDate = new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero)
                },
                Email = "user2@mail.com",
                Password = "pass!123"
            },
            new TestUserCredentials()
            {
                Account = new TestUserAccount()
                {
                    Id = "3",
                    Handler = "user3",
                    UserName = "User 3",
                    AccountCreationDate = new(2024, 3, 3, 0, 0, 0, TimeSpan.Zero)
                },
                Email = "user3@mail.com",
                Password = "pass!123"
            },
            new TestUserCredentials()
            {
                Account = new TestUserAccount()
                {
                    Id = "4",
                    Handler = "user4",
                    UserName = "User 4",
                    AccountCreationDate = new(2024, 4, 4, 0, 0, 0, TimeSpan.Zero)
                },
                Email = "user4@mail.com",
                Password = "pass!123"
            }
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(users[0].Account.Id))
            .Returns(users[0]);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(users[1].Account.Id))
            .Returns(users[1]);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(users[2].Account.Id))
            .Returns(users[2]);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(users[3].Account.Id))
            .Returns(users[3]);

        Group existingGroup = new("1", "Group", "Group Desc", users.Take(3).Select(user => user.Account));

        UpdateGroupCommand command = new(new UpdateGroupContract(
            existingGroup.GroupId, 
            existingGroup.GroupName, 
            existingGroup.GroupDescription, 
            new List<string>()
            {
                users[0].Account.Id,
                users[3].Account.Id,
            }));

        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupById(existingGroup.GroupId))
            .Returns(existingGroup);

        _groupPersistenceRepositoryMock
            .Setup(repo => repo.UpdateGroup(It.Is<Group>(
                group => group.GroupId == existingGroup.GroupId
                      && group.GroupName == existingGroup.GroupName
                      && group.GroupDescription == existingGroup.GroupDescription
                      && group.Users.Count == 2
                      && group.Users.Contains(users[0].Account)
                      && group.Users.Contains(users[3].Account))))
            .Returns(true);

        // When
        var result = await _updateGroupHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        result.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenGroupDoesNotExist_ThrowGroupNotFoundException()
    {
        // Given
        UpdateGroupCommand command = new(new UpdateGroupContract(
            "1",
            "New Group Name",
            "New Group Description",
            new List<string>()));

        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupById(It.IsAny<string>()))
            .Returns((Group?)null);

        // When
        var testAction = async () => await _updateGroupHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<GroupNotFoundException>().WithMessage($"*{command.UpdateGroupContract.GroupId}*");
    }


    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUserNotFoundInTheDatabase_ThrowUserNotFoundException()
    {
        // Given
        TestUserAccount testUser = new()
        {
            Id = "1",
            Handler = "user1",
            UserName = "User 1",
            AccountCreationDate = new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(It.IsAny<string>()))
            .Returns((IUserCredentials?)null);

        Group existingGroup = new("1", "Group", "Group Desc");

        UpdateGroupCommand command = new(new UpdateGroupContract(
            existingGroup.GroupId,
            existingGroup.GroupName,
            existingGroup.GroupDescription,
            new List<string>()
            {
                testUser.Id,
            }));

        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupById(existingGroup.GroupId))
            .Returns(existingGroup);

        // When
        var testAction = async () => await _updateGroupHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<UserNotFoundException>().WithMessage($"*{testUser.Id}*");
    }
}
