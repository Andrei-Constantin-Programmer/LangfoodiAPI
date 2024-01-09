using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using RecipeSocialMediaAPI.Application.DTO.ImageHosting;
using RecipeSocialMediaAPI.Application.Options;
using RecipeSocialMediaAPI.Application.Tests.Unit.TestHelpers;
using RecipeSocialMediaAPI.Application.WebClients;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Net;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.WebClients;
public class CloudinaryWebClientTests
{
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<IOptions<CloudinaryEndpointOptions>> _endpointOptionsMock;  

    private readonly CloudinaryWebClient _cloudinaryWebClientSUT;

    private const string TEST_PUBLIC_ID = "publicid123";
    private const string TEST_API_KEY = "apiKey";
    private const string TEST_API_SECRET = "apiSecret";
    private readonly List<string> _testPublicIds = new() { "id1", "id2", "id3" };
    private readonly CloudinarySignatureDTO _signatureTestData = new()
    {
        Signature = "signature1",
        TimeStamp = new DateTimeOffset(2023, 08, 19, 12, 30, 0, TimeSpan.Zero)
            .ToUnixTimeSeconds()
    };

    public CloudinaryWebClientTests()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();

        _endpointOptionsMock = new Mock<IOptions<CloudinaryEndpointOptions>>();
        _endpointOptionsMock
            .Setup(x => x.Value)
            .Returns(new CloudinaryEndpointOptions()
            {
                SingleRemoveUrl = "https://www.example.com/singleremove",
                BulkRemoveUrl = "https://www.example.com/bulkremove"
            });

        _cloudinaryWebClientSUT = new CloudinaryWebClient(
            _httpClientFactoryMock.Object,
            _endpointOptionsMock.Object
        );
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void RemoveHostedImage_RequestReturnsOk_ReturnsTrue()
    {
        // Given
        HttpMethod? requestMethod = null;
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
        var result = _cloudinaryWebClientSUT.RemoveHostedImage(
            _signatureTestData,
            TEST_API_KEY,
            TEST_PUBLIC_ID);

        // Then
        result.Should().BeTrue();
        requestMethod?.Method.Should().Be("POST");
        requestUri.Should().Be(
            "https://www.example.com/singleremove" +
            $"?public_id={TEST_PUBLIC_ID}&api_key={TEST_API_KEY}&signature={_signatureTestData.Signature}&timestamp={_signatureTestData.TimeStamp}"
        );
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void RemoveHostedImage_RequestReturnsNotOk_ReturnsFalse()
    {
        // Given
        HttpMethod? requestMethod = null;
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
        var result = _cloudinaryWebClientSUT.RemoveHostedImage(
            _signatureTestData,
            TEST_API_KEY,
            TEST_PUBLIC_ID);

        // Then
        result.Should().BeFalse();
        requestMethod?.Method.Should().Be("POST");
        requestUri.Should().Be(
            "https://www.example.com/singleremove" +
            $"?public_id={TEST_PUBLIC_ID}&api_key={TEST_API_KEY}&signature={_signatureTestData.Signature}&timestamp={_signatureTestData.TimeStamp}"
        );
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void BulkRemoveHostedImages_RequestReturnsOk_ReturnsTrue()
    {
        // Given
        HttpMethod? requestMethod = null;
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
        var result = _cloudinaryWebClientSUT.BulkRemoveHostedImages(
            _testPublicIds,
            TEST_API_KEY,
            TEST_API_SECRET);

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
        HttpMethod? requestMethod = null;
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
        var result = _cloudinaryWebClientSUT.BulkRemoveHostedImages(
            _testPublicIds,
            TEST_API_KEY,
            TEST_API_SECRET);

        // Then
        result.Should().BeFalse();
        requestMethod?.Method.Should().Be("DELETE");
        requestUri.Should().Be("https://www.example.com/bulkremove");
    }
}
