using FluentValidation.TestHelper;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Validators.Messages;

public class SendMessageValidatorTests
{
    private readonly SendMessageCommandValidator _sendMessageCommandValidatorSUT;

    public SendMessageValidatorTests()
    {
        _sendMessageCommandValidatorSUT = new();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void SendMessageValidation_WhenValidTextMessage_DontThrow()
    {
        // Given
        SendMessageContract testContract = new("convo1", "sender1", "Text", new(), new(), "repliedTo");
        SendMessageCommand testCommand = new(testContract);

        // When
        var validationResult = _sendMessageCommandValidatorSUT.TestValidate(testCommand);

        // Then
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    [InlineData(true)]
    [InlineData(false)]
    public void SendMessageValidation_WhenValidImageMessage_DontThrow(bool containsText)
    {
        // Given
        SendMessageContract testContract = new("convo1", "sender1", containsText ? "Text" : null, new(), new() { "Image1", "Image2" }, "repliedTo");
        SendMessageCommand testCommand = new(testContract);

        // When
        var validationResult = _sendMessageCommandValidatorSUT.TestValidate(testCommand);

        // Then
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    [InlineData(true)]
    [InlineData(false)]
    public void SendMessageValidation_WhenValidRecipeMessage_DontThrow(bool containsText)
    {
        // Given
        SendMessageContract testContract = new("convo1", "sender1", containsText ? "Text" : null, new() { "Recipe1", "Recipe2" }, new(), "repliedTo");
        SendMessageCommand testCommand = new(testContract);

        // When
        var validationResult = _sendMessageCommandValidatorSUT.TestValidate(testCommand);

        // Then
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void SendMessageValidation_WhenEmptyMessage_ThrowValidationException()
    {
        // Given
        SendMessageContract testContract = new("convo1", "sender1", null, new(), new(), "repliedTo");
        SendMessageCommand testCommand = new(testContract);

        // When
        var validationResult = _sendMessageCommandValidatorSUT.TestValidate(testCommand);

        // Then
        validationResult.ShouldHaveValidationErrorFor(command => command.Contract).WithErrorMessage("Message content must not be empty");
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void SendMessageValidation_WhenMessageContainsBothImagesAndRecipes_ThrowValidationException()
    {
        // Given
        SendMessageContract testContract = new("convo1", "sender1", null, new() { "Recipe1", "Recipe2" }, new() { "Image1", "Image2" }, "repliedTo");
        SendMessageCommand testCommand = new(testContract);

        // When
        var validationResult = _sendMessageCommandValidatorSUT.TestValidate(testCommand);

        // Then
        validationResult.ShouldHaveValidationErrorFor(command => command.Contract).WithErrorMessage("A message cannot contain both images and recipes");
    }
}
