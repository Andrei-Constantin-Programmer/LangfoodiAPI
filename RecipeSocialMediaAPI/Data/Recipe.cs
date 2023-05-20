namespace RecipeSocialMediaAPI.Data
{
    public class Recipe
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Chef { get; set; }

        public DateTimeOffset CreationDate { get; set; }

        public Recipe(string title, string description, string chef, DateTimeOffset creationDate)
        {
            Title = title;
            Description = description;
            Chef = chef;
            CreationDate = creationDate;
        }
    }
}
