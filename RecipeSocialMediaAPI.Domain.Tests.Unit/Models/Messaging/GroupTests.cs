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
        _groupSUT = new Group("GroupId", "GroupName");
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

        _groupSUT = new(_groupSUT.GroupId, _groupSUT.GroupName, new List<IUserAccount>()
        {
            account
        });

        // When
        var result = _groupSUT.AddUser(account);

        // Then
        result.Should().BeFalse();
        _groupSUT.Users.Should().HaveCount(1);
    }
}
