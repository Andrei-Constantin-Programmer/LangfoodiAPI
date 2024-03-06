using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;

namespace RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
public interface IConnectionDocumentToModelMapper
{
    Task<IConnection> MapConnectionFromDocumentAsync(ConnectionDocument connectionDocument, CancellationToken cancellationToken = default);
}