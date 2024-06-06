using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Infrastructure.Exceptions;

[Serializable]
public class DocumentAlreadyExistsException<TDocument> : Exception where TDocument : MongoDocument
{
    private const string DOCUMENT_PROPERTY_NAME = "Document";
    public TDocument Document { get; }

    public DocumentAlreadyExistsException(TDocument document) : base($"{document.GetType()} already exists with id {document.Id}")
    {
        Document = document;
    }

    protected DocumentAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        Document = info.GetValue(DOCUMENT_PROPERTY_NAME, typeof(TDocument)) as TDocument ?? default!;
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(DOCUMENT_PROPERTY_NAME, Document);
    }
}
