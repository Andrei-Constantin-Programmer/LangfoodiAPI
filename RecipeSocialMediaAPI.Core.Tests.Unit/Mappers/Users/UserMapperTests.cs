using FluentAssertions;
using RecipeSocialMediaAPI.Core.DTO;
using RecipeSocialMediaAPI.Core.Mappers.Users;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Core.Tests.Unit.Mappers.Users;
public class UserMapperTests
{
    private readonly UserMapper _userMapperSUT;

    public UserMapperTests()
    {
        _userMapperSUT = new UserMapper();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void MapUserDtoToUser_GivenUserDto_ReturnUser()
    {
        // Given
        UserDTO testUser = new UserDTO()
        {
            Id = "1",
            UserName = "user",
            Email = "mail",
            Password = "password",
        };

        User expectedResult = new User(testUser.Id, testUser.UserName, testUser.Email, testUser.Password);

        // When
        var result = _userMapperSUT.MapUserDtoToUser(testUser);

        // Then
        result.Should().BeOfType<User>();
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void MapUserToUserDto_GivenUser_ReturnUserDto()
    {
        // Given
        User testUser = new User("1", "user", "mail", "password");

        UserDTO expectedResult = new UserDTO()
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
