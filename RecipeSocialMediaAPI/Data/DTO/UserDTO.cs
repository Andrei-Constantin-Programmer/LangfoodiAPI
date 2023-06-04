namespace RecipeSocialMediaAPI.Data.DTO;

public record UserDto
{
    required public string? Id { get; set; }
    required public string UserName { get; set; }
    required public string Email { get; set; }
    required public string Password { get; set; }
}
