namespace RecipeSocialMediaAPI.Domain.Models.Users;

public class UserAccount : IUserAccount
{
    public string Id { get; }
    public string Handler { get; }
    public string UserName { get; set; }
    public DateTimeOffset AccountCreationDate { get; }

    internal UserAccount(string id, string handler, string username, DateTimeOffset accountCreationDate)
    {
        Id = id;
        Handler = handler;
        UserName = username;
        AccountCreationDate = accountCreationDate;
    }
}
