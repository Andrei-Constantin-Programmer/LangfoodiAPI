using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using RecipeSocialMediaAPI.Exceptions;
using RecipeSocialMediaAPI.Handlers.Users.Commands;
using RecipeSocialMediaAPI.Middleware;
using RecipeSocialMediaAPI.Tests.Shared.TestHelpers;
using RecipeSocialMediaAPI.Tests.Shared.Traits;

namespace RecipeSocialMediaAPI.Tests.Unit.Middleware;

public class ExceptionMappingMiddlewareTests
{
    private readonly ExceptionMappingMiddleware _exceptionMappingMiddlewareSUT;
    private readonly Mock<ILogger<ExceptionMappingMiddleware>> _loggerMock;
    private readonly Mock<RequestDelegate> _nextMock;

    public ExceptionMappingMiddlewareTests()
    {
        _nextMock = new Mock<RequestDelegate>();
        _loggerMock = new Mock<ILogger<ExceptionMappingMiddleware>>();

        _exceptionMappingMiddlewareSUT = new ExceptionMappingMiddleware(_nextMock.Object, _loggerMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, "Infrastructure")]
    public async Task InvokeAsync_WhenRequestThrowsValidationException_WriteMessageAndSetStatusCodeToBadRequest()
    {
        // Given
        List<ValidationFailure> errors = new()
        {
            new ValidationFailure("TestProp", "Validation failed", "InvalidValue")
        };
        _nextMock
            .Setup(next => next(It.IsAny<HttpContext>()))
            .Throws(new ValidationException(errors))
            .Verifiable();

        HttpContext context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // When
        await _exceptionMappingMiddlewareSUT.InvokeAsync(context);

        // Then
        HttpResponse response = context.Response;
        response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        var responseBody = await HttpContextHelper.GetResponseBodyAsync(context);
        responseBody.Should().Contain(errors[0].AttemptedValue.ToString());
    }

    [Fact]
    [Trait(Traits.DOMAIN, "Infrastructure")]
    public async Task InvokeAsync_WhenRequestThrowsInvalidCredentialsException_SetStatusCodeToBadRequest()
    {
        // Given
        _nextMock
            .Setup(next => next(It.IsAny<HttpContext>()))
            .Throws(new InvalidCredentialsException())
            .Verifiable();

        HttpContext context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // When
        await _exceptionMappingMiddlewareSUT.InvokeAsync(context);

        // Then
        HttpResponse response = context.Response;
        response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    [Trait(Traits.DOMAIN, "Infrastructure")]
    public async Task InvokeAsync_WhenRequestThrowsUsernameAlreadyInUseException_WriteMessageAndSetStatusCodeToBadRequest()
    {
        // Given
        string username = "TestUsername";
        _nextMock
            .Setup(next => next(It.IsAny<HttpContext>()))
            .Throws(new UsernameAlreadyInUseException(username))
            .Verifiable();

        HttpContext context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // When
        await _exceptionMappingMiddlewareSUT.InvokeAsync(context);

        // Then
        HttpResponse response = context.Response;
        response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        var responseBody = await HttpContextHelper.GetResponseBodyAsync(context);
        responseBody.Should().Contain(username);
    }

    [Fact]
    [Trait(Traits.DOMAIN, "Infrastructure")]
    public async Task InvokeAsync_WhenRequestThrowsEmailAlreadyInUseException_WriteMessageAndSetStatusCodeToBadRequest()
    {
        // Given
        string email = "test@mail.com";
        _nextMock
            .Setup(next => next(It.IsAny<HttpContext>()))
            .Throws(new EmailAlreadyInUseException(email))
            .Verifiable();

        HttpContext context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // When
        await _exceptionMappingMiddlewareSUT.InvokeAsync(context);

        // Then
        HttpResponse response = context.Response;
        response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        var responseBody = await HttpContextHelper.GetResponseBodyAsync(context);
        responseBody.Should().Contain(email);
    }

    [Fact]
    [Trait(Traits.DOMAIN, "Infrastructure")]
    public async Task InvokeAsync_WhenRequestThrowsUserNotFoundException_SetStatusCodeToNotFound()
    {
        // Given
        _nextMock
            .Setup(next => next(It.IsAny<HttpContext>()))
            .Throws(new UserNotFoundException())
            .Verifiable();

        HttpContext context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // When
        await _exceptionMappingMiddlewareSUT.InvokeAsync(context);

        // Then
        HttpResponse response = context.Response;
        response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, "Infrastructure")]
    public async Task InvokeAsync_WhenRequestThrowsRecipeNotFoundException_SetStatusCodeToNotFound()
    {
        // Given
        int recipeId = 1;
        _nextMock
            .Setup(next => next(It.IsAny<HttpContext>()))
            .Throws(new RecipeNotFoundException(recipeId))
            .Verifiable();

        HttpContext context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // When
        await _exceptionMappingMiddlewareSUT.InvokeAsync(context);

        // Then
        HttpResponse response = context.Response;
        response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, "Infrastructure")]
    public async Task InvokeAsync_WhenRequestThrowsGenericException_SetStatusCodeToInternalServerError()
    {
        // Given
        _nextMock
            .Setup(next => next(It.IsAny<HttpContext>()))
            .Throws(new Exception())
            .Verifiable();

        HttpContext context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // When
        await _exceptionMappingMiddlewareSUT.InvokeAsync(context);

        // Then
        HttpResponse response = context.Response;
        response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    [Trait(Traits.DOMAIN, "Infrastructure")]
    public async Task InvokeAsync_WhenRequestDoesNotThrow_DoNotWriteMessageOrChangeStatusCodeFromOk()
    {
        // Given
        _nextMock
            .Setup(next => next(It.IsAny<HttpContext>()))
            .Verifiable();

        HttpContext context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // When
        await _exceptionMappingMiddlewareSUT.InvokeAsync(context);

        // Then
        HttpResponse response = context.Response;
        response.StatusCode.Should().Be(StatusCodes.Status200OK);

        var responseBody = await HttpContextHelper.GetResponseBodyAsync(context);
        responseBody.Should().BeNullOrEmpty();
    }
}
