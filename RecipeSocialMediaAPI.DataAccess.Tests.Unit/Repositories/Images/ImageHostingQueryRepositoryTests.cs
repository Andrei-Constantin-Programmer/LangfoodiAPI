using CloudinaryDotNet;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RecipeSocialMediaAPI.Application.DTO.ImageHosting;
using RecipeSocialMediaAPI.Application.Services.Interfaces;
using RecipeSocialMediaAPI.DataAccess.Helpers;
using RecipeSocialMediaAPI.DataAccess.Repositories.ImageHosting;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Unit.Repositories.ImageHosting;
public class ImageHostingQueryRepositoryTests
{
    private readonly Mock<ILogger<ImageHostingQueryRepository>> _loggerMock;
    private readonly Mock<ICloudinarySignatureService> _signatureServiceMock;
    private readonly CloudinarySignatureDTO _signatureTestData = new("signature1", new DateTimeOffset(2023, 08, 19, 12, 30, 0, TimeSpan.Zero).ToUnixTimeSeconds());
    
    private readonly ImageHostingQueryRepository _imageHostingQueryRepositorySUT;

    public ImageHostingQueryRepositoryTests()
    {
        _loggerMock = new Mock<ILogger<ImageHostingQueryRepository>>();
        _signatureServiceMock = new Mock<ICloudinarySignatureService>();

        var options = Options.Create(new CloudinaryApiOptions()
        {
            CloudName = "cloudname",
            ApiKey = "apikey",
            ApiSecret = "apisecret"
        });

        _imageHostingQueryRepositorySUT = new ImageHostingQueryRepository(
            _signatureServiceMock.Object,
            _loggerMock.Object,
            options
        );

    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GenerateClientSignature_WhenPublicIdIsNullAndNoExceptionThrown_ReturnCloudinarySignatureDTO()
    {
        // Given
        _signatureServiceMock
            .Setup(x => x.GenerateSignature(It.IsAny<Cloudinary>(), null))
            .Returns(_signatureTestData);

        // When
        var result = _imageHostingQueryRepositorySUT.GenerateSignature(null);

        // Then
        result.Should().NotBeNull();
        result!.Signature.Should().NotBeEmpty();
        result!.TimeStamp.Should().Be(_signatureTestData.TimeStamp);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GenerateClientSignature_WhenPublicIdIsNotNullAndNoExceptionThrown_ReturnCloudinarySignatureDTO()
    {
        // Given
        _signatureServiceMock
            .Setup(x => x.GenerateSignature(It.IsAny<Cloudinary>(), It.IsAny<string>()))
            .Returns(_signatureTestData);

        // When
        var result = _imageHostingQueryRepositorySUT.GenerateSignature("sdfsdgs43534");

        // Then
        result.Should().NotBeNull();
        result!.Signature.Should().NotBeEmpty();
        result!.TimeStamp.Should().Be(_signatureTestData.TimeStamp);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GenerateClientSignature_WhenPublicIdIsNotNullAndExceptionThrownForAnyReason_LogInformationAndReturnNull()
    {
        // Given
        Exception testException = new("exception here");
        _signatureServiceMock
            .Setup(x => x.GenerateSignature(It.IsAny<Cloudinary>(), It.IsAny<string>()))
            .Throws(testException);

        // When
        var result = _imageHostingQueryRepositorySUT.GenerateSignature("sdfsdfsdg43453");

        // Then
        result.Should().BeNull();
        _loggerMock
           .Verify(logger =>
               logger.Log(
                   LogLevel.Error,
                   It.IsAny<EventId>(),
                   It.IsAny<It.IsAnyType>(),
                   testException,
                   It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
               Times.Once());
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GenerateClientSignature_WhenPublicIdIsNullAndExceptionThrownForAnyReason_LogInformationAndReturnNull()
    {
        // Given
        Exception testException = new("exception here");
        _signatureServiceMock
            .Setup(x => x.GenerateSignature(It.IsAny<Cloudinary>(), It.IsAny<string>()))
            .Throws(testException);

        // When
        var result = _imageHostingQueryRepositorySUT.GenerateSignature(null);

        // Then
        result.Should().BeNull();
        _loggerMock
           .Verify(logger =>
               logger.Log(
                   LogLevel.Error,
                   It.IsAny<EventId>(),
                   It.IsAny<It.IsAnyType>(),
                   testException,
                   It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
               Times.Once());
    }
}
