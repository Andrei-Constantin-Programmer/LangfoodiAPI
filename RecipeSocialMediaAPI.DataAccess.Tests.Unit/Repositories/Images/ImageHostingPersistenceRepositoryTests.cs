using CloudinaryDotNet;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RecipeSocialMediaAPI.Application.DTO.ImageHosting;
using RecipeSocialMediaAPI.Application.Services.Interfaces;
using RecipeSocialMediaAPI.Application.WebClients.Interfaces;
using RecipeSocialMediaAPI.DataAccess.Helpers;
using RecipeSocialMediaAPI.DataAccess.Repositories.Images;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Unit.Repositories.Images;
public class ImageHostingPersistenceRepositoryTests
{
    private readonly Mock<ILogger<ImageHostingPersistenceRepository>> _loggerMock;
    private readonly Mock<ICloudinarySignatureService> _cloudinarySignatureServiceMock;
    private readonly Mock<ICloudinaryWebClient> _cloudinaryWebClientMock;

    private readonly ImageHostingPersistenceRepository _imageHostingPersistenceRepositorySUT;

    private const string TEST_PUBLIC_ID = "354234535sgf45";
    private readonly CloudinarySignatureDTO _signatureTestData = new()
    {
        Signature = "signature1",
        TimeStamp = new DateTimeOffset(2023, 08, 19, 12, 30, 0, TimeSpan.Zero)
            .ToUnixTimeSeconds()
    };
    private readonly List<string> _testPublicIds = new() { "id1", "id2", "id3" };

    public ImageHostingPersistenceRepositoryTests()
    {
        _loggerMock = new Mock<ILogger<ImageHostingPersistenceRepository>>();
        _cloudinarySignatureServiceMock = new Mock<ICloudinarySignatureService>();
        _cloudinaryWebClientMock = new Mock<ICloudinaryWebClient>();

        var options = Options.Create(new CloudinaryApiOptions()
        {
            CloudName = "cloudname",
            ApiKey = "apikey",
            ApiSecret = "apisecret"
        });

        _cloudinarySignatureServiceMock
            .Setup(x => x.GenerateSignature(It.IsAny<Cloudinary>(), TEST_PUBLIC_ID))
            .Returns(_signatureTestData);

        _imageHostingPersistenceRepositorySUT = new ImageHostingPersistenceRepository(
            _cloudinaryWebClientMock.Object,
            _cloudinarySignatureServiceMock.Object,
            _loggerMock.Object,
            options
        );
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void RemoveHostedImage_RemovalWorks_ReturnsTrue()
    {
        // Given
        _cloudinaryWebClientMock
            .Setup(x => x.RemoveHostedImage(
                _signatureTestData,
                It.IsAny<string>(),
                TEST_PUBLIC_ID
            ))
            .Returns(true);

        // When
        var result = _imageHostingPersistenceRepositorySUT.RemoveHostedImage(TEST_PUBLIC_ID);

        // Then
        result.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void RemoveHostedImage_RemovalFailsNoExceptionThrown_ReturnsFalse()
    {
        // Given
        _cloudinaryWebClientMock
            .Setup(x => x.RemoveHostedImage(
                _signatureTestData,
                It.IsAny<string>(),
                TEST_PUBLIC_ID
            ))
            .Returns(false);

        // When
        var result = _imageHostingPersistenceRepositorySUT.RemoveHostedImage(TEST_PUBLIC_ID);

        // Then
        result.Should().BeFalse();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void RemoveHostedImage_RemovalFailsBecauseExceptionIsThrown_ExceptionPropagated()
    {
        // Given
        Exception testException = new("exception here");
        _cloudinarySignatureServiceMock
            .Setup(x => x.GenerateSignature(It.IsAny<Cloudinary>(), TEST_PUBLIC_ID))
            .Throws(testException);

        // When
        var result = _imageHostingPersistenceRepositorySUT.RemoveHostedImage(TEST_PUBLIC_ID);

        // Then
        result.Should().BeFalse();
        _loggerMock
           .Verify(logger =>
               logger.Log(
                   LogLevel.Error,
                   It.IsAny<EventId>(),
                   It.IsAny<It.IsAnyType>(),
                   testException,
                   It.IsAny<Func<It.IsAnyType, Exception, string>>()),
               Times.Once());
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void BulkRemoveHostedImages_RemovalWorks_ReturnsTrue()
    {
        // Given
        _cloudinaryWebClientMock
            .Setup(x => x.BulkRemoveHostedImages(
                It.IsAny<List<string>>(),
                It.IsAny<string>(),
                It.IsAny<string>()
            ))
            .Returns(true);

        // When
        var result = _imageHostingPersistenceRepositorySUT.BulkRemoveHostedImages(_testPublicIds);

        // Then
        result.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void BulkRemoveHostedImages_RemovalFailsNoExceptionThrown_ReturnsFalse()
    {
        // Given
        _cloudinaryWebClientMock
            .Setup(x => x.BulkRemoveHostedImages(
                It.IsAny<List<string>>(),
                It.IsAny<string>(),
                It.IsAny<string>()
            ))
            .Returns(false);

        // When
        var result = _imageHostingPersistenceRepositorySUT.BulkRemoveHostedImages(_testPublicIds);

        // Then
        result.Should().BeFalse();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void BulkRemoveHostedImages_RemovalFailsBecauseExceptionIsThrown_ExceptionPropagated()
    {
        // Given
        Exception testException = new("exception here");
        _cloudinaryWebClientMock
            .Setup(x => x.BulkRemoveHostedImages(
                It.IsAny<List<string>>(),
                It.IsAny<string>(),
                It.IsAny<string>()
            ))
            .Throws(testException);

        // When
        var result = _imageHostingPersistenceRepositorySUT.BulkRemoveHostedImages(_testPublicIds);

        // Then
        result.Should().BeFalse();
        _loggerMock
           .Verify(logger =>
               logger.Log(
                   LogLevel.Error,
                   It.IsAny<EventId>(),
                   It.IsAny<It.IsAnyType>(),
                   testException,
                   It.IsAny<Func<It.IsAnyType, Exception, string>>()),
               Times.Once());
    }
}
