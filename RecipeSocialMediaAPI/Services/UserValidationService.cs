using System.Text.RegularExpressions;
using RecipeSocialMediaAPI.Data.DTO;
using BCrypter = BCrypt.Net.BCrypt;

namespace RecipeSocialMediaAPI.Services
{
    internal class UserValidationService : IUserValidationService
    {
        public string HashPassword(string password) =>
            BCrypter.HashPassword(password);

        public bool VerifyPassword(string password, string hash) =>
            BCrypter.Verify(password, hash);

        public bool ValidUser(UserDto user) =>
            ValidUserName(user.UserName)
            && ValidEmail(user.Email)
            && ValidPassword(user.Password);

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
}
