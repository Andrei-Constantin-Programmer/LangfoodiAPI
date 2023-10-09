using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Application.Mappers.Users;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Mappers.Users;

public class UserMapperTests
{
    private readonly UserMapper _userMapperSUT;

    private readonly Mock<IUserFactory> _userFactoryMock;

    public UserMapperTests()
    {
        _userFactoryMock = new Mock<IUserFactory>();

        _userMapperSUT = new UserMapper(_userFactoryMock.Object);
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
            Handler = "handler",
            UserName = "user",
            Email = "mail",
            Password = "password",
        };

        IUserCredentials expectedResult = new TestUserCredentials
        {
            Account = new TestUserAccount
            {
                Id = "1",
                Handler = "handler",
                UserName = "user",
                AccountCreationDate = new(2023, 10, 9, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "mail",
            Password = "password"
        };

        _userFactoryMock
            .Setup(factory => factory
                .CreateUserCredentials(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>()))
            .Returns(expectedResult);

        // When
        var result = _userMapperSUT.MapUserDtoToUser(testUser);

        // Then
        result.Should().Be(expectedResult);
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
                AccountCreationDate = new(2023, 10, 9, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "mail",
            Password = "password"
        };

        UserDTO expectedResult = new()
        {
            Id = testUser.Account.Id,
            Handler = testUser.Account.Handler,
            UserName = testUser.Account.UserName,
            Email = testUser.Email,
            Password = testUser.Password,
            AccountCreationDate = testUser.Account.AccountCreationDate
        };

        // When
        var result = _userMapperSUT.MapUserToUserDto(testUser);

        // Then
        result.Should().Be(expectedResult);
    }
}
