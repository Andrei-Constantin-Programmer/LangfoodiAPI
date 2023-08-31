using MediatR;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Core.Exceptions;
using RecipeSocialMediaAPI.Core.DTO.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using AutoMapper;

namespace RecipeSocialMediaAPI.Core.Handlers.Recipes.Queries;

public record GetRecipeByIdQuery(string Id) : IRequest<RecipeDetailedDTO>;

internal class GetRecipeByIdHandler : IRequestHandler<GetRecipeByIdQuery, RecipeDetailedDTO>
{
    private readonly IMapper _mapper;
    private readonly IRecipeRepository _recipeRepository;

    public GetRecipeByIdHandler(IMapper mapper, IRecipeRepository recipeRepository)
    {
        _mapper = mapper;
        _recipeRepository = recipeRepository;
    }

    public async Task<RecipeDetailedDTO> Handle(GetRecipeByIdQuery request, CancellationToken cancellationToken)
    {
        RecipeAggregate? recipe = _recipeRepository.GetRecipeById(request.Id);

        if (recipe is null)
        {
            throw new RecipeNotFoundException(request.Id);
        }

        return await Task.FromResult(_mapper.Map<RecipeDetailedDTO>(recipe));
    }
}
