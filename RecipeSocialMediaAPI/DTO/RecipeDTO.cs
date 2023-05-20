namespace RecipeSocialMediaAPI.DTO
{
    public class RecipeDTO
    {
        public required string Title { get; set; }

        public required string Description { get; set; }

        public required string Chef { get; set; }

        public DateTimeOffset? CreationDate { get; set; }
    }
}
