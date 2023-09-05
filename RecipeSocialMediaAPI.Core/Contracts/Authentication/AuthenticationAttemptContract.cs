namespace RecipeSocialMediaAPI.Core.Contracts.Authentication;

public record AuthenticationAttemptContract
{
    required public string UsernameOrEmail { get; set; }
    required public string Password { get; set; }
}
