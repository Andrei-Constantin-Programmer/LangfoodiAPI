namespace RecipeSocialMediaAPI.Application.Contracts.Authentication;

public record AuthenticationAttemptContract(string HandlerOrEmail, string Password);