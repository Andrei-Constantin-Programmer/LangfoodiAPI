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

    private readonly CloudinaryWebClient _cloudinaryWebClientSUT;

    private const string TEST_PUBLIC_ID = "publicid123";
    private const string TEST_API_KEY = "apiKey";
    private const string TEST_API_SECRET = "apiSecret";
    private readonly List<string> _test_public_ids = new() { "id1", "id2", "id3" };
    private readonly CloudinarySignatureDTO _signature_test_data = new()
    {
        Signature = "signature1",
        TimeStamp = new DateTimeOffset(2023, 08, 19, 12, 30, 0, TimeSpan.Zero)
            .ToUnixTimeSeconds()
    };

    public CloudinaryWebClientTests()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();

        var options = new Mock<IOptions<CloudinaryEndpointOptions>>();
        options
            .Setup(x => x.Value)
            .Returns(new CloudinaryEndpointOptions()
            {
                SingleRemoveUrl = "https://www.example.com/singleremove",
                BulkRemoveUrl = "https://www.example.com/bulkremove"
            });

        _cloudinaryWebClientSUT = new CloudinaryWebClient(
            _httpClientFactoryMock.Object,
            options.Object
        );
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void RemoveHostedImage_RequestReturnsOk_ReturnsTrue()
    {
        // Given
        var handlerStub = new HttpClientSendHandlerStub(
            (request, cancellationToken) => 
            {
                request.Method.Should().Be(new HttpMethod("POST"));
                request.RequestUri.Should().Be(
                    "https://www.example.com/singleremove" +
                    $"?public_id={TEST_PUBLIC_ID}&api_key={TEST_API_KEY}&signature={_signature_test_data.Signature}&timestamp={_signature_test_data.TimeStamp}"
                );
                return new HttpResponseMessage(HttpStatusCode.OK); 
            }
        );
        var client = new HttpClient(handlerStub);

        _httpClientFactoryMock
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(client);

        // When
        var result = _cloudinaryWebClientSUT.RemoveHostedImage(
            _signature_test_data, 
            TEST_API_KEY,
            TEST_PUBLIC_ID);

        // Then
        result.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void RemoveHostedImage_RequestReturnsNotOk_ReturnsFalse()
    {
        // Given
        var handlerStub = new HttpClientSendHandlerStub(
            (request, cancellationToken) =>
            {
                request.Method.Should().Be(new HttpMethod("POST"));
                request.RequestUri.Should().Be(
                    "https://www.example.com/singleremove" +
                    $"?public_id={TEST_PUBLIC_ID}&api_key={TEST_API_KEY}&signature={_signature_test_data.Signature}&timestamp={_signature_test_data.TimeStamp}"
                );
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        );
        var client = new HttpClient(handlerStub);

        _httpClientFactoryMock
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(client);

        // When
        var result = _cloudinaryWebClientSUT.RemoveHostedImage(
            _signature_test_data,
            TEST_API_KEY,
            TEST_PUBLIC_ID);

        // Then
        result.Should().BeFalse();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void BulkRemoveHostedImages_RequestReturnsOk_ReturnsTrue()
    {
        // Given
        var handlerStub = new HttpClientSendHandlerStub(
            (request, cancellationToken) =>
            {
                request.Method.Should().Be(new HttpMethod("DELETE"));
                request.RequestUri.Should().Be(
                    "https://www.example.com/bulkremove"
                );
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
        );
        var client = new HttpClient(handlerStub);

        _httpClientFactoryMock
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(client);

        // When
        var result = _cloudinaryWebClientSUT.BulkRemoveHostedImages(
            _test_public_ids,
            TEST_API_KEY,
            TEST_API_SECRET);

        // Then
        result.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void BulkRemoveHostedImages_RequestReturnsNotOk_ReturnsFalse()
    {
        // Given
        var handlerStub = new HttpClientSendHandlerStub(
            (request, cancellationToken) =>
            {
                request.Method.Should().Be(new HttpMethod("DELETE"));
                request.RequestUri.Should().Be(
                    "https://www.example.com/bulkremove"
                );
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        );
        var client = new HttpClient(handlerStub);

        _httpClientFactoryMock
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(client);

        // When
        var result = _cloudinaryWebClientSUT.BulkRemoveHostedImages(
            _test_public_ids,
            TEST_API_KEY,
            TEST_API_SECRET);

        // Then
        result.Should().BeFalse();
    }
}
