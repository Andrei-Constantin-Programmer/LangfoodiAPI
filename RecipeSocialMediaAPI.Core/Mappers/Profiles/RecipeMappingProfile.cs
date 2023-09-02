using AutoMapper;
using RecipeSocialMediaAPI.Core.Contracts;
using RecipeSocialMediaAPI.Core.DTO.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

namespace RecipeSocialMediaAPI.Core.Mappers.Profiles;

internal class RecipeMappingProfile : Profile
{
    public RecipeMappingProfile() 
    {
        CreateMap<NewRecipeContract, Recipe>();

        CreateMap<RecipeAggregate, RecipeDetailedDTO>()
            .ForMember(
                dest => dest.Ingredients,
                opt => opt.MapFrom(src => src.Recipe.Ingredients))
            .ForMember(
                dest => dest.RecipeSteps,
                opt => opt.MapFrom(src => src.Recipe.Steps));

        CreateMap<RecipeAggregate, RecipeDTO>()
            .ForMember(
                dest => dest.ChefUsername,
                opt => opt.MapFrom(src => src.Chef.UserName));
    }
}
