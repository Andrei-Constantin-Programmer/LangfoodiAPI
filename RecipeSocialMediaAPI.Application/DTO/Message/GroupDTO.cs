namespace RecipeSocialMediaAPI.Application.DTO.Message;

public record GroupDTO(string Id, string Name, string Description, List<string> UserIds);
