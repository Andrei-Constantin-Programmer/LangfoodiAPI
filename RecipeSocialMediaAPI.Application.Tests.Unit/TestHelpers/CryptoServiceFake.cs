using RecipeSocialMediaAPI.Application.Cryptography.Interfaces;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.TestHelpers;

public class CryptoServiceFake : ICryptoService
{
    private const string ENCRYPTION_PREFIX = "CRYPT_";

    public bool ArePasswordsTheSame(string decryptedPassword, string encryptedPassword)
    {
        return decryptedPassword == encryptedPassword.Remove(0, ENCRYPTION_PREFIX.Length);
    }

    public string Encrypt(string password)
    {
        return ENCRYPTION_PREFIX + password;
    }
}
