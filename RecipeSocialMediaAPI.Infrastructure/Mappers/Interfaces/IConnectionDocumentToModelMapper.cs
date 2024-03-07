using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

namespace RecipeSocialMediaAPI.Infrastructure.Mappers.Interfaces;

public interface IConnectionDocumentToModelMapper
{
    Task<IConnection> MapConnectionFromDocumentAsync(ConnectionDocument connectionDocument, CancellationToken cancellationToken = default);
}