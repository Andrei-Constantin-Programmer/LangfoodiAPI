using AutoMapper;
using RecipeSocialMediaAPI.Core.Contracts;
using RecipeSocialMediaAPI.Core.DTO.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using System.Collections.Immutable;

namespace RecipeSocialMediaAPI.Core.Mappers.Profiles;

internal class RecipeMappingProfile : Profile
{
    public RecipeMappingProfile()
    {
        CreateMap<RecipeAggregate, RecipeDetailedDTO>()
            .ForMember(
                dest => dest.Ingredients,
                opt => opt.MapFrom(src => ImmutableList.CreateRange(src.Recipe.Ingredients.Select(
                    x => new IngredientDTO()
                    {
                        Name = x.Name,
                        Quantity = x.Quantity,
                        UnitOfMeasurement = x.UnitOfMeasurement
                    }))))
            .ForMember(
                dest => dest.RecipeSteps,
                opt => opt.MapFrom(src => ImmutableStack.CreateRange(src.Recipe.Steps.Select(
                    x => new RecipeStepDTO()
                    {
                        Text = x.Text,
                        ImageUrl = x.Image.ImageUrl
                    }))));

        CreateMap<RecipeAggregate, RecipeDTO>()
            .ForMember(
                dest => dest.ChefUsername,
                opt => opt.MapFrom(src => src.Chef.UserName));
    }
}
