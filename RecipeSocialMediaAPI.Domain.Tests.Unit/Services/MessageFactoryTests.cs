using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Services;
using RecipeSocialMediaAPI.Domain.Utilities;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Domain.Tests.Unit.Services;

public class MessageFactoryTests
{
    private readonly MessageFactory _messageFactorySUT;
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;

    public MessageFactoryTests()
    {
        _dateTimeProviderMock = new Mock<IDateTimeProvider>();

        _messageFactorySUT = new MessageFactory(_dateTimeProviderMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void CreateTextMessage_ReturnsTextMessageWithExpectedPropertyValues()
    {
        // Given
        var testId = "TestId";
        User testSender = new("UserId", "Username", "UserEmail", "UserPassword");
        var testText = "Message content";
        DateTimeOffset testSentDate = new(2023, 9, 3, 16, 30, 0, TimeSpan.Zero);
        DateTimeOffset testUpdateDate = new(2023, 10, 3, 16, 30, 0, TimeSpan.Zero);

        Message testReplyMessage = new TextMessage(_dateTimeProviderMock.Object, "ReplyId", testSender, "ReplyText", testSentDate.AddDays(-5));

        // When
        TextMessage result = _messageFactorySUT.CreateTextMessage(testId, testSender, testText, testSentDate, testUpdateDate, testReplyMessage);

        // Then
        result.Id.Should().Be(testId);
        result.Sender.Should().Be(testSender);
        result.TextContent.Should().Be(testText);
        result.SentDate.Should().Be(testSentDate);
        result.UpdatedDate.Should().Be(testUpdateDate);
        result.RepliedToMessage.Should().Be(testReplyMessage);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void CreateTextMessage_WithInvalidTextContent_ThrowsArgumentException(string textContent)
    {
        // Given
        var testId = "TestId";
        User testSender = new("UserId", "Username", "UserEmail", "UserPassword");
        DateTimeOffset testSentDate = new(2023, 9, 3, 16, 30, 0, TimeSpan.Zero);
        DateTimeOffset testUpdateDate = new(2023, 10, 3, 16, 30, 0, TimeSpan.Zero);

        // When
        var testAction = () => _messageFactorySUT.CreateTextMessage(testId, testSender, textContent, testSentDate, testUpdateDate);

        // Then
        testAction.Should().Throw<ArgumentException>();
    }
}
