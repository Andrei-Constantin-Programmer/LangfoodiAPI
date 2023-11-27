using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using RecipeSocialMediaAPI.Application.DTO.ImageHosting;
using RecipeSocialMediaAPI.Core.Tests.Integration.IntegrationHelpers;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Net.Http.Json;
using System.Net;

namespace RecipeSocialMediaAPI.Core.Tests.Integration.Endpoints;
public class ImageEndpointsTests : EndpointTestBase
{
    public ImageEndpointsTests(WebApplicationFactory<Program> factory) : base(factory) { }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void GetCloudinarySignature_GivenPublicIdAndValidCall_ReturnGeneratedCloudinarySignature()
    {
        // Given

        // When
        var result = await _client.PostAsync("image/get/cloudinary-signature", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        CloudinarySignatureDTO generatedSignature = (await result.Content.ReadFromJsonAsync<CloudinarySignatureDTO>())!;

        generatedSignature.Signature.Should().NotBeEmpty();
        generatedSignature.TimeStamp.Should().BeGreaterThan(0);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void SingleDelete_GivenPublicIdAndValidCall_ReturnOk()
    {
        // Given


        // When
        var result = await _client.DeleteAsync("/image/single-delete?publicId=publicId123");

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
