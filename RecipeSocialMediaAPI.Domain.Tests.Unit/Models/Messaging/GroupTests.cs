using FluentAssertions;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Domain.Tests.Unit.Models.Messaging;

public class GroupTests
{
    private Group _groupSUT;

    public GroupTests()
    {
        _groupSUT = new Group("GroupId", "GroupName", "Description");
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void AddUser_WhenUserIsNotYetAdded_AddsUserToListAndReturnTrue()
    {
        // Given
        TestUserAccount account = new()
        {
            Id = "UserId",
            Handler = "UserHandler",
            UserName = "UserName",
            AccountCreationDate = new (2023, 10, 10, 18, 0, 0, TimeSpan.Zero)
        };

        // When
        var result = _groupSUT.AddUser(account);

        // Then
        result.Should().BeTrue();
        _groupSUT.Users.Should().HaveCount(1);
        _groupSUT.Users.Should().Contain(account);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void AddUser_WhenUserIsAlreadyAdded_DoNotAddUserToListAndReturnFalse()
    {
        // Given
        TestUserAccount account = new()
        {
            Id = "UserId",
            Handler = "UserHandler",
            UserName = "UserName",
            AccountCreationDate = new (2023, 10, 10, 18, 0, 0, TimeSpan.Zero)
        };

        _groupSUT = new(_groupSUT.GroupId, _groupSUT.GroupName, _groupSUT.GroupDescription, new List<IUserAccount>()
        {
            account
        });

        // When
        var result = _groupSUT.AddUser(account);

        // Then
        result.Should().BeFalse();
        _groupSUT.Users.Should().HaveCount(1);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void RemoveUser_WhenUserExists_RemoveUserAndReturnTrue()
    {
        // Given
        TestUserAccount testAccount = new()
        {
            Id = "UserId",
            Handler = "UserHandler",
            UserName = "UserName",
            AccountCreationDate = new (2023, 10, 10, 18, 0, 0, TimeSpan.Zero)
        };
        TestUserAccount accountToRemove = new()
        {
            Id = "UserId",
            Handler = "UserHandler",
            UserName = "UserName",
            AccountCreationDate = new(2023, 10, 10, 18, 0, 0, TimeSpan.Zero)
        };

        _groupSUT = new(_groupSUT.GroupId, _groupSUT.GroupName, _groupSUT.GroupDescription, new List<IUserAccount>()
        {
            testAccount, accountToRemove
        });

        // When
        var result = _groupSUT.RemoveUser(accountToRemove);

        // Then
        result.Should().BeTrue();
        _groupSUT.Users.Should().HaveCount(1);
        _groupSUT.Users.Should().Contain(testAccount);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void RemoveUser_WhenUserDoesNotExist_DoNotChangeUserListAndReturnFalse()
    {
        // Given
        TestUserAccount testAccount = new()
        {
            Id = "UserId",
            Handler = "UserHandler",
            UserName = "UserName",
            AccountCreationDate = new(2023, 10, 10, 18, 0, 0, TimeSpan.Zero)
        };
        TestUserAccount accountToRemove = new()
        {
            Id = "UserId",
            Handler = "UserHandler",
            UserName = "UserName",
            AccountCreationDate = new(2023, 10, 10, 18, 0, 0, TimeSpan.Zero)
        };

        _groupSUT = new(_groupSUT.GroupId, _groupSUT.GroupName, _groupSUT.GroupDescription, new List<IUserAccount>()
        {
            testAccount
        });

        // When
        var result = _groupSUT.RemoveUser(accountToRemove);

        // Then
        result.Should().BeFalse();
        _groupSUT.Users.Should().HaveCount(1);
        _groupSUT.Users.Should().Contain(testAccount);
    }
}
