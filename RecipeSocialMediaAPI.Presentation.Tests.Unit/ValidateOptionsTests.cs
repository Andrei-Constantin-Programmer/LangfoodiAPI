using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RecipeSocialMediaAPI.Presentation.OptionValidation;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Presentation.Tests.Unit;

public class ValidateOptionsTests
{
    private readonly ValidateOptions<TestOptions> _validateOptionsSUT;

    private readonly Mock<IServiceProvider> _serviceProviderMock;

    public ValidateOptionsTests()
    {
        _serviceProviderMock = new Mock<IServiceProvider>();
        _serviceProviderMock
            .Setup(_serviceProviderMock => _serviceProviderMock.GetService(typeof(IValidator<TestOptions>)))
            .Returns(new TestOptionsValidator());

        var serviceScopeMock = new Mock<IServiceScope>();
        serviceScopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);
        var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
        serviceScopeFactoryMock
            .Setup(x => x.CreateScope())
            .Returns(serviceScopeMock.Object);

        _serviceProviderMock
            .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
            .Returns(serviceScopeFactoryMock.Object);

        _validateOptionsSUT = new ValidateOptions<TestOptions>(_serviceProviderMock.Object, null);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    [InlineData("")]
    [InlineData("OtherOptions")]
    public void Validate_WhenOptionsNameDoesNotMatch_ReturnsSkip(string optionsName)
    {
        // Given
        var options = new TestOptions
        {
            Name = "Test"
        };

        // When
        var result = _validateOptionsSUT.Validate(optionsName, options);

        // Then
        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public void Validate_WhenOptionsAreNull_ThrowArgumentNullException()
    {
        // Given

        // When
        var testAction = () => _validateOptionsSUT.Validate(nameof(TestOptions), null!);

        // Then
        testAction.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public void Validate_WhenOptionsAreInvalid_ReturnFailure()
    {
        // Given
        var options = new TestOptions
        {
            Name = string.Empty
        };

        // When
        var result = _validateOptionsSUT.Validate(nameof(TestOptions), options);

        // Then
        result.Succeeded.Should().BeFalse();
        result.FailureMessage.Should().Be("Validation failed for TestOptions.Name with error: The Name cannot be empty.");
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public void Validate_WhenOptionsAreValid_ReturnsSuccess()
    {
        // Given
        var options = new TestOptions
        {
            Name = "Test"
        };

        // When
        var result = _validateOptionsSUT.Validate(nameof(TestOptions), options);

        // Then
        result.Succeeded.Should().BeTrue();
    }

    private class TestOptions
    {
        public string? Name { get; set; }
    }

    private sealed class TestOptionsValidator : AbstractValidator<TestOptions>
    {
        public TestOptionsValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("The Name cannot be empty.");
        }
    }
}
