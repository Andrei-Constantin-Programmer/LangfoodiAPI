﻿using FluentValidation;
using MediatR;
using RecipeSocialMediaAPI.Core.Contracts.Recipes;
using RecipeSocialMediaAPI.Core.DTO.Recipes;
using RecipeSocialMediaAPI.Core.Exceptions;
using RecipeSocialMediaAPI.Core.Mappers.Recipes.Interfaces;
using RecipeSocialMediaAPI.Core.Utilities;
using RecipeSocialMediaAPI.Core.Validation;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;

namespace RecipeSocialMediaAPI.Core.Handlers.Recipes.Commands;

public record AddRecipeCommand(NewRecipeContract NewRecipeContract) : IValidatableRequest<RecipeDetailedDTO>;

internal class AddRecipeHandler : IRequestHandler<AddRecipeCommand, RecipeDetailedDTO>
{
    private readonly IRecipeMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IRecipeRepository _recipeRepository;
    private readonly IUserRepository _userRepository;

    public AddRecipeHandler(IRecipeMapper mapper, IUserRepository userRepository, IRecipeRepository recipeRepository, IDateTimeProvider dateTimeProvider)
    {
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
        _recipeRepository = recipeRepository;
        _userRepository = userRepository;
    }

    public async Task<RecipeDetailedDTO> Handle(AddRecipeCommand request, CancellationToken cancellationToken)
    {
        User? chef = _userRepository.GetUserById(request.NewRecipeContract.ChefId);
        if (chef is null)
        {
            throw new UserNotFoundException();
        }

        DateTimeOffset dateOfCreation = _dateTimeProvider.Now;
        RecipeAggregate insertedRecipe = _recipeRepository.CreateRecipe(
            request.NewRecipeContract.Title,
            new Recipe(
                request.NewRecipeContract.Ingredients
                    .Select(_mapper.MapIngredientDtoToIngredient)
                    .ToList(),
                new Stack<RecipeStep>(request.NewRecipeContract.RecipeSteps
                    .Select(_mapper.MapRecipeStepDtoToRecipeStep)),
                request.NewRecipeContract.NumberOfServings,
                request.NewRecipeContract.CookingTime,
                request.NewRecipeContract.KiloCalories
            ),
            request.NewRecipeContract.Description,
            chef,
            request.NewRecipeContract.Labels,
            dateOfCreation,
            dateOfCreation
        );

        return await Task.FromResult(_mapper
            .MapRecipeAggregateToRecipeDetailedDto(insertedRecipe));
    }
}

public class AddRecipeCommandValidator : AbstractValidator<AddRecipeCommand>
{
    private readonly IRecipeValidationService _recipeValidationService;

    public AddRecipeCommandValidator(IRecipeValidationService recipeValidationService)
    {
        _recipeValidationService = recipeValidationService;

        RuleFor(x => x.NewRecipeContract.Title)
            .Must(_recipeValidationService.ValidTitle);

        RuleFor(x => x.NewRecipeContract.NumberOfServings)
            .GreaterThanOrEqualTo(1)
            .When(x => x.NewRecipeContract.NumberOfServings is not null);

        RuleFor(x => x.NewRecipeContract.CookingTime)
            .GreaterThanOrEqualTo(0)
            .When(x => x.NewRecipeContract.CookingTime is not null);

        RuleFor(x => x.NewRecipeContract.KiloCalories)
            .GreaterThanOrEqualTo(0)
            .When(x => x.NewRecipeContract.KiloCalories is not null);

        RuleFor(x => x.NewRecipeContract.Ingredients)
            .NotEmpty();

        RuleFor(x => x.NewRecipeContract.RecipeSteps)
            .NotEmpty();
    }
}