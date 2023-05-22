using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using RecipeSocialMediaAPI.Data.DTO;

namespace RecipeSocialMediaAPI.Services
{
    public class ValidationService : IValidationService
    {
        public ValidationService() {}

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        public bool ValidUser(UserDto user, IUserService userService)
        {
            return ValidUserName(user.UserName)
                && ValidEmail(user.Email)
                && ValidPassword(user.Password)
                && !userService.CheckEmailExists(user)
                && !userService.CheckUserNameExists(user);
        }

        private static bool RegexPatternMatch(string pattern, string value)
        {
            Regex rgx = new Regex(pattern, RegexOptions.Compiled);
            return rgx.IsMatch(value);
        }

        public bool ValidPassword(string password)
        {
            return RegexPatternMatch(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[!@#\$&*~]).{8,}$", password);
        }

        public bool ValidEmail(string email)
        {
            return RegexPatternMatch(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,253}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,253}[a-zA-Z0-9])?)+$", email);
        }

        public bool ValidUserName(string userName)
        {
            return RegexPatternMatch(@"^[a-zA-Z0-9]{3,}$", userName);
        }
    }
}
