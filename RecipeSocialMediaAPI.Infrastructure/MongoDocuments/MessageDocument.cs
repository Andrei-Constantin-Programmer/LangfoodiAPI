using RecipeSocialMediaAPI.Infrastructure.MongoConfiguration;

namespace RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

[MongoCollection("Message")]
public record MessageDocument(
    string SenderId,
    MessageContentDTO MessageContent,
    List<string> SeenByUserIds,
    DateTimeOffset SentDate,
    DateTimeOffset? LastUpdatedDate = null,
    string? MessageRepliedToId = null,
    string? Id = null
) : MongoDocument(Id);

public record MessageContentDTO(string? Text = null, List<string>? RecipeIds = null, List<string>? ImageURLs = null);