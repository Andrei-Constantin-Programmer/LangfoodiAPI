namespace RecipeSocialMediaAPI.Domain;

public class User
{
    public string Id { get; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public User(string id, string username, string email, string password)
    {
        Id = id;
        UserName = username;
        Email = email;
        Password = password;
    }
}
