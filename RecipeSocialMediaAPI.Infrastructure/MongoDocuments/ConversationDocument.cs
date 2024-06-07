using RecipeSocialMediaAPI.Infrastructure.MongoConfiguration;
using System.Diagnostics.CodeAnalysis;

namespace RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

[MongoCollection("Conversation")]
[ExcludeFromCodeCoverage(Justification = "Unnecessary testing on DTO")]
public record ConversationDocument(
    List<string> Messages,
    string? ConnectionId = null,
    string? GroupId = null,
    string? Id = null
) : MongoDocument(Id);
