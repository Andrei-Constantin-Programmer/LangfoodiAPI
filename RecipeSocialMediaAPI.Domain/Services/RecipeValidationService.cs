using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using System.Text.RegularExpressions;

namespace RecipeSocialMediaAPI.Domain.Services;
public class RecipeValidationService : BaseValidationService, IRecipeValidationService
{
    public bool ValidTitle(string title) =>
        RegexPatternMatch("^(?=.*[a-zA-Z0-9])[a-zA-Z0-9:,.()!?;'\\-\\/ ]{3,100}$", title);
}
