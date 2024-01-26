using RecipeSocialMediaAPI.Application.Cryptography.Interfaces;
using System.Runtime.CompilerServices;
using System.Text;
using BCrypter = BCrypt.Net.BCrypt;

namespace RecipeSocialMediaAPI.Application.Cryptography;

public class CryptoService : ICryptoService
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
