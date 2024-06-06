﻿using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class UnsupportedConnectionStatusException : Exception
{
    public string UnsupportedStatus { get; }

    public UnsupportedConnectionStatusException(string connectionStatus) : base($"Could not map {connectionStatus} to {typeof(ConnectionStatus)}")
    {
        UnsupportedStatus = connectionStatus;
    }

    protected UnsupportedConnectionStatusException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        UnsupportedStatus = info.GetString("UnsupportedStatus") ?? string.Empty;
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue("UnsupportedStatus", UnsupportedStatus);
    }
}
