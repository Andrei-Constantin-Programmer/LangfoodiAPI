using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Core.Mappers.Recipes.Interfaces;
using RecipeSocialMediaAPI.Domain.Mappers.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

namespace RecipeSocialMediaAPI.Core.Mappers.Recipes;

public class RecipeMapper : IRecipeMapper
{
    private readonly IUserMapper _userMapper;

    public RecipeMapper(IUserMapper userMapper)
    {
        _userMapper = userMapper;
    }

    public Ingredient MapIngredientDtoToIngredient(IngredientDTO ingredientDTO)
    {
        return new(ingredientDTO.Name, ingredientDTO.Quantity, ingredientDTO.UnitOfMeasurement);
    }

    public IngredientDTO MapIngredientToIngredientDto(Ingredient ingredient)
    {
        return new IngredientDTO
        {
            Name = ingredient.Name,
            Quantity = ingredient.Quantity,
            UnitOfMeasurement = ingredient.UnitOfMeasurement
        };
    }

    public RecipeStep MapRecipeStepDtoToRecipeStep(RecipeStepDTO recipeStepDTO)
    {
        return new(recipeStepDTO.Text, recipeStepDTO.ImageUrl is null ? null : new RecipeImage(recipeStepDTO.ImageUrl));
    }

    public RecipeStepDTO MapRecipeStepToRecipeStepDto(RecipeStep recipeStep)
    {
        return new RecipeStepDTO()
        {
            Text = recipeStep.Text,
            ImageUrl = recipeStep.Image?.ImageUrl
        };
    }

    public RecipeDetailedDTO MapRecipeAggregateToRecipeDetailedDto(RecipeAggregate recipeAggregate)
    {
        return new RecipeDetailedDTO()
        {
            Id = recipeAggregate.Id,
            Title = recipeAggregate.Title,
            Description = recipeAggregate.Description,
            Chef = _userMapper.MapUserToUserDto(recipeAggregate.Chef),
            Labels = recipeAggregate.Labels,
            Ingredients = recipeAggregate.Recipe.Ingredients
                .Select(MapIngredientToIngredientDto)
                .ToList(),
            RecipeSteps = new Stack<RecipeStepDTO>(recipeAggregate.Recipe.Steps
                .Select(MapRecipeStepToRecipeStepDto)),
            KiloCalories = recipeAggregate.Recipe.KiloCalories,
            NumberOfServings = recipeAggregate.Recipe.NumberOfServings,
            CookingTime = recipeAggregate.Recipe.CookingTimeInSeconds,
            CreationDate = recipeAggregate.CreationDate,
            LastUpdatedDate = recipeAggregate.LastUpdatedDate,
        };
    }

    public RecipeDTO MapRecipeAggregateToRecipeDto(RecipeAggregate recipeAggregate)
    {
        return new RecipeDTO()
        {
            Id = recipeAggregate.Id,
            Title = recipeAggregate.Title,
            Description = recipeAggregate.Description,
            ChefUsername = recipeAggregate.Chef.UserName,
            Labels = recipeAggregate.Labels,
            KiloCalories = recipeAggregate.Recipe.KiloCalories,
            NumberOfServings = recipeAggregate.Recipe.NumberOfServings,
            CookingTime = recipeAggregate.Recipe.CookingTimeInSeconds,
            CreationDate = recipeAggregate.CreationDate,
        };
    }
}
