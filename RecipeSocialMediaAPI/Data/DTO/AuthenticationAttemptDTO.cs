namespace RecipeSocialMediaAPI.Data.DTO;

public record AuthenticationAttemptDTO
{
    public required string UsernameOrEmail { get; set; }
    public required string Password { get; set; }
}
