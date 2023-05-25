using System.Text.RegularExpressions;
using MediatR;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Handlers.Users.Querries;
using BCrypter = BCrypt.Net.BCrypt;

namespace RecipeSocialMediaAPI.Services
{
    internal class UserValidationService : IUserValidationService
    {
        private readonly ISender _sender;

        public UserValidationService(ISender sender) 
        {
            _sender = sender;
        }

        public string HashPassword(string password)
        {
            return BCrypter.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hash)
        {
            return BCrypter.Verify(password, hash);
        }

        public async Task<bool> ValidUserAsync(UserDto user)
        {
            return ValidUserName(user.UserName)
                && ValidEmail(user.Email)
                && ValidPassword(user.Password)
                && !await _sender.Send(new CheckEmailExistsQuery(user))
                && !await _sender.Send(new CheckUsernameExistsQuery(user));
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

        private static bool RegexPatternMatch(string pattern, string value)
        {
            Regex rgx = new(pattern, RegexOptions.Compiled);
            return rgx.IsMatch(value);
        }

    }
}
