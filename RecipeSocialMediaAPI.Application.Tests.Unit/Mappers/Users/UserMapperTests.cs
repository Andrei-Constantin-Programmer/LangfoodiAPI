using FluentAssertions;
using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Application.Mappers.Users;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.TestInfrastructure;

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
    public void MapUserDtoToUser_GivenUserDto_ReturnUser()
    {
        // Given
        UserDTO testUser = new()
        {
            Id = "1",
            UserName = "user",
            Email = "mail",
            Password = "password",
        };

        UserCredentials expectedResult = new(testUser.Id, testUser.UserName, testUser.Email, testUser.Password);

        // When
        var result = _userMapperSUT.MapUserDtoToUser(testUser);

        // Then
        result.Should().BeOfType<UserCredentials>();
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void MapUserToUserDto_GivenUser_ReturnUserDto()
    {
        // Given
        UserCredentials testUser = new("1", "user", "mail", "password");

        UserDTO expectedResult = new()
        {
            Id = testUser.Id,
            UserName = testUser.UserName,
            Email = testUser.Email,
            Password = testUser.Password,
        };

        // When
        var result = _userMapperSUT.MapUserToUserDto(testUser);

        // Then
        result.Should().BeOfType<UserDTO>();
        result.Should().BeEquivalentTo(expectedResult);
    }
}
