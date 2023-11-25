using CloudinaryDotNet;
using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Services;
using RecipeSocialMediaAPI.Domain.Utilities;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Services;
public class CloudinarySignatureServiceTests
{
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
    private readonly Cloudinary _cloudinaryConfig;
    private readonly CloudinarySignatureService _cloudinarySignatureServiceSUT;
    private static readonly DateTimeOffset _testDate = new(2023, 08, 19, 12, 30, 0, TimeSpan.Zero);

    public CloudinarySignatureServiceTests()
    {
        _dateTimeProviderMock = new Mock<IDateTimeProvider>();
        _cloudinaryConfig = new Cloudinary(new Account(
            "cloudname", "apikey", "apisecret"
        ));

        _dateTimeProviderMock
            .Setup(x => x.Now)
            .Returns(_testDate);

        _cloudinarySignatureServiceSUT = new CloudinarySignatureService(_dateTimeProviderMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void GenerateClientSignature_WhenPublicIdIsNullAndNoExceptionThrown_ReturnCloudinarySignatureDTO()
    {
        // Given

        // When
        var result = _cloudinarySignatureServiceSUT.GenerateSignature(_cloudinaryConfig, null);

        // Then
        result.Should().NotBeNull();
        result!.Signature.Should().NotBeEmpty();
        result!.TimeStamp.Should().Be(_testDate.ToUnixTimeSeconds());
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void GenerateClientSignature_WhenPublicIdIsNotNullAndNoExceptionThrown_ReturnCloudinarySignatureDTO()
    {
        // Given

        // When
        var result = _cloudinarySignatureServiceSUT.GenerateSignature(_cloudinaryConfig, "sdfsdgs43534");

        // Then
        result.Should().NotBeNull();
        result!.Signature.Should().NotBeEmpty();
        result!.TimeStamp.Should().Be(_testDate.ToUnixTimeSeconds());
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void GenerateClientSignature_WhenPublicIdIsNotNullAndException_ThrowException()
    {
        // Given
        Exception testException = new("exception here");
        _dateTimeProviderMock
            .Setup(x => x.Now)
            .Throws(testException);

        // When
        var action = () => _cloudinarySignatureServiceSUT.GenerateSignature(_cloudinaryConfig, "sdfsdfsdg43453");

        // Then
        action.Should()
            .Throw<Exception>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void GenerateClientSignature_WhenPublicIdIsNullAndException_ThrowException()
    {
        // Given
        Exception testException = new("exception here");
        _dateTimeProviderMock
            .Setup(x => x.Now)
            .Throws(testException);

        // When
        var action = () => _cloudinarySignatureServiceSUT.GenerateSignature(_cloudinaryConfig, null);

        // Then
        action.Should()
            .Throw<Exception>();
    }
}
