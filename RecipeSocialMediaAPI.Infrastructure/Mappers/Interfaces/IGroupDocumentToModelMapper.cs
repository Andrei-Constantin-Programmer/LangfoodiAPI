using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

namespace RecipeSocialMediaAPI.Infrastructure.Mappers.Interfaces;

public interface IGroupDocumentToModelMapper
{
    Task<Group> MapGroupFromDocumentAsync(GroupDocument groupDocument, CancellationToken cancellationToken = default);
}