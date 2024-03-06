using FluentAssertions;
using Microsoft.Extensions.Logging;
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
    private readonly Mock<ILogger<UpdateGroupCommand>> _loggerMock;

    private readonly List<IUserCredentials> _testUsers;

    private readonly UpdateGroupHandler _updateGroupHandlerSUT;

    public UpdateGroupHandlerTests()
    {
        _groupQueryRepositoryMock = new Mock<IGroupQueryRepository>();
        _groupPersistenceRepositoryMock = new Mock<IGroupPersistenceRepository>();
        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();
        _loggerMock = new Mock<ILogger<UpdateGroupCommand>>();

        _testUsers = new()
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

        _updateGroupHandlerSUT = new(_groupQueryRepositoryMock.Object, _groupPersistenceRepositoryMock.Object, _userQueryRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenGroupMetadataIsUpdatedSuccessfully_ReturnTrue()
    {
        // Given
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(_testUsers[0].Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testUsers[0]);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(_testUsers[1].Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testUsers[1]);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(_testUsers[2].Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testUsers[2]);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(_testUsers[3].Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testUsers[3]);
        Group existingGroup = new("1", "Group", "Group Desc", _testUsers.Select(user => user.Account));

        UpdateGroupCommand command = new(new UpdateGroupContract(
            existingGroup.GroupId,
            "New Group Name",
            "New Group Description",
            _testUsers.Select(user => user.Account.Id).ToList()));

        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupById(existingGroup.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingGroup);

        _groupPersistenceRepositoryMock
            .Setup(repo => repo.UpdateGroup(
                It.Is<Group>(group => group.GroupId == existingGroup.GroupId
                                   && group.GroupName == command.Contract.GroupName
                                   && group.GroupDescription == command.Contract.GroupDescription
                                   && group.Users.Count == _testUsers.Count), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // When
        var testAction = async () => await _updateGroupHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await testAction.Should().NotThrowAsync();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenGroupIsNotUpdatedSuccessfully_ThrowsGroupUpdateException()
    {
        // Given
        Group existingGroup = new("1", "Group", "Group Desc");

        UpdateGroupCommand command = new(new UpdateGroupContract(
            existingGroup.GroupId, 
            "New Group Name", 
            "New Group Description", 
            new List<string>()));

        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupById(existingGroup.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingGroup);

        _groupPersistenceRepositoryMock
            .Setup(repo => repo.UpdateGroup(
                It.Is<Group>(
                    group => group.GroupId == existingGroup.GroupId
                          && group.GroupName == command.Contract.GroupName
                          && group.GroupDescription == command.Contract.GroupDescription
                          && !group.Users.Any()), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // When
        var testAction = async () => await _updateGroupHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<GroupUpdateException>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenGroupSuffersNoChanges_DoesNotThrow()
    {
        // Given
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(_testUsers[0].Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testUsers[0]);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(_testUsers[1].Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testUsers[1]);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(_testUsers[2].Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testUsers[2]);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(_testUsers[3].Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testUsers[3]);
        Group existingGroup = new("1", "Group", "Group Desc", _testUsers.Select(user => user.Account));

        UpdateGroupCommand command = new(new UpdateGroupContract(
            existingGroup.GroupId, 
            existingGroup.GroupName, 
            existingGroup.GroupDescription,
            _testUsers.Select(user => user.Account.Id).ToList()));

        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupById(existingGroup.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingGroup);

        _groupPersistenceRepositoryMock
            .Setup(repo => repo.UpdateGroup(It.IsAny<Group>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // When
        var testAction = async () => await _updateGroupHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await testAction.Should().NotThrowAsync();
    }
    
    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenGroupUsersAreRemoved_UpdateAndDontThrow()
    {
        // Given
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(_testUsers[0].Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testUsers[0]);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(_testUsers[1].Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testUsers[1]);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(_testUsers[2].Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testUsers[2]);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(_testUsers[3].Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testUsers[3]);

        Group existingGroup = new("1", "Group", "Group Desc", _testUsers.Select(user => user.Account));

        UpdateGroupCommand command = new(new UpdateGroupContract(
            existingGroup.GroupId, 
            existingGroup.GroupName, 
            existingGroup.GroupDescription, 
            new List<string>()
            {
                _testUsers[0].Account.Id,
                _testUsers[1].Account.Id,
            }));

        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupById(existingGroup.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingGroup);

        _groupPersistenceRepositoryMock
            .Setup(repo => repo.UpdateGroup(
                It.Is<Group>(
                    group => group.GroupId == existingGroup.GroupId
                          && group.GroupName == existingGroup.GroupName
                          && group.GroupDescription == existingGroup.GroupDescription
                          && group.Users.Count == 2
                          && group.Users.Contains(_testUsers[0].Account)
                          && group.Users.Contains(_testUsers[1].Account)), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // When
        var testAction = async () => await _updateGroupHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await testAction.Should().NotThrowAsync();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenAllGroupUsersAreRemoved_DeleteAndLog()
    {
        // Given
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(_testUsers[0].Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testUsers[0]);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(_testUsers[1].Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testUsers[1]);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(_testUsers[2].Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testUsers[2]);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(_testUsers[3].Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testUsers[3]);

        Group existingGroup = new("1", "Group", "Group Desc", _testUsers.Select(user => user.Account));

        UpdateGroupCommand command = new(new UpdateGroupContract(
            existingGroup.GroupId,
            existingGroup.GroupName,
            existingGroup.GroupDescription,
            new List<string>()));

        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupById(existingGroup.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingGroup);

        _groupPersistenceRepositoryMock
            .Setup(repo => repo.DeleteGroup(It.Is<Group>(
                group => group.GroupId == existingGroup.GroupId
                      && group.GroupName == existingGroup.GroupName
                      && group.GroupDescription == existingGroup.GroupDescription
                      && group.Users.Count == 0)))
            .Returns(true);

        // When
        var testAction = async () => await _updateGroupHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await testAction.Should().NotThrowAsync();
        _loggerMock.Verify(logger =>
            logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString() == $"Group with id {existingGroup.GroupId} was deleted due to all users quitting the group"),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenGroupUsersAreAdded_UpdateAndDontThrow()
    {
        // Given
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(_testUsers[0].Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testUsers[0]);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(_testUsers[1].Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testUsers[1]);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(_testUsers[2].Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testUsers[2]);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(_testUsers[3].Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testUsers[3]);

        Group existingGroup = new("1", "Group", "Group Desc", _testUsers.Take(3).Select(user => user.Account));

        UpdateGroupCommand command = new(new UpdateGroupContract(
            existingGroup.GroupId, 
            existingGroup.GroupName, 
            existingGroup.GroupDescription, 
            new List<string>()
            {
                _testUsers[0].Account.Id,
                _testUsers[1].Account.Id,
                _testUsers[2].Account.Id,
                _testUsers[3].Account.Id,
            }));

        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupById(existingGroup.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingGroup);

        _groupPersistenceRepositoryMock
            .Setup(repo => repo.UpdateGroup(
                It.Is<Group>(
                    group => group.GroupId == existingGroup.GroupId
                          && group.GroupName == existingGroup.GroupName
                          && group.GroupDescription == existingGroup.GroupDescription
                          && group.Users.Count == 4
                          && group.Users.Contains(_testUsers[0].Account)
                          && group.Users.Contains(_testUsers[1].Account)
                          && group.Users.Contains(_testUsers[2].Account)
                          && group.Users.Contains(_testUsers[3].Account)), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // When
        var testAction = async () => await _updateGroupHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await testAction.Should().NotThrowAsync();
    }
    
    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenGroupUsersAreBothAddedAndRemoved_UpdateAndDontThrow()
    {
        // Given
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(_testUsers[0].Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testUsers[0]);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(_testUsers[1].Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testUsers[1]);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(_testUsers[2].Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testUsers[2]);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(_testUsers[3].Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testUsers[3]);

        Group existingGroup = new("1", "Group", "Group Desc", _testUsers.Take(3).Select(user => user.Account));

        UpdateGroupCommand command = new(new UpdateGroupContract(
            existingGroup.GroupId, 
            existingGroup.GroupName, 
            existingGroup.GroupDescription, 
            new List<string>()
            {
                _testUsers[0].Account.Id,
                _testUsers[3].Account.Id,
            }));

        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupById(existingGroup.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingGroup);

        _groupPersistenceRepositoryMock
            .Setup(repo => repo.UpdateGroup(
                It.Is<Group>(
                    group => group.GroupId == existingGroup.GroupId
                          && group.GroupName == existingGroup.GroupName
                          && group.GroupDescription == existingGroup.GroupDescription
                          && group.Users.Count == 2
                          && group.Users.Contains(_testUsers[0].Account)
                          && group.Users.Contains(_testUsers[3].Account)), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // When
        var testAction = async () => await _updateGroupHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await testAction.Should().NotThrowAsync();
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
            .Setup(repo => repo.GetGroupById(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Group?)null);

        // When
        var testAction = async () => await _updateGroupHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<GroupNotFoundException>().WithMessage($"*{command.Contract.GroupId}*");
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
            .Setup(repo => repo.GetUserById(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IUserCredentials?)null);

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
            .Setup(repo => repo.GetGroupById(existingGroup.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingGroup);

        // When
        var testAction = async () => await _updateGroupHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<UserNotFoundException>().WithMessage($"*{testUser.Id}*");
    }
}
