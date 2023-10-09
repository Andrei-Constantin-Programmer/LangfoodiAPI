using RecipeSocialMediaAPI.DataAccess.MongoConfiguration;

namespace RecipeSocialMediaAPI.DataAccess.MongoDocuments;

[MongoCollection("User")]
public record UserDocument : MongoDocument
{
    required public string Handler { get; set; }
    required public string UserName { get; set; }
    required public string Email { get; set; }
    required public string Password { get; set; }
    public DateTimeOffset AccountCreationDate { get; set; }
}