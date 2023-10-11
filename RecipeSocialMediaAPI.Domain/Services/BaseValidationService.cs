using System.Text.RegularExpressions;

namespace RecipeSocialMediaAPI.Domain.Services;
public class BaseValidationService
{
    protected bool RegexPatternMatch(string pattern, string value) =>
        new Regex(pattern, RegexOptions.Compiled)
        .IsMatch(value);
}
