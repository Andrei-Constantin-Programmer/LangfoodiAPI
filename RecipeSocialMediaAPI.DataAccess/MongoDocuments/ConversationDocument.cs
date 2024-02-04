using RecipeSocialMediaAPI.DataAccess.MongoConfiguration;

namespace RecipeSocialMediaAPI.DataAccess.MongoDocuments;

[MongoCollection("Conversation")]
public record ConversationDocument(List<string> Messages, string? ConnectionId = null, string? GroupId = null, string? Id = null) : MongoDocument(Id);
