using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Handlers.Images.Commands;
using RecipeSocialMediaAPI.Application.WebClients.Interfaces;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Images.Commands;
public class RemoveMultipleImagesHandlerTests
{
    private readonly Mock<ICloudinaryWebClient> _cloudinaryWebClientMock;
    
    private readonly RemoveMultipleImagesHandler _removeImagesHandlerSUT;

    private readonly List<string> _testPublicIds = new() { "id1", "id2", "id3" };

    public RemoveMultipleImagesHandlerTests()
    {
        _cloudinaryWebClientMock = new Mock<ICloudinaryWebClient>();
        _removeImagesHandlerSUT = new RemoveMultipleImagesHandler(_cloudinaryWebClientMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_RemoveImagesWorks_TaskIsCompleted()
    {
        // Given
        _cloudinaryWebClientMock
            .Setup(x => x.BulkRemoveHostedImages(_testPublicIds))
            .Returns(true);

        // When
        var action = async () => await _removeImagesHandlerSUT
            .Handle(new RemoveMultipleImagesCommand(_testPublicIds), CancellationToken.None);

        // Then
        await action.Should().NotThrowAsync();
        _cloudinaryWebClientMock
            .Verify(repo => repo.BulkRemoveHostedImages(_testPublicIds));
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_RemoveImagesFails_ExceptionThrown()
    {
        // Given
        _cloudinaryWebClientMock
            .Setup(x => x.BulkRemoveHostedImages(_testPublicIds))
            .Returns(false);

        // When
        var action = async () => await _removeImagesHandlerSUT
            .Handle(new RemoveMultipleImagesCommand(_testPublicIds), CancellationToken.None);

        // Then
        await action.Should()
            .ThrowAsync<Exception>()
            .WithMessage($"Could not remove images or only partially removed some images from: [{string.Join(",", _testPublicIds)}]");
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_EmptyIdsListAndRemoveImagesFails_ExceptionThrown()
    {
        // Given
        _cloudinaryWebClientMock
            .Setup(x => x.BulkRemoveHostedImages(It.IsAny<List<string>>()))
            .Returns(false);

        // When
        var action = async () => await _removeImagesHandlerSUT
            .Handle(new RemoveMultipleImagesCommand(new()), CancellationToken.None);

        // Then
        await action.Should()
            .ThrowAsync<Exception>()
            .WithMessage($"Could not remove images or only partially removed some images from: []");
    }
}
