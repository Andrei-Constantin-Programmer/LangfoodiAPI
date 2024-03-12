namespace RecipeSocialMediaAPI.Application.Cryptography.Interfaces;

public interface IPasswordCryptoService
{
    bool ArePasswordsTheSame(string decryptedPassword, string encryptedPassword);
    string Encrypt(string password);
}