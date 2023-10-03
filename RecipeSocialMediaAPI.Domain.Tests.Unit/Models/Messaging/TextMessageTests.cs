using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Utilities;
using Moq;
using FluentAssertions;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Domain.Tests.Unit.Models.Messaging;
public class TextMessageTests
{
    private readonly TextMessage _textMessageSUT;

    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;

    public TextMessageTests() 
    {
        _dateTimeProviderMock = new Mock<IDateTimeProvider>();
        User testUser = new("UserId", "Username", "UserEmail", "UserPassword");

        DateTimeOffset testDate = new(2023, 10, 3, 16, 30, 0, TimeSpan.Zero);

        _textMessageSUT = new(_dateTimeProviderMock.Object, "MessageId", testUser, "Message Content", testDate, testDate);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void SetText_UpdatesTextAndUpdatedTime()
    {
        // Given
        DateTimeOffset testNow = new(2023, 10, 3, 17, 0, 0, TimeSpan.Zero);
        _dateTimeProviderMock
            .Setup(provider => provider.Now)
            .Returns(testNow);

        var newText = "New message content";

        // When
        _textMessageSUT.Text = newText;

        // Then
        _textMessageSUT.Text.Should().Be(newText);
        _textMessageSUT.UpdatedDate.Should().Be(testNow);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void SetText_WithInvalidTextContent_ThrowsArgumentException(string textContent)
    {
        // Given
        DateTimeOffset testNow = new(2023, 10, 3, 17, 0, 0, TimeSpan.Zero);
        _dateTimeProviderMock
            .Setup(provider => provider.Now)
            .Returns(testNow);

        // When
        var testAction = () => _textMessageSUT.Text = textContent;

        // Then
        testAction.Should().Throw<ArgumentException>();
        _textMessageSUT.Text.Should().NotBe(textContent);
        _textMessageSUT.UpdatedDate.Should().NotBe(testNow);
    }
}
