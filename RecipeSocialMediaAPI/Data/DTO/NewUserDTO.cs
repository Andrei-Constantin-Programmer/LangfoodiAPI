namespace RecipeSocialMediaAPI.Data.DTO;

public record NewUserDTO
{
    required public string UserName { get; set; }
    required public string Email { get; set; }
    required public string Password { get; set; }
}
