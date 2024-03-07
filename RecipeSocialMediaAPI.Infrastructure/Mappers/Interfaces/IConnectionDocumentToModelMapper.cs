using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;

namespace RecipeSocialMediaAPI.Infrastructure.Mappers.Interfaces;

public interface IConnectionDocumentToModelMapper
{
    Task<IConnection> MapConnectionFromDocumentAsync(ConnectionDocument connectionDocument, CancellationToken cancellationToken = default);
}