using FluentValidation.TestHelper;
using RecipeSocialMediaAPI.Application.Handlers.Images.Commands;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Validators.Images;

public class RemoveMultipleImagesValidatorTests
{
    private readonly RemoveMultipleImagesValidator _removeMultipleImagesValidatorSUT;

    public RemoveMultipleImagesValidatorTests()
    {
        _removeMultipleImagesValidatorSUT = new RemoveMultipleImagesValidator();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void RemoveMultipleImagesValidation_WhenPublicIdsIsPopulated_DontThrow()
    {
        // Given
        RemoveMultipleImagesCommand testCommand = new(new() { "id1", "id2", "id3" });

        // When
        var validationResult = _removeMultipleImagesValidatorSUT.TestValidate(testCommand);

        // Then
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void RemoveMultipleImagesValidation_WhenPublicIdsListEmpty_Throw()
    {
        // Given
        RemoveMultipleImagesCommand testCommand = new(new());

        // When
        var validationResult = _removeMultipleImagesValidatorSUT.TestValidate(testCommand);

        // Then
        validationResult.ShouldHaveValidationErrorFor(command => command.PublicIds);
    }
}
