﻿using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

namespace RecipeSocialMediaAPI.Infrastructure.Exceptions;

[Serializable]
public class MalformedMessageDocumentException : Exception
{
    public MalformedMessageDocumentException(MessageDocument messageDocument)
        : base($"The message document with id {messageDocument.Id} is malformed")
    { }
}