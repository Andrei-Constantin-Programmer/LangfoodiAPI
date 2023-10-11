namespace RecipeSocialMediaAPI.Domain.Models.Users;

public interface IUserCredentials
{
    IUserAccount Account { get; set; }
    string Email { get; set; }
    string Password { get; set; }
}