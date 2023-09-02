using AutoMapper;
using FluentValidation;
using MediatR;
using RecipeSocialMediaAPI.Core.Contracts;
using RecipeSocialMediaAPI.Core.Exceptions;
using RecipeSocialMediaAPI.Core.Utilities;
using RecipeSocialMediaAPI.Core.Validation;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

namespace RecipeSocialMediaAPI.Core.Handlers.Recipes.Commands;

public record UpdateRecipeCommand(UpdateRecipeContract UpdateRecipeContract) : IValidatableRequest<bool>;

internal class UpdateRecipeHandler : IRequestHandler<UpdateRecipeCommand, bool>
{
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IRecipeRepository _recipeRepository;

    public UpdateRecipeHandler(IMapper mapper, IRecipeRepository recipeRepository, IDateTimeProvider dateTimeProvider)
    {
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
        _recipeRepository = recipeRepository;
    }

    public async Task<bool> Handle(UpdateRecipeCommand request, CancellationToken cancellationToken)
    {
        RecipeAggregate? existingRecipe = _recipeRepository.GetRecipeById(request.UpdateRecipeContract.Id);
        if (existingRecipe == null)
        {
            throw new RecipeNotFoundException(request.UpdateRecipeContract.Id);
        }

        RecipeAggregate updatedRecipe = new(
            existingRecipe.Id,
            request.UpdateRecipeContract.Title,
            _mapper.Map<Recipe>(request.UpdateRecipeContract),
            request.UpdateRecipeContract.Description,
            existingRecipe.Chef,
            existingRecipe.CreationDate,
            _dateTimeProvider.Now,
            request.UpdateRecipeContract.Labels
        );

        return await Task.FromResult(_recipeRepository.UpdateRecipe(updatedRecipe));
    }
}

public class UpdateRecipeCommandValidator : AbstractValidator<UpdateRecipeCommand>
{
    public UpdateRecipeCommandValidator()
    {
        RuleFor(x => x.UpdateRecipeContract.Ingredients)
            .NotEmpty();

        RuleFor(x => x.UpdateRecipeContract.RecipeSteps)
            .NotEmpty();
    }
}
