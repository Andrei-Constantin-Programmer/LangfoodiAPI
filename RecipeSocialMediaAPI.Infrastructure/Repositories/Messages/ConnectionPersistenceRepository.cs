﻿using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Infrastructure.Mappers.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

namespace RecipeSocialMediaAPI.Infrastructure.Repositories.Messages;

public class ConnectionPersistenceRepository : IConnectionPersistenceRepository
{
    private readonly IConnectionDocumentToModelMapper _mapper;
    private readonly IMongoCollectionWrapper<ConnectionDocument> _connectionCollection;

    public ConnectionPersistenceRepository(IConnectionDocumentToModelMapper mapper, IMongoCollectionFactory mongoCollectionFactory)
    {
        _mapper = mapper;
        _connectionCollection = mongoCollectionFactory.CreateCollection<ConnectionDocument>();
    }

    public async Task<IConnection> CreateConnectionAsync(IUserAccount userAccount1, IUserAccount userAccount2, ConnectionStatus connectionStatus, CancellationToken cancellationToken = default)
    {
        ConnectionDocument connectionDocument = await _connectionCollection.InsertAsync(new ConnectionDocument(
            AccountId1: userAccount1.Id,
            AccountId2: userAccount2.Id,
            ConnectionStatus: connectionStatus.ToString()
        ), cancellationToken);

        return await _mapper.MapConnectionFromDocumentAsync(connectionDocument, cancellationToken);
    }

    public async Task<bool> UpdateConnectionAsync(IConnection connection, CancellationToken cancellationToken = default)
    {
        return await _connectionCollection.UpdateAsync(
            new ConnectionDocument(
                AccountId1: connection.Account1.Id,
                AccountId2: connection.Account2.Id,
                ConnectionStatus: connection.Status.ToString()
            ),
            doc => (doc.AccountId1 == connection.Account1.Id && doc.AccountId2 == connection.Account2.Id)
                || (doc.AccountId1 == connection.Account2.Id && doc.AccountId2 == connection.Account1.Id),
            cancellationToken);
    }

    public async Task<bool> DeleteConnectionAsync(IConnection connection, CancellationToken cancellationToken = default) 
        => await DeleteConnectionAsync(connection.Account1, connection.Account2, cancellationToken);

    public async Task<bool> DeleteConnectionAsync(IUserAccount userAccount1, IUserAccount userAccount2, CancellationToken cancellationToken = default) 
        => await _connectionCollection.DeleteAsync(
            doc => (doc.AccountId1 == userAccount1.Id && doc.AccountId2 == userAccount2.Id)
                || (doc.AccountId1 == userAccount2.Id && doc.AccountId2 == userAccount1.Id), 
            cancellationToken);
}
