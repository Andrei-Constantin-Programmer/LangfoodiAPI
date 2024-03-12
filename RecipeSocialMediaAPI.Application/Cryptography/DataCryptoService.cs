using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RecipeSocialMediaAPI.Application.Cryptography.Interfaces;
using RecipeSocialMediaAPI.Application.Options;
using System.Security.Cryptography;
using System.Text;

namespace RecipeSocialMediaAPI.Application.Cryptography;

public class DataCryptoService : IDataCryptoService
{
    private readonly ILogger<DataCryptoService> _logger;

    private readonly byte[] _encryptionKeyBytes;

    public DataCryptoService(IOptions<EncryptionOptions> encryptionOptions, ILogger<DataCryptoService> logger)
    {
        _encryptionKeyBytes = Encoding.UTF8.GetBytes(encryptionOptions.Value.EncryptionKey);
        _logger = logger;
    }

    public string Encrypt(string plainText)
    {
        using var aes = Aes.Create();

        aes.Key = _encryptionKeyBytes;
        aes.IV = RandomNumberGenerator.GetBytes(16);

        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        using var memoryStream = new MemoryStream();
        using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
        using (var streamWriter = new StreamWriter(cryptoStream))
        {
            streamWriter.Write(plainText);
        }

        byte[] encryptedBytes = memoryStream.ToArray();
        byte[] bytesWithIV = new byte[aes.IV.Length + encryptedBytes.Length];

        Array.Copy(aes.IV, 0, bytesWithIV, 0, aes.IV.Length);
        Array.Copy(encryptedBytes, 0, bytesWithIV, aes.IV.Length, encryptedBytes.Length);

        return Convert.ToBase64String(bytesWithIV);
    }

    public string Decrypt(string cipherText)
    {
        byte[] bytesWithIV = Convert.FromBase64String(cipherText);
        
        byte[] ivBytes = new byte[16];
        Array.Copy(bytesWithIV, 0, ivBytes, 0, 16);

        byte[] encryptedBytes = new byte[bytesWithIV.Length - 16];
        Array.Copy(bytesWithIV, 16, encryptedBytes, 0, encryptedBytes.Length);

        using var aes = Aes.Create();
        aes.Key = _encryptionKeyBytes;
        aes.IV = ivBytes;

        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using var memoryStream = new MemoryStream(encryptedBytes);
        using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        using var streamReader = new StreamReader(cryptoStream);

        try
        {
            return streamReader.ReadToEnd();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was an error trying to decrypt information {CipherText}", cipherText);
            return string.Empty;
        }
    }
}
