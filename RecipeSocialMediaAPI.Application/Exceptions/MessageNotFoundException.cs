﻿using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class MessageNotFoundException : Exception
{
    public MessageNotFoundException(string messageId) : base($"The message with the id {messageId} was not found") { }

    [ExcludeFromCodeCoverage(Justification = "Already tested (indirectly) in exception tests for serialization")]
    protected MessageNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
