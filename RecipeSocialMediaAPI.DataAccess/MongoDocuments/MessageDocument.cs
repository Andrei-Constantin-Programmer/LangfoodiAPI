using RecipeSocialMediaAPI.DataAccess.MongoConfiguration;

namespace RecipeSocialMediaAPI.DataAccess.MongoDocuments;

[MongoCollection("Message")]
public record MessageDocument : MongoDocument
{
    required public string SenderId { get; set; }
    required public MessageContentDTO MessageContent { get; set; }
    required public DateTimeOffset SentDate { get; set; }
    public DateTimeOffset? LastUpdatedDate { get; set; } 
    public string? MessageRepliedToId { get; set; }
}

public record MessageContentDTO (string? Text = null, List<string>? RecipeIds = null, List<string>? ImageURLs = null);