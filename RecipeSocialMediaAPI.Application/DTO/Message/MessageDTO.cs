namespace RecipeSocialMediaAPI.Application.DTO.Message;

public record MessageDTO
{
    required public string Id { get; set; }
    required public string SenderId { get; set; }
    public DateTimeOffset? SentDate { get; set; }
    public DateTimeOffset? UpdatedDate { get; set; }
    public string? RepliedToMessageId { get; set; }
    public string? TextContent { get; set; }
    public List<string>? ImageURLs { get; set; }
    public List<string>? RecipeIds { get; set; }
}
