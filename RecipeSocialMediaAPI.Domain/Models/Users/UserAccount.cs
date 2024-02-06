namespace RecipeSocialMediaAPI.Domain.Models.Users;

public class UserAccount : IUserAccount
{
    public string Id { get; }
    public string Handler { get; }
    public string UserName { get; set; }
    public string? ProfileImageId { get; set; }
    public DateTimeOffset AccountCreationDate { get; }

    internal UserAccount(string id, string handler, string username, string? profileImageId, DateTimeOffset accountCreationDate)
    {
        Id = id;
        Handler = handler;
        UserName = username;
        ProfileImageId = profileImageId;
        AccountCreationDate = accountCreationDate;
    }
}
