using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Messaging;

namespace RecipeSocialMediaAPI.Infrastructure.Mappers.Interfaces;
public interface IGroupDocumentToModelMapper
{
    Task<Group> MapGroupFromDocumentAsync(GroupDocument groupDocument, CancellationToken cancellationToken = default);
}