using RecipeSocialMediaAPI.Infrastructure.MongoConfiguration;

namespace RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

[MongoCollection("Conversation")]
public record ConversationDocument(
    List<string> Messages,
    string? ConnectionId = null,
    string? GroupId = null,
    string? Id = null
) : MongoDocument(Id);
