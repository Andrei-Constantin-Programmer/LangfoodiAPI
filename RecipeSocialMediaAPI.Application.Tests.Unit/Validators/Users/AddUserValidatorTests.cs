using FluentValidation.TestHelper;
using Moq;
using RecipeSocialMediaAPI.Application.Contracts.Users;
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
        AddUserCommand testCommand = new(new NewUserContract("testHandler", "testUser", "testEmail", "password"));

        _userValidationServiceMock
            .Setup(service => service.ValidHandler(It.IsAny<string>()))
            .Returns(true);
        _userValidationServiceMock
            .Setup(service => service.ValidUserName(It.IsAny<string>()))
            .Returns(true);
        _userValidationServiceMock
            .Setup(service => service.ValidEmail(It.IsAny<string>()))
            .Returns(true);
        _userValidationServiceMock
            .Setup(service => service.ValidPassword(It.IsAny<string>()))
            .Returns(true);

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
        AddUserCommand testCommand = new(new NewUserContract("testHandler", "testUser", "testEmail", "password"));

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
        validationResult.ShouldHaveValidationErrorFor(command => command.Contract.UserName);
        validationResult.ShouldHaveValidationErrorFor(command => command.Contract.Email);
        validationResult.ShouldHaveValidationErrorFor(command => command.Contract.Password);
    }
}
