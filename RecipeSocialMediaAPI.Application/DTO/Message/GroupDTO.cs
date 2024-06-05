namespace RecipeSocialMediaAPI.Application.DTO.Message;

public record GroupDto(string Id, string Name, string Description, List<string> UserIds);
