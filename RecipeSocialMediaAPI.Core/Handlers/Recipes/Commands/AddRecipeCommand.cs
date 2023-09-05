using AutoMapper;
using FluentValidation;
using MediatR;
using RecipeSocialMediaAPI.Core.Contracts.Recipes;
using RecipeSocialMediaAPI.Core.DTO.Recipes;
using RecipeSocialMediaAPI.Core.Exceptions;
using RecipeSocialMediaAPI.Core.Mappers.Interfaces;
using RecipeSocialMediaAPI.Core.Utilities;
using RecipeSocialMediaAPI.Core.Validation;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Domain.Mappers.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Core.Handlers.Recipes.Commands;

public record AddRecipeCommand(NewRecipeContract NewRecipeContract) : IValidatableRequest<RecipeDetailedDTO>;

internal class AddRecipeHandler : IRequestHandler<AddRecipeCommand, RecipeDetailedDTO>
{
    private readonly IRecipeAggregateToRecipeDetailedDtoMapper _aggregateMapper;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IRecipeRepository _recipeRepository;
    private readonly IUserRepository _userRepository;

    public AddRecipeHandler(IRecipeAggregateToRecipeDetailedDtoMapper aggregateMapper, IUserRepository userRepository, IRecipeRepository recipeRepository, IDateTimeProvider dateTimeProvider)
    {
        _aggregateMapper = aggregateMapper;
        _dateTimeProvider = dateTimeProvider;
        _recipeRepository = recipeRepository;
        _userRepository = userRepository;
    }

    public async Task<RecipeDetailedDTO> Handle(AddRecipeCommand request, CancellationToken cancellationToken)
    {
        User? queriedUser = _userRepository.GetUserById(request.NewRecipeContract.ChefId);
        if (queriedUser is null)
        {
            throw new UserNotFoundException();
        }

        DateTimeOffset dateOfCreation = _dateTimeProvider.Now;
        RecipeAggregate insertedRecipe = _recipeRepository.CreateRecipe(
            request.NewRecipeContract.Title,
            _contractMapper.MapNewRecipeContractToRecipe(request.NewRecipeContract),
            request.NewRecipeContract.Description,
            queriedUser,
            request.NewRecipeContract.Labels,
            request.NewRecipeContract.NumberOfServings,
            request.NewRecipeContract.CookingTime,
            request.NewRecipeContract.KiloCalories,
            dateOfCreation,
            dateOfCreation
        );

        return await Task.FromResult(_aggregateMapper.MapRecipeAggregateToRecipeDetailedDto(insertedRecipe));
    }
}

public class AddRecipeCommandValidator : AbstractValidator<AddRecipeCommand>
{
    public AddRecipeCommandValidator()
    {
        RuleFor(x => x.NewRecipeContract.Ingredients)
            .NotEmpty();

        RuleFor(x => x.NewRecipeContract.RecipeSteps)
            .NotEmpty();
    }
}