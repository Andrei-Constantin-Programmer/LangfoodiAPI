using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Domain.Tests.Shared;

public class TestUserCredentials : IUserCredentials
{
    required public IUserAccount Account { get; set; }
    required public string Email { get; set; }
    required public string Password { get; set; }
}
