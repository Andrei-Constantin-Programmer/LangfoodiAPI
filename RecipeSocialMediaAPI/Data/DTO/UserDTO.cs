namespace RecipeSocialMediaAPI.Data.DTO;

public record UserDto
{
    public required string? Id { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}
