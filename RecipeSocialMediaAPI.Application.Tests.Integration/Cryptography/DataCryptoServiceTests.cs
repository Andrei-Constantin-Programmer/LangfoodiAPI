using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using RecipeSocialMediaAPI.Application.Cryptography;
using RecipeSocialMediaAPI.Application.Options;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Integration.Cryptography;

public class DataCryptoServiceTests
{
    private readonly Mock<IOptions<EncryptionOptions>> _encryptionOptionsMock;

    private readonly DataCryptoService _dataCryptoServiceSUT;

    public DataCryptoServiceTests()
    {
        _encryptionOptionsMock = new Mock<IOptions<EncryptionOptions>>();
        _encryptionOptionsMock
            .Setup(options => options.Value)
            .Returns(new EncryptionOptions()
            {
                EncryptionKey = "TempRecipeShareSocialMediaApiKey"
            });

        _dataCryptoServiceSUT = new DataCryptoService(_encryptionOptionsMock.Object);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.CRYPTOGRAPHY)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    [InlineData("")]
    [InlineData("1")]
    [InlineData(":)")]
    [InlineData("22")]
    [InlineData("   ")]
    [InlineData("small")]
    [InlineData("MediuMSizE")]
    [InlineData("Long@Test!With_Special-Characters")]
    [InlineData("Extremely Long Test &*/. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec vitae.")]
    public void Encrypt_ChangesOriginalString(string plainText)
    {
        // Given

        // When
        var result = _dataCryptoServiceSUT.Encrypt(plainText);

        // Then
        result.Should().NotBe(plainText);
        result.Should().NotBeEmpty();
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.CRYPTOGRAPHY)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    [InlineData("")]
    [InlineData("1")]
    [InlineData(":)")]
    [InlineData("22")]
    [InlineData("   ")]
    [InlineData("small")]
    [InlineData("MediuMSizE")]
    [InlineData("Long@Test!With_Special-Characters")]
    [InlineData("Extremely Long Test &*/. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec vitae.")]
    public void Decrypt_WithSameKey_CorrectlyRetrievesOriginalText(string plainText)
    {
        // Given
        var cipherText = new DataCryptoService(_encryptionOptionsMock.Object).Encrypt(plainText);

        // When
        var result = _dataCryptoServiceSUT.Decrypt(cipherText);

        // Then
        result.Should().Be(plainText);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.CRYPTOGRAPHY)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    [InlineData("")]
    [InlineData("1")]
    [InlineData(":)")]
    [InlineData("22")]
    [InlineData("   ")]
    [InlineData("small")]
    [InlineData("MediuMSizE")]
    [InlineData("Long@Test!With_Special-Characters")]
    [InlineData("Extremely Long Test &*/. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec vitae.")]
    public void Decrypt_WithDifferentKey_DoesNotRetrieveOriginalText(string plainText)
    {
        // Given
        var differentOptions = new Mock<IOptions<EncryptionOptions>>();
        differentOptions
            .Setup(options => options.Value)
            .Returns(new EncryptionOptions()
            {
                EncryptionKey = "yeKipAaideMlaicoSerahSepiceRpmeT"
            });
        
        var cipherText = new DataCryptoService(differentOptions.Object).Encrypt(plainText);

        // When
        var testAction = () => _dataCryptoServiceSUT.Decrypt(cipherText);

        // Then
        testAction.Should().Throw<Exception>();
    }
}
