using RecipeSocialMediaAPI.Infrastructure.MongoConfiguration;

namespace RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

[MongoCollection("Message")]
public record MessageDocument(
    string SenderId,
    MessageContentDto MessageContent,
    List<string> SeenByUserIds,
    DateTimeOffset SentDate,
    DateTimeOffset? LastUpdatedDate = null,
    string? MessageRepliedToId = null,
    string? Id = null
) : MongoDocument(Id);

public record MessageContentDto(
    string? Text = null,
    List<string>? RecipeIds = null,
    List<string>? ImageURLs = null
);
