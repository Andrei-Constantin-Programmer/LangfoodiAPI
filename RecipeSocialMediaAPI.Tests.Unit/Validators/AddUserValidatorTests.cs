using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using Moq;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Handlers.Users.Commands;
using RecipeSocialMediaAPI.Services;
using RecipeSocialMediaAPI.Tests.Shared.TestHelpers;

namespace RecipeSocialMediaAPI.Tests.Unit.Validators;

public class AddUserValidatorTests
{
    private readonly AddUserValidator _addUserValidatorSUT;
    private readonly Mock<IUserValidationService> _userValidationServiceMock;

    public AddUserValidatorTests() 
    {
        _userValidationServiceMock = new Mock<IUserValidationService>();

        _addUserValidatorSUT = new AddUserValidator(_userValidationServiceMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, "User")]
    public void AddUserValidation_WhenValidUser_DontThrow()
    {
        // Given
        AddUserCommand testCommand = new(
            new()
            {
                UserName = "testUser",
                Email = "testEmail",
                Password = "password"
            }
        );

        _userValidationServiceMock
            .Setup(service => service.ValidUser(It.IsAny<NewUserDTO>()))
            .Verifiable();

        // When
        var action = () => _addUserValidatorSUT.TestValidate(testCommand);

        // Then
        action.Should().NotThrow();
    }

    [Fact]
    [Trait(Traits.DOMAIN, "User")]
    public void AddUserValidation_WhenInvalidUser_ThrowsValidationException()
    {
        // Given
        AddUserCommand testCommand = new(
            new()
            {
                UserName = "testUser",
                Email = "testEmail",
                Password = "password"
            }
        );

        _userValidationServiceMock
            .Setup(service => service.ValidUser(It.IsAny<NewUserDTO>()))
            .Callback(() => throw new ValidationException("Test exception"));

        // When
        var action = () => _addUserValidatorSUT.TestValidate(testCommand);

        // Then
        action.Should().Throw<ValidationException>();
    }
}
