using FluentValidation.TestHelper;
using Moq;
using RecipeSocialMediaAPI.Application.Handlers.Users.Commands;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Validators.Users;

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
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void AddUserValidation_WhenValidUser_DontThrow()
    {
        // Given
        AddUserCommand testCommand = new(
            new()
            {
                Handler = "testHandler",
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
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void AddUserValidation_WhenInvalidUser_ThrowsValidationException()
    {
        // Given
        AddUserCommand testCommand = new(
            new()
            {
                Handler = "testHandler",
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
