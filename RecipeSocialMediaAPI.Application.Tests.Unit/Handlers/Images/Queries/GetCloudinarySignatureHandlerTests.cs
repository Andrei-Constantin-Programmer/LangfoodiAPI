using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.DTO.ImageHosting;
using RecipeSocialMediaAPI.Application.Handlers.Images.Queries;
using RecipeSocialMediaAPI.Application.Repositories.ImageHosting;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Images.Queries;
public class GetCloudinarySignatureHandlerTests
{
    private readonly Mock<IImageHostingQueryRepository> _imageHostingQueryRepositoryMock;

    private readonly GetCloudinarySignatureHandler _getCloudinarySignatureHandlerSUT;

    public GetCloudinarySignatureHandlerTests()
    {
        _imageHostingQueryRepositoryMock = new Mock<IImageHostingQueryRepository>();
        _getCloudinarySignatureHandlerSUT = new GetCloudinarySignatureHandler(_imageHostingQueryRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_GenerateSignatureWorks_ReturnSignatureDTO()
    {
        // Given
        _imageHostingQueryRepositoryMock
            .Setup(x => x.GenerateSignature(null))
            .Returns(new CloudinarySignatureDTO() { Signature = "sig", TimeStamp = 1000 });

        // When
        var result = await _getCloudinarySignatureHandlerSUT.Handle(new GetCloudinarySignatureQuery(), CancellationToken.None);

        // Then
        result.Signature.Should().Be("sig");
        result.TimeStamp.Should().Be(1000);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_NoSignatureGenerated_ThrowsException()
    {
        // Given
        _imageHostingQueryRepositoryMock
            .Setup(x => x.GenerateSignature(null))
            .Returns((CloudinarySignatureDTO?)null);

        // When
        var action = async () => await _getCloudinarySignatureHandlerSUT.Handle(new GetCloudinarySignatureQuery(), CancellationToken.None);

        // Then
        await action.Should()
            .ThrowAsync<Exception>()
            .WithMessage("Failed to generate signature");
    }
}
