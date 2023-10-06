namespace RecipeSocialMediaAPI.Domain.Models.Users;

public interface IUserAccount
{
    string Id { get; }
    string Handler { get; }
    string UserName { get; set; }
    DateTimeOffset AccountCreationDate { get; }
}