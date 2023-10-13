using CloudinaryDotNet;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RecipeSocialMediaAPI.DataAccess.Helpers;
using RecipeSocialMediaAPI.DataAccess.Repositories.ImageHosting;
using RecipeSocialMediaAPI.Domain.Utilities;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Unit.Repositories.ImageHosting;
public class ImageHostingQueryRepositoryTests
{
    private readonly Mock<ILogger<ImageHostingQueryRepository>> _loggerMock;
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
    private readonly CloudinaryApiConfiguration _cloudinaryApiConfiguration;

    private readonly ImageHostingQueryRepository _imageHostingQueryRepositorySUT;
    private static readonly DateTimeOffset _testDate = new(2023, 08, 19, 12, 30, 0, TimeSpan.Zero);

    public ImageHostingQueryRepositoryTests()
    {
        _loggerMock = new Mock<ILogger<ImageHostingQueryRepository>>();
        _dateTimeProviderMock = new Mock<IDateTimeProvider>();
        _cloudinaryApiConfiguration = new CloudinaryApiConfiguration("cloudname", "apikey", "apisecret");

        _imageHostingQueryRepositorySUT = new ImageHostingQueryRepository(
            _loggerMock.Object, 
            _dateTimeProviderMock.Object,
            _cloudinaryApiConfiguration
        );
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.AUTHENTICATION)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GenerateClientSignature_WhenParametersGivenAndNoExceptionThrown_ReturnCloudinarySignatureDTO()
    {
        // Given
        _dateTimeProviderMock
            .Setup(x => x.Now)
            .Returns(_testDate);

        // When
        var result = _imageHostingQueryRepositorySUT.GenerateClientSignature();

        // Then
        result.Signature.Length.Should().BeGreaterThan(0);
        result.TimeStamp.Should().Be(_testDate.ToUnixTimeSeconds());
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.AUTHENTICATION)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GenerateClientSignature_WhenExceptionThrownForAnyReason_LogInformationAndReturnNull()
    {
        // Given
        Exception testException = new("exception here");
        _dateTimeProviderMock
            .Setup(x => x.Now)
            .Throws(testException);

        // When
        var result = _imageHostingQueryRepositorySUT.GenerateClientSignature();

        // Then
        result.Should().BeNull();
        _loggerMock
           .Verify(logger =>
               logger.Log(
                   LogLevel.Information,
                   It.IsAny<EventId>(),
                   It.IsAny<It.IsAnyType>(),
                   testException,
                   It.IsAny<Func<It.IsAnyType, Exception, string>>()),
               Times.Once());
    }
}
