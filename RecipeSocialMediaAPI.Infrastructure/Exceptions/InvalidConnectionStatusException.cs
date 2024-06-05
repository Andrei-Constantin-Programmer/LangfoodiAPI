using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Infrastructure.Exceptions;

[Serializable]
public class InvalidConnectionStatusException : Exception
{
    public InvalidConnectionStatusException(ConnectionDocument connectionDocument)
        : base($"The connection document with id {connectionDocument.Id} has an invalid status: {connectionDocument.ConnectionStatus}")
    { }

    protected InvalidConnectionStatusException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}