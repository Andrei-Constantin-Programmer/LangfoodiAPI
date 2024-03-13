using RecipeSocialMediaAPI.Application.Cryptography.Interfaces;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.TestHelpers;

public class FakePasswordCryptoService : IPasswordCryptoService
{
    private const string ENCRYPTION_PREFIX = "CRYPT_";

    public bool ArePasswordsTheSame(string decryptedPassword, string encryptedPassword) => 
        decryptedPassword == encryptedPassword[ENCRYPTION_PREFIX.Length..];

    public string Encrypt(string password) => $"{ENCRYPTION_PREFIX}{password}";
}
