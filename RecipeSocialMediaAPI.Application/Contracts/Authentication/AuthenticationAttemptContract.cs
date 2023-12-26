namespace RecipeSocialMediaAPI.Application.Contracts.Authentication;

public record AuthenticationAttemptContract
{
    required public string HandlerOrEmail { get; set; }
    required public string Password { get; set; }
}
