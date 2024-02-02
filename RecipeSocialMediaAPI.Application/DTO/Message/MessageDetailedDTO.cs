using RecipeSocialMediaAPI.Application.DTO.Recipes;

namespace RecipeSocialMediaAPI.Application.DTO.Message;

public record MessageDetailedDTO
{
    required public string Id { get; set; }
    required public string SenderId { get; set; }
    required public string SenderName { get; set; }
    public DateTimeOffset? SentDate { get; set; }
    public DateTimeOffset? UpdatedDate { get; set; }
    public MessageDetailedDTO? RepliedToMessage { get; set; }
    public string? TextContent { get; set; }
    public List<string>? ImageURLs { get; set; }
    public List<RecipeDTO>? Recipes { get; set; }
}
