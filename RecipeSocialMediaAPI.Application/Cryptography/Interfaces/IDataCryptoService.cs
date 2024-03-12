namespace RecipeSocialMediaAPI.Application.Cryptography.Interfaces;

public interface IDataCryptoService
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
}
