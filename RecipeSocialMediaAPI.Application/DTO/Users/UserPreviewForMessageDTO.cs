namespace RecipeSocialMediaAPI.Application.DTO.Users;

public record UserPreviewForMessageDto(
    string Id,
    string Username,
    string? ProfileImageId = null
);
