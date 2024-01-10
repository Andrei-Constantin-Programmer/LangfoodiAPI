using FluentValidation.TestHelper;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Validators.Messages;

public class UpdateGroupValidatorTests
{
    private readonly UpdateGroupCommandValidator _updateGroupCommandValidatorSUT;

    public UpdateGroupValidatorTests()
    {
        _updateGroupCommandValidatorSUT = new();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void UpdateGroupValidation_WhenValidGroup_DontThrow()
    {
        // Given
        UpdateGroupContract testContract = new("1", "Group", "Group Desc", new());
        UpdateGroupCommand testCommand = new(testContract);

        // When
        var validationResult = _updateGroupCommandValidatorSUT.TestValidate(testCommand);

        // Then
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void UpdateGroupValidation_WhenInvalidGroup_ThrowValidationException()
    {
        // Given
        UpdateGroupContract testContract = new(string.Empty, string.Empty, "Group Desc", new());
        UpdateGroupCommand testCommand = new(testContract);

        // When
        var validationResult = _updateGroupCommandValidatorSUT.TestValidate(testCommand);

        // Then
        validationResult.ShouldHaveValidationErrorFor(command => command.UpdateGroupContract.GroupId);
        validationResult.ShouldHaveValidationErrorFor(command => command.UpdateGroupContract.GroupName);
    }
}
