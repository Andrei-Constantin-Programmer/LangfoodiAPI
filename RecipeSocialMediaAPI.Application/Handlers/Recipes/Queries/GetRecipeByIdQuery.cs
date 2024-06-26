﻿using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Mappers.Recipes.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

namespace RecipeSocialMediaAPI.Application.Handlers.Recipes.Queries;

public record GetRecipeByIdQuery(string Id) : IRequest<RecipeDetailedDto>;

internal class GetRecipeByIdHandler : IRequestHandler<GetRecipeByIdQuery, RecipeDetailedDto>
{
    private readonly IRecipeMapper _mapper;
    private readonly IRecipeQueryRepository _recipeQueryRepository;

    public GetRecipeByIdHandler(IRecipeMapper mapper, IRecipeQueryRepository recipeQueryRepository)
    {
        _mapper = mapper;
        _recipeQueryRepository = recipeQueryRepository;
    }

    public async Task<RecipeDetailedDto> Handle(GetRecipeByIdQuery request, CancellationToken cancellationToken)
    {
        Recipe recipe = await _recipeQueryRepository.GetRecipeByIdAsync(request.Id, cancellationToken)
            ?? throw new RecipeNotFoundException(request.Id);

        return _mapper.MapRecipeToRecipeDetailedDto(recipe);
    }
}
