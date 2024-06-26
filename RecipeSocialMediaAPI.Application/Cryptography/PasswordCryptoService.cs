﻿using RecipeSocialMediaAPI.Application.Cryptography.Interfaces;
using BCrypter = BCrypt.Net.BCrypt;

namespace RecipeSocialMediaAPI.Application.Cryptography;

public class PasswordCryptoService : IPasswordCryptoService
{

    public bool ArePasswordsTheSame(string decryptedPassword, string encryptedPassword)
    {
        try
        {
            return BCrypter.Verify(decryptedPassword, encryptedPassword)
                   || decryptedPassword == encryptedPassword;
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
