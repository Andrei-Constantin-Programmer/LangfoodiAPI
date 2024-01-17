namespace RecipeSocialMediaAPI.Application.DTO.Users;

public record UserDTO(string Id, string Handler, string UserName, string Email, string Password, DateTimeOffset? AccountCreationDate = null);