namespace RecipeSocialMediaAPI.Data.DTO
{
    public record RecipeDTO
    {
        public required int Id { get; set; }

        public required string Title { get; set; }

        public required string Description { get; set; }

        public required string Chef { get; set; }

        public DateTimeOffset? CreationDate { get; set; }
    }
}
