using RecipeSocialMediaAPI.Cryptography.Interfaces;
using BCrypter = BCrypt.Net.BCrypt;

namespace RecipeSocialMediaAPI.Cryptography;

public class CryptoService : ICryptoService
{
    public bool ArePasswordsTheSame(string decryptedPassword, string encryptedPassword)
    {
        try
        {
            return BCrypter.Verify(decryptedPassword, encryptedPassword);
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
