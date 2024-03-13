using RecipeSocialMediaAPI.Application.Cryptography.Interfaces;
using BCrypter = BCrypt.Net.BCrypt;

namespace RecipeSocialMediaAPI.Application.Cryptography;

public class PasswordCryptoService : IPasswordCryptoService
{

    public bool ArePasswordsTheSame(string clientPassword, string serverPassword)
    {
        try
        {
            return BCrypter.Verify(clientPassword, serverPassword)
                   || clientPassword == serverPassword;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public string Encrypt(string password)
    {
        return BCrypter.HashPassword(password);
    }
}
