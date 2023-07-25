using RecipeSocialMediaAPI.DataAccess.MongoDocuments;

namespace RecipeSocialMediaAPI.DataAccess.Exceptions;

[Serializable]
public class DocumentAlreadyExistsException<TDocument> : Exception where TDocument : MongoDocument
{
    public TDocument Document { get; }

    public DocumentAlreadyExistsException(TDocument document) : base($"{document.GetType()} already exists with id {document.Id}")
    {
        Document = document;
    }
}
