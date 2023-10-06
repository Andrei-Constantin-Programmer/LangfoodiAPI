using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Domain.Tests.Shared;

public class TestUserAccount : IUserAccount
{
    required public string Id { get; set; }
    required public string Handler { get; set; }
    required public string UserName { get; set; }
    public DateTimeOffset AccountCreationDate { get; set; }
}
