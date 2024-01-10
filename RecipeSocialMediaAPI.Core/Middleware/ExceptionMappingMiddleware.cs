using FluentValidation;
using RecipeSocialMediaAPI.Application.Exceptions;
using System.Text.Json;

namespace RecipeSocialMediaAPI.Core.Middleware;

public class ExceptionMappingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMappingMiddleware> _logger;

    public ExceptionMappingMiddleware(RequestDelegate next, ILogger<ExceptionMappingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogInformation(ex, "Validation failed: {ValidationErrorMessage}", ex.Message);
            await HandleExceptionAsync(context, StatusCodes.Status400BadRequest, new ValidationErrorResponse(ex));
        }
        catch (InvalidCredentialsException)
        {
            await HandleExceptionAsync(context, StatusCodes.Status400BadRequest, "Invalid credentials");
        }
        catch(HandlerAlreadyInUseException ex)
        {
            _logger.LogInformation(ex, "Attempted to add already existing user with handler {Handler}", ex.Handler);
            await HandleExceptionAsync(context, StatusCodes.Status400BadRequest, $"Handler {ex.Handler} already in use");
        }
        catch (UsernameAlreadyInUseException ex)
        {
            _logger.LogInformation(ex, "Attempted to add already existing user with username {Username}", ex.Username);
            await HandleExceptionAsync(context, StatusCodes.Status400BadRequest, $"Username {ex.Username} already in use");
        }
        catch (EmailAlreadyInUseException ex)
        {
            _logger.LogInformation(ex, "Attempted to add already existing user with email {Email}", ex.Email);
            await HandleExceptionAsync(context, StatusCodes.Status400BadRequest, $"Email {ex.Email} already in use");
        }
        catch (UserNotFoundException)
        {
            await HandleExceptionAsync(context, StatusCodes.Status404NotFound, "User not found");
        }
        catch (RecipeNotFoundException)
        {
            await HandleExceptionAsync(context, StatusCodes.Status404NotFound, "Recipe not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Internal error: {ErrorMessage}", ex.Message);
            await HandleExceptionAsync(context, StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, int statusCode, string? message = null)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        if (message is not null)
        {
            await context.Response.WriteAsync(message);
        }
    }

    private static async Task HandleExceptionAsync<TJsonResponse>(HttpContext context, int statusCode, TJsonResponse jsonResponse)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        await context.Response
            .WriteAsJsonAsync(jsonResponse, options: new JsonSerializerOptions() { WriteIndented = true });
    }
}

public record ValidationErrorResponse
{
    public string Message { get; } = "Validation failed";
    public IEnumerable<string> Errors { get; set; }

    public ValidationErrorResponse(ValidationException validationException)
        : this(validationException.Errors
              .Select(error => $"Invalid {GetFormattedPropertyName(error.PropertyName)} with value '{error.AttemptedValue}'.").Distinct())
    { }

    private ValidationErrorResponse(IEnumerable<string> errors)
    {
        Errors = errors;
    }

    private static string GetFormattedPropertyName(string propertyName)
    {
        const int containsPeriod = -1;
        int lastPeriodIndex = propertyName.LastIndexOf('.');

        return
            lastPeriodIndex == containsPeriod
            ? propertyName
            : propertyName[(lastPeriodIndex + 1)..];
    }
}
