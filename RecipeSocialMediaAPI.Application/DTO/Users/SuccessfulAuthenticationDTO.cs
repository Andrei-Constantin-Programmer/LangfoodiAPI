namespace RecipeSocialMediaAPI.Application.DTO.Users;

public record SuccessfulAuthenticationDto(UserDto User, string Token);
