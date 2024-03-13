using RecipeSocialMediaAPI.Application.Cryptography.Interfaces;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.TestHelpers;

public class FakeDataCryptoService : IDataCryptoService
{
    private const string ENCRYPTION_PREFIX = "ENCRYPTED_";

    public string Encrypt(string plainText) => $"{ENCRYPTION_PREFIX}{plainText}";
    public string Decrypt(string cipherText) => cipherText[ENCRYPTION_PREFIX.Length..];
}
