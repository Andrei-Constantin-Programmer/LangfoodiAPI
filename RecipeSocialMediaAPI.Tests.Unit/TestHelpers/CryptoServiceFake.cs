using RecipeSocialMediaAPI.Core.Cryptography.Interfaces;

namespace RecipeSocialMediaAPI.TestInfrastructure.Unit.TestHelpers;

internal class CryptoServiceFake : ICryptoService
{
    private const string ENCRYPTION_PREFIX = "CRYPT_";

    public bool ArePasswordsTheSame(string decryptedPassword, string encryptedPassword)
    {
        return decryptedPassword == encryptedPassword.Remove(0, ENCRYPTION_PREFIX.Length);}

    public string Encrypt(string password)
    {
        return ENCRYPTION_PREFIX + password;
    }
}
