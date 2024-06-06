using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Infrastructure.Exceptions;

[Serializable]
public class UnknownUserRoleException : Exception
{
    public UnknownUserRoleException(int role) : base($"Unknown user role {role}") { }

    [ExcludeFromCodeCoverage(Justification = "Already tested (indirectly) in exception tests for serialization")]
    protected UnknownUserRoleException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
