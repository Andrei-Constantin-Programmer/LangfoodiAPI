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
    private readonly Mock<Cloudinary> _cloudinaryConfigMock;
    private readonly CloudinarySignatureService _cloudinarySignatureService;
    private static readonly DateTimeOffset _testDate = new(2023, 08, 19, 12, 30, 0, TimeSpan.Zero);

    public CloudinarySignatureServiceTests()
    {
        _dateTimeProviderMock = new Mock<IDateTimeProvider>();
        _cloudinaryConfigMock = new Mock<Cloudinary>();

        _dateTimeProviderMock
            .Setup(x => x.Now)
            .Returns(_testDate);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void GenerateClientSignature_WhenPublicIdIsNullAndNoExceptionThrown_ReturnCloudinarySignatureDTO()
    {
        // Given

        // When
        var result = _cloudinarySignatureService.GenerateSignature(_cloudinaryConfigMock.Object, null);

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
        var result = _cloudinarySignatureService.GenerateSignature(_cloudinaryConfigMock.Object, "sdfsdgs43534");

        // Then
        result.Should().NotBeNull();
        result!.Signature.Should().NotBeEmpty();
        result!.TimeStamp.Should().Be(_testDate.ToUnixTimeSeconds());
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void GenerateClientSignature_WhenPublicIdIsNotNullAndExceptionThrownForAnyReason_ReturnNull()
    {
        // Given
        Exception testException = new("exception here");
        _dateTimeProviderMock
            .Setup(x => x.Now)
            .Throws(testException);

        // When
        var result = _cloudinarySignatureService.GenerateSignature(_cloudinaryConfigMock.Object, "sdfsdfsdg43453");

        // Then
        result.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void GenerateClientSignature_WhenPublicIdIsNullAndExceptionThrownForAnyReason_ReturnNull()
    {
        // Given
        Exception testException = new("exception here");
        _dateTimeProviderMock
            .Setup(x => x.Now)
            .Throws(testException);

        // When
        var result = _cloudinarySignatureService.GenerateSignature(_cloudinaryConfigMock.Object, null);

        // Then
        result.Should().BeNull();
    }
}
