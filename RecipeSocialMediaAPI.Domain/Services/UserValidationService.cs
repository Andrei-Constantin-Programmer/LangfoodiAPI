using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using System.Text.RegularExpressions;

namespace RecipeSocialMediaAPI.Domain.Services;

public class UserValidationService : IUserValidationService
{
    public bool ValidPassword(string password) =>
        RegexPatternMatch(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[!@#\$&*~]).{8,}$", password);

    public bool ValidEmail(string email) =>
        RegexPatternMatch(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,253}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,253}[a-zA-Z0-9])?)+$", email);

    public bool ValidUserName(string userName) =>
        RegexPatternMatch(@"^[a-zA-Z0-9]{3,}$", userName);

    private static bool RegexPatternMatch(string pattern, string value) =>
        new Regex(pattern, RegexOptions.Compiled)
        .IsMatch(value);
}
