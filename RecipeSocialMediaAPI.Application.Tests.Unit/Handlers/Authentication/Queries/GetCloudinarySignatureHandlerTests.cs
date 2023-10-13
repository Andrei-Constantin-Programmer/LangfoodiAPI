using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.DTO.ImageHosting;
using RecipeSocialMediaAPI.Application.Handlers.Authentication.Queries;
using RecipeSocialMediaAPI.Application.Repositories.ImageHosting;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Authentication.Queries;
public class GetCloudinarySignatureHandlerTests
{
    private readonly Mock<IImageHostingQueryRepository> _imageHostingQueryRepositoryMock;

    private readonly GetCloudinarySignatureHandler _getCloudinarySignatureHandler;

    public GetCloudinarySignatureHandlerTests()
    {
        _imageHostingQueryRepositoryMock = new Mock<IImageHostingQueryRepository>();
        _getCloudinarySignatureHandler = new GetCloudinarySignatureHandler(_imageHostingQueryRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.AUTHENTICATION)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenSignatureIsNotNull_ReturnSignatureDTO()
    {
        // Given
        _imageHostingQueryRepositoryMock
            .Setup(x => x.GenerateClientSignature())
            .Returns(new CloudinarySignatureDTO() { Signature = "sig", TimeStamp = 1000 });

        // When
        var result = await _getCloudinarySignatureHandler.Handle(new GetCloudinarySignatureQuery(), CancellationToken.None);

        // Then
        result.Signature.Should().Be("sig");
        result.TimeStamp.Should().Be(1000);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.AUTHENTICATION)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenSignatureIsNull_ThrowInvalidOperationException()
    {
        // Given
        _imageHostingQueryRepositoryMock
            .Setup(x => x.GenerateClientSignature())
            .Returns((CloudinarySignatureDTO)null);

        // When
        var action = async () => await _getCloudinarySignatureHandler.Handle(new GetCloudinarySignatureQuery(), CancellationToken.None);

        // Then
        await action.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Failed to generate signature");
    }
}
