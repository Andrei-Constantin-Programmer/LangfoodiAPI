namespace RecipeSocialMediaAPI.Cryptography.Interfaces;

public interface ICryptoService
{
    bool ArePasswordsTheSame(string decryptedPassword, string encryptedPassword);
    string Encrypt(string password);
}