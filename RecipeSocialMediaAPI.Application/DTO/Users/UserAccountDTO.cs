namespace RecipeSocialMediaAPI.Application.DTO.Users;

public record UserAccountDTO(string Id, string Handler, string UserName, string? ProfileImageId = null, DateTimeOffset? AccountCreationDate = null);
