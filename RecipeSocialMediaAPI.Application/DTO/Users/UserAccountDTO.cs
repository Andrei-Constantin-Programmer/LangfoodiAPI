namespace RecipeSocialMediaAPI.Application.DTO.Users;

public record UserAccountDTO(string Id, string Handler, string UserName, DateTimeOffset? AccountCreationDate = null);
