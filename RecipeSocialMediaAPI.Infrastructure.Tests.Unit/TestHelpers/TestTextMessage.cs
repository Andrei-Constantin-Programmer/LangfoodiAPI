﻿using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;

namespace RecipeSocialMediaAPI.Infrastructure.Tests.Unit.TestHelpers;

internal class TestTextMessage : TestMessage
{
    public string? Text { get; set; }

    public TestTextMessage(
        string id,
        IUserAccount sender,
        string? text,
        DateTimeOffset sentDate,
        DateTimeOffset? updatedDate,
        Message? repliedToMessage = null,
        List<IUserAccount>? seenBy = null)
        : base(id, sender, sentDate, updatedDate, repliedToMessage, seenBy)
    {
        Text = text;
    }
}
