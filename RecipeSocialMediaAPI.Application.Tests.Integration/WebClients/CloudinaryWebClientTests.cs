using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RecipeSocialMediaAPI.Application.DTO.ImageHosting;
using RecipeSocialMediaAPI.Application.Options;
using RecipeSocialMediaAPI.Application.Tests.Unit.TestHelpers;
using RecipeSocialMediaAPI.Application.WebClients;
using RecipeSocialMediaAPI.Domain.Utilities;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Net;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.WebClients;

public class CloudinaryWebClientTests
{
    private static readonly DateTimeOffset _testDate = new(2023, 08, 19, 12, 30, 0, TimeSpan.Zero);

    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<IOptions<CloudinaryOptions>> _cloudinaryOptionsMock;
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
    private readonly Mock<ILogger<CloudinaryWebClient>> _loggerMock;

    private readonly CloudinaryWebClient _cloudinaryWebClientSUT;

    private const string TEST_PUBLIC_ID = "publicid123";
    private const string TEST_API_KEY = "apiKey";
    private const string TEST_API_SECRET = "apiSecret";
    private const string TEST_CLOUD_NAME = "cloudName";

    private readonly List<string> _testPublicIds = new() { "id1", "id2", "id3" };

    private readonly CloudinarySignatureDTO _signatureTestData = 
        new("signature1", new DateTimeOffset(2023, 08, 19, 12, 30, 0, TimeSpan.Zero).ToUnixTimeSeconds());

    public CloudinaryWebClientTests()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();

        _cloudinaryOptionsMock = new Mock<IOptions<CloudinaryOptions>>();
        _cloudinaryOptionsMock
            .Setup(x => x.Value)
            .Returns(new CloudinaryOptions()
            {
                SingleRemoveUrl = "https://www.example.com/singleremove",
                BulkRemoveUrl = "https://www.example.com/bulkremove",
                ApiKey = TEST_API_KEY,
                ApiSecret = TEST_API_SECRET,
                CloudName = TEST_CLOUD_NAME
            });

        _dateTimeProviderMock = new Mock<IDateTimeProvider>();
        _dateTimeProviderMock
            .Setup(provider => provider.Now)
            .Returns(_testDate);
        _loggerMock = new Mock<ILogger<CloudinaryWebClient>>();

        _cloudinaryWebClientSUT = new CloudinaryWebClient(
            _httpClientFactoryMock.Object,
            _cloudinaryOptionsMock.Object,
            _dateTimeProviderMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void RemoveHostedImage_RequestReturnsOk_ReturnsTrue()
    {
        // Given
        System.Net.Http.HttpMethod? requestMethod = null;
        Uri? requestUri = null;
        var handlerStub = new HttpClientSendHandlerStub(
            (request, cancellationToken) =>
            {
                requestMethod = request.Method;
                requestUri = request.RequestUri;
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
        );
        var client = new HttpClient(handlerStub);

        _httpClientFactoryMock
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(client);

        // When
        var result = _cloudinaryWebClientSUT.RemoveHostedImage(TEST_PUBLIC_ID);

        // Then
        result.Should().BeTrue();
        requestMethod?.Method.Should().Be("POST");
        requestUri!.ToString().Should().Match(
            "https://www.example.com/singleremove" +
            $"?public_id={TEST_PUBLIC_ID}&api_key={TEST_API_KEY}&signature=*&timestamp={_signatureTestData.TimeStamp}"
        );
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void RemoveHostedImage_RequestReturnsNotOk_ReturnsFalse()
    {
        // Given
        System.Net.Http.HttpMethod? requestMethod = null;
        Uri? requestUri = null;
        var handlerStub = new HttpClientSendHandlerStub(
            (request, cancellationToken) =>
            {
                requestMethod = request.Method;
                requestUri = request.RequestUri;
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        );

        var client = new HttpClient(handlerStub);

        _httpClientFactoryMock
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(client);

        // When
        var result = _cloudinaryWebClientSUT.RemoveHostedImage(TEST_PUBLIC_ID);

        // Then
        result.Should().BeFalse();
        requestMethod?.Method.Should().Be("POST");
        requestUri!.ToString().Should().Match(
            "https://www.example.com/singleremove" +
            $"?public_id={TEST_PUBLIC_ID}&api_key={TEST_API_KEY}&signature=*&timestamp={_signatureTestData.TimeStamp}"
        );
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void BulkRemoveHostedImages_RequestReturnsOk_ReturnsTrue()
    {
        // Given
        System.Net.Http.HttpMethod? requestMethod = null;
        Uri? requestUri = null;
        var handlerStub = new HttpClientSendHandlerStub(
            (request, cancellationToken) =>
            {
                requestMethod = request.Method;
                requestUri = request.RequestUri;
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
        );
        var client = new HttpClient(handlerStub);

        _httpClientFactoryMock
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(client);

        // When
        var result = _cloudinaryWebClientSUT.BulkRemoveHostedImages(_testPublicIds);

        // Then
        result.Should().BeTrue();
        requestMethod?.Method.Should().Be("DELETE");
        requestUri.Should().Be("https://www.example.com/bulkremove");
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void BulkRemoveHostedImages_RequestReturnsNotOk_ReturnsFalse()
    {
        // Given
        System.Net.Http.HttpMethod? requestMethod = null;
        Uri? requestUri = null;
        var handlerStub = new HttpClientSendHandlerStub(
            (request, cancellationToken) =>
            {
                requestMethod = request.Method;
                requestUri = request.RequestUri;
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        );
        var client = new HttpClient(handlerStub);

        _httpClientFactoryMock
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(client);

        // When
        var result = _cloudinaryWebClientSUT.BulkRemoveHostedImages(_testPublicIds);

        // Then
        result.Should().BeFalse();
        requestMethod?.Method.Should().Be("DELETE");
        requestUri.Should().Be("https://www.example.com/bulkremove");
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void GenerateClientSignature_WhenPublicIdIsNotGivenAndNoExceptionThrown_ReturnCloudinarySignatureDTO()
    {
        // Given

        // When
        var result = _cloudinaryWebClientSUT.GenerateSignature();

        // Then
        result.Should().NotBeNull();
        result!.Signature.Should().NotBeEmpty();
        result!.TimeStamp.Should().Be(_testDate.ToUnixTimeSeconds());
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void GenerateClientSignature_WhenPublicIdIsNullAndNoExceptionThrown_ReturnCloudinarySignatureDTO()
    {
        // Given

        // When
        var result = _cloudinaryWebClientSUT.GenerateSignature(null);

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
        var result = _cloudinaryWebClientSUT.GenerateSignature("sdfsdgs43534");

        // Then
        result.Should().NotBeNull();
        result!.Signature.Should().NotBeEmpty();
        result!.TimeStamp.Should().Be(_testDate.ToUnixTimeSeconds());
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void GenerateClientSignature_WhenPublicIdIsNotNullAndException_LogExceptionAndReturnNull()
    {
        // Given
        Exception testException = new("exception here");
        _dateTimeProviderMock
            .Setup(x => x.Now)
            .Throws(testException);

        // When
        var result = _cloudinaryWebClientSUT.GenerateSignature("sdfsdfsdg43453");

        // Then
        result.Should().BeNull();
        _loggerMock
            .Verify(logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    testException,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void GenerateClientSignature_WhenPublicIdIsNullAndException_LogExceptionAndReturnNull()
    {
        // Given
        Exception testException = new("exception here");
        _dateTimeProviderMock
            .Setup(x => x.Now)
            .Throws(testException);

        // When
        var result = _cloudinaryWebClientSUT.GenerateSignature(null);

        // Then
        result.Should().BeNull();
        _loggerMock
            .Verify(logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    testException,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
    }
}
