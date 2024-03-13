using FluentAssertions;
using RecipeSocialMediaAPI.Application.Cryptography;
using RecipeSocialMediaAPI.TestInfrastructure;
using BCrypter = BCrypt.Net.BCrypt;

namespace RecipeSocialMediaAPI.Application.Tests.Integration.Cryptography;

public class PasswordCryptoServiceTests
{
    private readonly PasswordCryptoService _passwordCryptoServiceSUT;

    public PasswordCryptoServiceTests()
    {
        _passwordCryptoServiceSUT = new PasswordCryptoService();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CRYPTOGRAPHY)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void ArePasswordsTheSame_WhenValidUnhashedPasswordAndHash_ReturnsTrue()
    {
        // Given
        string password = "P@ssw0rd";
        string hash = BCrypter.HashPassword(password);

        // When
        bool result = _passwordCryptoServiceSUT.ArePasswordsTheSame(password, hash);

        // Then
        result.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CRYPTOGRAPHY)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void ArePasswordsTheSame_WhenInvalidUnhashedPasswordAndHash_ReturnsFalse()
    {
        // Given
        string password = "P@ssw0rd";
        string hash = "wrongly_hashed_password";

        // When
        bool result = _passwordCryptoServiceSUT.ArePasswordsTheSame(password, hash);

        // Then
        result.Should().BeFalse();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CRYPTOGRAPHY)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void ArePasswordsTheSame_WhenValidHashedPasswordAndHash_ReturnsTrue()
    {
        // Given
        string password = "P@ssw0rd";
        string hash = BCrypter.HashPassword(password);

        // When
        bool result = _passwordCryptoServiceSUT.ArePasswordsTheSame(hash, hash);

        // Then
        result.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CRYPTOGRAPHY)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void ArePasswordsTheSame_WhenInvalidHashedPasswordAndHash_ReturnsFalse()
    {
        // Given
        string password = "P@ssw0rd";
        string clientHash = BCrypter.HashPassword(password);
        string serverHash = BCrypter.HashPassword(password);

        // When
        bool result = _passwordCryptoServiceSUT.ArePasswordsTheSame(clientHash, serverHash);

        // Then
        result.Should().BeFalse();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CRYPTOGRAPHY)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void Encrypt_ReturnsEncryptedPassword()
    {
        // Given
        string password = "P@ssw0rd";

        // When
        string result = _passwordCryptoServiceSUT.Encrypt(password);

        // Then
        result.Should().NotBe(password);
        BCrypter.Verify(password, result).Should().BeTrue();
    }
}
