namespace RecipeSocialMediaAPI.Application.DTO.Users;

public record SuccessfulAuthenticationDTO(UserDTO User, string Token);
