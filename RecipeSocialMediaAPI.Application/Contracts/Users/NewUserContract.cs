namespace RecipeSocialMediaAPI.Application.Contracts.Users;

public record NewUserContract
{
    required public string Handler { get; set; }
    required public string UserName { get; set; }
    required public string Email { get; set; }
    required public string Password { get; set; }
}
