using FluentAssertions;
using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Application.Mappers.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Collections.Immutable;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Mappers.Users;

public class UserMapperTests
{
    private readonly UserMapper _userMapperSUT;

    public UserMapperTests()
    {
        _userMapperSUT = new UserMapper();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void MapUserToUserDto_GivenUser_ReturnUserDto()
    {
        // Given
        IUserCredentials testUser = new TestUserCredentials
        {
            Account = new TestUserAccount
            {
                Id = "1",
                Handler = "handler",
                UserName = "user",
                AccountCreationDate = new(2023, 10, 9, 0, 0, 0, TimeSpan.Zero),
                ProfileImageId = "img.png",
                PinnedConversationIds = (new List<string> { "convo1", "convo2" }).ToImmutableList()
            },
            Email = "mail",
            Password = "password"
        };

        UserDTO expectedResult = new(
            Id: testUser.Account.Id,
            Handler: testUser.Account.Handler,
            UserName: testUser.Account.UserName,
            Email: testUser.Email,
            Password: testUser.Password,
            AccountCreationDate: testUser.Account.AccountCreationDate,
            ProfileImageId: testUser.Account.ProfileImageId,
            PinnedConversationIds: testUser.Account.PinnedConversationIds.ToList()
        );

        // When
        var result = _userMapperSUT.MapUserToUserDto(testUser);

        // Then
        result.Id.Should().Be(expectedResult.Id);
        result.Handler.Should().Be(expectedResult.Handler);
        result.UserName.Should().Be(expectedResult.UserName);
        result.Email.Should().Be(expectedResult.Email);
        result.Password.Should().Be(expectedResult.Password);
        result.AccountCreationDate.Should().Be(expectedResult.AccountCreationDate);
        result.ProfileImageId.Should().Be(expectedResult.ProfileImageId);
        result.PinnedConversationIds.Should().BeEquivalentTo(expectedResult.PinnedConversationIds);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void MapUserAccountToUserAccountDto_GivenUser_ReturnUserAccountDto()
    {
        // Given
        IUserAccount testUser = new TestUserAccount
        {
            Id = "1",
            Handler = "handler",
            UserName = "user",
            AccountCreationDate = new(2023, 10, 9, 0, 0, 0, TimeSpan.Zero),
            ProfileImageId = "img.png",
            PinnedConversationIds = (new List<string> { "convo1", "convo2" }).ToImmutableList()
        };

        UserAccountDTO expectedResult = new(
            Id: testUser.Id,
            Handler: testUser.Handler,
            UserName: testUser.UserName,
            AccountCreationDate: testUser.AccountCreationDate,
            ProfileImageId: testUser.ProfileImageId,
            PinnedConversationIds: testUser.PinnedConversationIds.ToList()
        );

        // When
        var result = _userMapperSUT.MapUserAccountToUserAccountDto(testUser);

        // Then
        result.Id.Should().Be(expectedResult.Id);
        result.Handler.Should().Be(expectedResult.Handler);
        result.UserName.Should().Be(expectedResult.UserName);
        result.AccountCreationDate.Should().Be(expectedResult.AccountCreationDate);
        result.ProfileImageId.Should().Be(expectedResult.ProfileImageId);
        result.PinnedConversationIds.Should().BeEquivalentTo(expectedResult.PinnedConversationIds);
    }
}
