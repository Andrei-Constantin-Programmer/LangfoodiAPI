using FluentValidation.TestHelper;
using Moq;
using RecipeSocialMediaAPI.Handlers.Users.Commands;
using RecipeSocialMediaAPI.Tests.Shared.Traits;
using RecipeSocialMediaAPI.Validation.GenericValidators.Interfaces;

namespace RecipeSocialMediaAPI.Tests.Unit.Validators;

public class AddUserValidatorTests
{
    private readonly AddUserCommandValidator _addUserValidatorSUT;
    private readonly Mock<IUserValidationService> _userValidationServiceMock;

    public AddUserValidatorTests() 
    {
        _userValidationServiceMock = new Mock<IUserValidationService>();

        _addUserValidatorSUT = new AddUserCommandValidator(_userValidationServiceMock.Object);
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
            .Setup(service => service.ValidUserName(It.IsAny<string>()))
            .Returns(true)
            .Verifiable();
        _userValidationServiceMock
            .Setup(service => service.ValidEmail(It.IsAny<string>()))
            .Returns(true)
            .Verifiable();
        _userValidationServiceMock
            .Setup(service => service.ValidPassword(It.IsAny<string>()))
            .Returns(true)
            .Verifiable();

        // When
        var validationResult = _addUserValidatorSUT.TestValidate(testCommand);

        // Then
        validationResult.ShouldNotHaveAnyValidationErrors();
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
            .Setup(service => service.ValidUserName(It.IsAny<string>()))
            .Returns(false)
            .Verifiable();
        _userValidationServiceMock
            .Setup(service => service.ValidEmail(It.IsAny<string>()))
            .Returns(false)
            .Verifiable();
        _userValidationServiceMock
            .Setup(service => service.ValidPassword(It.IsAny<string>()))
            .Returns(false)
            .Verifiable();

        // When
        var validationResult = _addUserValidatorSUT.TestValidate(testCommand);

        // Then
        validationResult.ShouldHaveValidationErrorFor(command => command.NewUserContract.UserName);
        validationResult.ShouldHaveValidationErrorFor(command => command.NewUserContract.Email);
        validationResult.ShouldHaveValidationErrorFor(command => command.NewUserContract.Password);
    }
}
