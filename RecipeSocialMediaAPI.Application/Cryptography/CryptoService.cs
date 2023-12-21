using RecipeSocialMediaAPI.Application.Cryptography.Interfaces;
using System.Runtime.CompilerServices;
using System.Text;
using BCrypter = BCrypt.Net.BCrypt;

namespace RecipeSocialMediaAPI.Application.Cryptography;

public class CryptoService : ICryptoService
{
    private readonly Encoding _safeUTF8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

    public bool ArePasswordsTheSame(string clientPassword, string serverPassword)
    {
        try
        {
            return BCrypter.Verify(clientPassword, serverPassword)
                || SecureEquals(
                    _safeUTF8.GetBytes(clientPassword),
                    _safeUTF8.GetBytes(serverPassword));
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

    // This method is taken directly from the BCrypt library, with no alterations
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    private bool SecureEquals(byte[] a, byte[] b)
    {
        if (a == null && b == null)
        {
            return true;
        }

        if (a == null || b == null || a.Length != b.Length)
        {
            return false;
        }

        int num = 0;
        for (int i = 0; i < a.Length; i++)
        {
            num |= a[i] ^ b[i];
        }

        return num == 0;
    }
}
