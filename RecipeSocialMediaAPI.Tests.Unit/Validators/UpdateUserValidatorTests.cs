using FluentValidation.TestHelper;
using Moq;
using RecipeSocialMediaAPI.Handlers.Users.Commands;
using RecipeSocialMediaAPI.Tests.Shared.TestHelpers;
using RecipeSocialMediaAPI.Validation.GenericValidators.Interfaces;

namespace RecipeSocialMediaAPI.Tests.Unit.Validators;

public class UpdateUserValidatorTests
{
    private readonly UpdateUserValidator _updateUserValidatorSUT;
    private readonly Mock<IUserValidationService> _userValidationServiceMock;

    public UpdateUserValidatorTests()
    {
        _userValidationServiceMock = new Mock<IUserValidationService>();

        _updateUserValidatorSUT = new UpdateUserValidator(_userValidationServiceMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, "User")]
    public void UpdateUserValidation_WhenValidUser_DontThrow()
    {
        // Given
        UpdateUserCommand testCommand = new(
            new()
            {
                Id = "testId",
                UserName = "TestUser",
                Email = "test@mail.com",
                Password = "Test@123"
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
        var validationResult = _updateUserValidatorSUT.TestValidate(testCommand);

        // Then
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    [Trait(Traits.DOMAIN, "User")]
    public void UpdateUserValidation_WhenInvalidUser_ThrowsValidationException()
    {
        // Given
        UpdateUserCommand testCommand = new (
            new()
            {
                Id = "TestId",
                UserName = string.Empty,
                Email = "test.com",
                Password = "test"
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
        var validationResult = _updateUserValidatorSUT.TestValidate(testCommand);

        // Then
        validationResult.ShouldHaveValidationErrorFor(command => command.User.UserName);
        validationResult.ShouldHaveValidationErrorFor(command => command.User.Email);
        validationResult.ShouldHaveValidationErrorFor(command => command.User.Password);
    }
}
