using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Utilities;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Domain.Tests.Unit.Models.Messaging;

public class ImageMessageTests
{
    private readonly ImageMessage _imageMessageSUT;

    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;

    public ImageMessageTests()
    {
        _dateTimeProviderMock = new Mock<IDateTimeProvider>();

        User testUser = new("UserId", "Username", "UserEmail", "UserPassword");
        DateTimeOffset testDate = new(2023, 10, 3, 16, 30, 0, TimeSpan.Zero);

        List<string> images = new() { "Image1" };

        _imageMessageSUT = new(_dateTimeProviderMock.Object, "MessageId", testUser, images, "Message Content", testDate, testDate);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    [InlineData("New Text")]
    [InlineData("")]
    [InlineData(null)]
    public void SetText_UpdatesTextAndUpdatedTime(string newText)
    {
        // Given
        DateTimeOffset testNow = new(2023, 10, 3, 17, 0, 0, TimeSpan.Zero);
        _dateTimeProviderMock
            .Setup(provider => provider.Now)
            .Returns(testNow);

        // When
        _imageMessageSUT.TextContent = newText;

        // Then
        _imageMessageSUT.TextContent.Should().Be(newText);
        _imageMessageSUT.UpdatedDate.Should().Be(testNow);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void AddImage_AddsImageToListAndUpdatedTime()
    {
        // Given
        DateTimeOffset testNow = new(2023, 10, 3, 17, 0, 0, TimeSpan.Zero);
        _dateTimeProviderMock
            .Setup(provider => provider.Now)
            .Returns(testNow);

        var newImage = "New Image";

        // When
        _imageMessageSUT.AddImage(newImage);

        // Then
        _imageMessageSUT.ImageURLs.Should().Contain(newImage);
        _imageMessageSUT.UpdatedDate.Should().Be(testNow);
    }
}
