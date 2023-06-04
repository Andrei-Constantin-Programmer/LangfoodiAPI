using RecipeSocialMediaAPI.DAL.MongoConfiguration;

namespace RecipeSocialMediaAPI.DAL.Documents
{
    [MongoCollection("User")]
    public record UserDocument : MongoDocument
    {
        required public string UserName { get; set; }
        required public string Email { get; set; }
        required public string Password { get; set; }
    }
}
