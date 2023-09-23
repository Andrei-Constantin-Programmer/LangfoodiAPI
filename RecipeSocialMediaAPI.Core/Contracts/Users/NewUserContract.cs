namespace RecipeSocialMediaAPI.Core.Contracts.Users;

public record NewUserContract
{
    required public string UserName { get; set; }
    required public string Email { get; set; }
    required public string Password { get; set; }
}
