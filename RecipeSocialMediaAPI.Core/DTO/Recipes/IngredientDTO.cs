namespace RecipeSocialMediaAPI.Core.DTO.Recipes;

public record IngredientDTO
{
    required public string Name { get; set; }
    required public double Quantity { get; set; }
    required public string UnitOfMeasurement { get; set; }
}
