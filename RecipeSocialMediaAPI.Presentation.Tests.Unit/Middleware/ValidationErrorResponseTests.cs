using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using RecipeSocialMediaAPI.Presentation.Middleware;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Presentation.Tests.Unit.Middleware;

public class ValidationErrorResponseTests
{
    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public void ValidationErrorResponse_WithNoErrors_ReturnNoErrors()
    {
        // Given
        List<ValidationFailure> errors = new();

        // When
        ValidationErrorResponse validationErrorResponse = new(new ValidationException(errors));

        // Then
        validationErrorResponse.Errors.Should().HaveCount(0);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public void ValidationErrorResponse_WithOneError_ReturnFormattedError()
    {
        // Given
        List<ValidationFailure> errors = new()
        {
            new ValidationFailure("TestProp", "Validation failed", "InvalidValue")
        };

        // When
        ValidationErrorResponse validationErrorResponse = new(new ValidationException(errors));

        // Then
        var formattedErrors = validationErrorResponse.Errors.ToList();

        formattedErrors.Should().HaveCount(1);
        formattedErrors[0].Should().Contain(errors[0].AttemptedValue.ToString());
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public void ValidationErrorResponse_WithMultipleError_ReturnFormattedErrorsWithoutDuplicates()
    {
        // Given
        List<ValidationFailure> errors = new()
        {
            new ValidationFailure("TestProp", "Validation failed", "InvalidValue"),
            new ValidationFailure("TestProp2", "Validation failed", "InvalidValue2"),
            new ValidationFailure("TestProp3", "Validation failed", "InvalidValue3"),
            new ValidationFailure("TestProp3", "Validation failed", "InvalidValue3"),
        };

        // When
        ValidationErrorResponse validationErrorResponse = new(new ValidationException(errors));

        // Then
        var formattedErrors = validationErrorResponse.Errors.ToList();

        formattedErrors.Should().HaveCount(3);
        formattedErrors[0].Should().Contain(errors[0].AttemptedValue.ToString());
        formattedErrors[1].Should().Contain(errors[1].AttemptedValue.ToString());
        formattedErrors[2].Should().Contain(errors[2].AttemptedValue.ToString());
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public void ValidationErrorResponse_Message_MustBeValidationFailed()
    {
        // Given
        List<ValidationFailure> errors = new();

        // When
        ValidationErrorResponse validationErrorResponse = new(new ValidationException(errors));

        // Then
        validationErrorResponse.Message.Should().Be("Validation failed");
    }
}
