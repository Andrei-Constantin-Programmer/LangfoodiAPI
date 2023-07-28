using FluentAssertions;
using RecipeSocialMediaAPI.Cryptography;
using RecipeSocialMediaAPI.Tests.Shared.Traits;
using BCrypter = BCrypt.Net.BCrypt;

namespace RecipeSocialMediaAPI.Tests.Integration.Cryptography;

public class CryptoServiceTests
{
    private readonly CryptoService _cryptoServiceSUT;

    public CryptoServiceTests()
    {
        _cryptoServiceSUT = new CryptoService();
    }

    [Fact]
    [Trait(Traits.DOMAIN, "Cryptography")]
    public void ArePasswordsTheSame_WhenValidPasswordAndHash_ReturnsTrue()
    {
        // Given
        string password = "P@ssw0rd";
        string hash = BCrypter.HashPassword(password);

        // When
        bool result = _cryptoServiceSUT.ArePasswordsTheSame(password, hash);

        // Then
        result.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, "Cryptography")]
    public void ArePasswordsTheSame_WhenInvalidPasswordAndHash_ReturnsFalse()
    {
        // Given
        string password = "P@ssw0rd";
        string hash = "wrongly_hashed_password";

        // When
        bool result = _cryptoServiceSUT.ArePasswordsTheSame(password, hash);

        // Then
        result.Should().BeFalse();
    }

    [Fact]
    [Trait(Traits.DOMAIN, "Cryptography")]
    public void Encrypt_ReturnsEncryptedPassword()
    {
        // Given
        string password = "P@ssw0rd";

        // When
        string result = _cryptoServiceSUT.Encrypt(password);

        // Then
        result.Should().NotBe(password);
        BCrypter.Verify(password, result).Should().BeTrue();
    }
}
