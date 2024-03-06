﻿using Microsoft.Extensions.Logging;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.DataAccess.Repositories.Messages;

public class ConnectionPersistenceRepository : IConnectionPersistenceRepository
{
    private readonly IConnectionDocumentToModelMapper _mapper;
    private readonly IMongoCollectionWrapper<ConnectionDocument> _connectionCollection;

    public ConnectionPersistenceRepository(IConnectionDocumentToModelMapper mapper, IMongoCollectionFactory mongoCollectionFactory)
    {
        _mapper = mapper;
        _connectionCollection = mongoCollectionFactory.CreateCollection<ConnectionDocument>();
    }

    public async Task<IConnection> CreateConnection(IUserAccount userAccount1, IUserAccount userAccount2, ConnectionStatus connectionStatus, CancellationToken cancellationToken = default)
    {
        ConnectionDocument connectionDocument = await _connectionCollection.Insert(new ConnectionDocument(
            AccountId1: userAccount1.Id,
            AccountId2: userAccount2.Id,
            ConnectionStatus: connectionStatus.ToString()
        ), cancellationToken);

        return await _mapper.MapConnectionFromDocument(connectionDocument, cancellationToken);
    }

    public bool UpdateConnection(IConnection connection)
    {
        return _connectionCollection.UpdateRecord(
            new ConnectionDocument(
                AccountId1: connection.Account1.Id,
                AccountId2: connection.Account2.Id,
                ConnectionStatus: connection.Status.ToString()
            ),
            doc => (doc.AccountId1 == connection.Account1.Id && doc.AccountId2 == connection.Account2.Id)
                || (doc.AccountId1 == connection.Account2.Id && doc.AccountId2 == connection.Account1.Id));
    }

    public bool DeleteConnection(IConnection connection) => DeleteConnection(connection.Account1, connection.Account2);

    public bool DeleteConnection(IUserAccount userAccount1, IUserAccount userAccount2) 
        => _connectionCollection.Delete(
            doc => (doc.AccountId1 == userAccount1.Id && doc.AccountId2 == userAccount2.Id)
                || (doc.AccountId1 == userAccount2.Id && doc.AccountId2 == userAccount1.Id));
}
