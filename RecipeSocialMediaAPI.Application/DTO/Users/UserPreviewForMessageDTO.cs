namespace RecipeSocialMediaAPI.Application.DTO.Users;
public record UserPreviewForMessageDTO(
    string Id,
    string Username,
    string? ProfileImageId = null
);
