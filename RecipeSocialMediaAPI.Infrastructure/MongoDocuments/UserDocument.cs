using RecipeSocialMediaAPI.Infrastructure.MongoConfiguration;
using System.Diagnostics.CodeAnalysis;

namespace RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

[MongoCollection("User")]
[ExcludeFromCodeCoverage(Justification = "Unnecessary testing on DTO")]
public record UserDocument(
    string Handler,
    string UserName,
    string Email,
    string Password,
    int Role,
    string? ProfileImageId = null,
    DateTimeOffset? AccountCreationDate = null,
    string? Id = null,
    List<string>? PinnedConversationIds = null,
    List<string>? BlockedConnectionIds = null
) : MongoDocument(Id);