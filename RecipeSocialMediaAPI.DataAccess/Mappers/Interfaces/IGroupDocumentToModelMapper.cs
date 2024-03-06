using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Messaging;

namespace RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
public interface IGroupDocumentToModelMapper
{
    Task<Group> MapGroupFromDocument(GroupDocument groupDocument, CancellationToken cancellationToken = default);
}