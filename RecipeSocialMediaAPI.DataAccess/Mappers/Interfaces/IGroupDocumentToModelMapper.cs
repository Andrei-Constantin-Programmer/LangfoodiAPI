using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Messaging;

namespace RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
public interface IGroupDocumentToModelMapper
{
    Group MapGroupFromDocument(GroupDocument groupDocument);
}