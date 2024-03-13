﻿using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;

namespace RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;

public abstract class Conversation
{
    public string ConversationId { get; }

    private readonly Stack<Message> _messages;

    public List<Message> Messages { get => _messages.ToList(); }

    public Conversation(string conversationId, IEnumerable<Message>? messages = null)
    {
        ConversationId = conversationId;
        _messages = messages is null ? new Stack<Message>() : new Stack<Message>(messages);
    }

    public virtual void SendMessage(Message message)
    {
        _messages.Push(message);
    }
}