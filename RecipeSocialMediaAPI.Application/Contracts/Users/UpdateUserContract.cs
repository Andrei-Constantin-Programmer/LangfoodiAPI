namespace RecipeSocialMediaAPI.Application.Contracts.Users;

public record UpdateUserContract(
    string Id,
    string? ProfileImageId,
    string? UserName,
    string? Email,
    string? Password
);