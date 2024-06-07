using RecipeSocialMediaAPI.Infrastructure.MongoConfiguration;
using System.Diagnostics.CodeAnalysis;

namespace RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

[MongoCollection("Message")]
[ExcludeFromCodeCoverage(Justification = "Unnecessary testing on DTO")]
public record MessageDocument(
    string SenderId,
    MessageContentDto MessageContent,
    List<string> SeenByUserIds,
    DateTimeOffset SentDate,
    DateTimeOffset? LastUpdatedDate = null,
    string? MessageRepliedToId = null,
    string? Id = null
) : MongoDocument(Id);

[ExcludeFromCodeCoverage(Justification = "Unnecessary testing on DTO")]
public record MessageContentDto(
    string? Text = null,
    List<string>? RecipeIds = null,
    List<string>? ImageURLs = null
);
