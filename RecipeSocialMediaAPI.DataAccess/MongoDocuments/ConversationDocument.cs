using RecipeSocialMediaAPI.DataAccess.MongoConfiguration;

namespace RecipeSocialMediaAPI.DataAccess.MongoDocuments;

[MongoCollection("Conversation")]
public record ConversationDocument : MongoDocument
{
    public string? Connection { get; set; }
    public string? GroupId { get; set; }
    required public List<string> Messages { get; set; }
}

