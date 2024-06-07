using FluentValidation;
using RecipeSocialMediaAPI.Application.Exceptions;
using System.Text.Json;

namespace RecipeSocialMediaAPI.Presentation.Middleware;

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
        catch (HandleAlreadyInUseException ex)
        {
            _logger.LogInformation(ex, "Attempted to add already existing user with handler {Handler}", ex.Handle);
            await HandleExceptionAsync(context, StatusCodes.Status400BadRequest, $"Handler {ex.Handle} already in use");
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
        catch (UnsupportedConnectionStatusException ex)
        {
            _logger.LogInformation(ex, "Attempted to change connection status to unsupported status {UnsupportedStatus}", ex.UnsupportedStatus);
            await HandleExceptionAsync(context, StatusCodes.Status400BadRequest, "Unsupported connection status");
        }
        catch (AttemptedToSendMessageToBlockedConnectionException ex)
        {
            _logger.LogWarning(ex, "Attempted to send a message to a blocked connection");
            await HandleExceptionAsync(context, StatusCodes.Status400BadRequest, "Cannot send message to blocked connection");
        }
        catch (InvalidUserRoleException ex)
        {
            _logger.LogInformation(ex, "Attempted to change user role to role {InvalidRole}", ex.InvalidRole);
            await HandleExceptionAsync(context, StatusCodes.Status400BadRequest, $"Invalid user role {ex.InvalidRole}");
        }
        catch (UserNotFoundException)
        {
            await HandleExceptionAsync(context, StatusCodes.Status404NotFound, "User not found");
        }
        catch (RecipeNotFoundException)
        {
            await HandleExceptionAsync(context, StatusCodes.Status404NotFound, "Recipe not found");
        }
        catch (MessageNotFoundException)
        {
            await HandleExceptionAsync(context, StatusCodes.Status404NotFound, "Message not found");
        }
        catch (GroupNotFoundException)
        {
            await HandleExceptionAsync(context, StatusCodes.Status404NotFound, "Group not found");
        }
        catch (ConnectionNotFoundException)
        {
            await HandleExceptionAsync(context, StatusCodes.Status404NotFound, "Connection not found");
        }
        catch (ConversationNotFoundException)
        {
            await HandleExceptionAsync(context, StatusCodes.Status404NotFound, "Conversation not found");
        }
        catch (OperationCanceledException)
        {
            await HandleExceptionAsync(context, StatusCodes.Status500InternalServerError, "Operation cancelled");
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
