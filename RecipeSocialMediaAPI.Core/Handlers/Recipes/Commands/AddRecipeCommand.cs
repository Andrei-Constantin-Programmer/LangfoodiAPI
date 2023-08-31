using MediatR;
using RecipeSocialMediaAPI.Domain.Models;
using RecipeSocialMediaAPI.Core.DTO;
using RecipeSocialMediaAPI.Core.Utilities;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Core.Contracts;
using RecipeSocialMediaAPI.Core.Validation;
using RecipeSocialMediaAPI.Core.DTO.Recipes;
using RecipeSocialMediaAPI.Core.Exceptions;
using FluentValidation;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using AutoMapper;

namespace RecipeSocialMediaAPI.Core.Handlers.Recipes.Commands;

public record AddRecipeCommand(NewRecipeContract NewRecipeContract) : IValidatableRequest<RecipeDetailedDTO>;

internal class AddRecipeHandler : IRequestHandler<AddRecipeCommand, RecipeDetailedDTO>
{
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IRecipeRepository _recipeRepository;
    private readonly IUserRepository _userRepository;

    public AddRecipeHandler(IMapper mapper, IUserRepository userRepository, IRecipeRepository recipeRepository, IDateTimeProvider dateTimeProvider)
    {
        _mapper = mapper;
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
            _mapper.Map<Recipe>(request.NewRecipeContract),
            request.NewRecipeContract.Description,
            queriedUser,
            request.NewRecipeContract.Labels,
            request.NewRecipeContract.NumberOfServings,
            request.NewRecipeContract.CookingTime,
            request.NewRecipeContract.KiloCalories,
            dateOfCreation,
            dateOfCreation
        );

        return await Task.FromResult(_mapper.Map<RecipeDetailedDTO>(insertedRecipe));
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