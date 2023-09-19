using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using System.Text.RegularExpressions;

namespace RecipeSocialMediaAPI.Domain.Services;
public class RecipeValidationService : IRecipeValidationService
{
    public bool ValidTitle(string title) =>
        new Regex("^(?=.*[a-zA-Z0-9])[a-zA-Z0-9:,.()!?;'\\-\\/ ]{3,100}$", RegexOptions.Compiled)
        .IsMatch(title);
}
