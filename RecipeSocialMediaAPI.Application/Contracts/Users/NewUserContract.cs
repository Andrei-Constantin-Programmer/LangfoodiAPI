namespace RecipeSocialMediaAPI.Application.Contracts.Users;

public record NewUserContract(
    string Handler,
    string UserName,
    string Email,
    string Password
);