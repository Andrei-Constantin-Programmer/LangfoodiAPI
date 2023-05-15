namespace RecipeSocialMediaAPI.DTO
{
    public record UserDTO
    {
        public required string UserName { get; init; }
        public required string Email { get; init; }
        public required string Password { get; init; }
    }
}
