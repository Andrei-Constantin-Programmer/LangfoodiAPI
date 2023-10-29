namespace RecipeSocialMediaAPI.Application.DTO.Recipes;

public record ServingSizeDTO
{
    required public double Quantity { get; set; }
    required public  string UnitOfMeasurement { get; set; }
}
