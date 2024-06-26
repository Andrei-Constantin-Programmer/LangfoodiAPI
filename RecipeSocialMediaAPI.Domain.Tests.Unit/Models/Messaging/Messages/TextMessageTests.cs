﻿using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.Domain.Utilities;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Domain.Tests.Unit.Models.Messaging.Messages;

public class TextMessageTests
{
    private readonly TextMessage _textMessageSUT;

    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;

    public TextMessageTests()
    {
        _dateTimeProviderMock = new Mock<IDateTimeProvider>();
        IUserAccount testUser = new TestUserAccount
        {
            Id = "UserId",
            Handler = "UserHandler",
            UserName = "Username",
            AccountCreationDate = new(2023, 10, 9, 0, 0, 0, TimeSpan.Zero)
        };

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
        _textMessageSUT.TextContent = newText;

        // Then
        _textMessageSUT.TextContent.Should().Be(newText);
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
        var testAction = () => _textMessageSUT.TextContent = textContent;

        // Then
        testAction.Should().Throw<ArgumentException>();
        _textMessageSUT.TextContent.Should().NotBe(textContent);
        _textMessageSUT.UpdatedDate.Should().NotBe(testNow);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void MarkAsSeenBy_IfUserHasNotYetSeenTheMessage_AddUserToSeenByListAndReturnTrue()
    {
        // Given
        TestUserAccount newUser = new()
        {
            Id = "u1",
            Handler = "user_1",
            UserName = "User 1"
        };

        // When
        var result = _textMessageSUT.MarkAsSeenBy(newUser);

        // Then
        result.Should().BeTrue();
        _textMessageSUT.GetSeenBy().Should().Contain(newUser);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void MarkAsSeenBy_IfUserHasAlreadySeenTheMessage_DoNotAddUserToSeenByListAndReturnFalse()
    {
        // Given
        TestUserAccount existingUser = new()
        {
            Id = "u1",
            Handler = "user_1",
            UserName = "User 1"
        };
        _textMessageSUT.MarkAsSeenBy(existingUser);

        // When
        var result = _textMessageSUT.MarkAsSeenBy(existingUser);

        // Then
        result.Should().BeFalse();
        _textMessageSUT.GetSeenBy().Should().OnlyHaveUniqueItems().And.Contain(existingUser);
    }
}
