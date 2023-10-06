namespace RecipeSocialMediaAPI.Domain.Models.Users;

public class UserCredentials : IUserCredentials
{
    public IUserAccount Account { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    internal UserCredentials(IUserAccount account, string email, string password)
    {
        Account = account;
        Email = email;
        Password = password;
    }
}
