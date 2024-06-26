﻿using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class InvalidUserRoleException : Exception
{
    private const string INVALID_ROLE_PROPERTY_NAME = "InvalidRole";
    public string InvalidRole { get; }

    public InvalidUserRoleException(string role) : base($"Invalid user role {role}")
    {
        InvalidRole = role;
    }

    [ExcludeFromCodeCoverage(Justification = "Already tested (indirectly) in exception tests for serialization")]
    protected InvalidUserRoleException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        InvalidRole = info.GetString(INVALID_ROLE_PROPERTY_NAME) ?? string.Empty;
    }

    [ExcludeFromCodeCoverage(Justification = "Already tested (indirectly) in exception tests for serialization")]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(INVALID_ROLE_PROPERTY_NAME, InvalidRole);
    }
}
