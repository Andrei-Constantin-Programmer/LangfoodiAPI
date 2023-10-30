﻿using Microsoft.Extensions.Logging;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.DataAccess.Repositories.Messages;

public class ConnectionQueryRepository : IConnectionQueryRepository
{
    private readonly ILogger<ConnectionQueryRepository> _logger;
    private readonly IConnectionDocumentToModelMapper _mapper;
    private readonly IMongoCollectionWrapper<ConnectionDocument> _connectionCollection;

    public ConnectionQueryRepository(ILogger<ConnectionQueryRepository> logger, IConnectionDocumentToModelMapper connectionDocumentToModelMapper, IMongoCollectionFactory mongoCollectionFactory)
    {
        _logger = logger;
        _mapper = connectionDocumentToModelMapper;
        _connectionCollection = mongoCollectionFactory.CreateCollection<ConnectionDocument>();
    }

    public Connection? GetConnection(IUserAccount userAccount1, IUserAccount userAccount2)
    {
        ConnectionDocument? connectionDocument;
        try
        {
            connectionDocument = _connectionCollection.Find(
                connectionDoc => connectionDoc.AccountId1 == userAccount1.Id 
                                 && connectionDoc.AccountId2 == userAccount2.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was an error trying to get connection between users with ids {UserId1} and {UserId2}: {ErrorMessage}", userAccount1.Id, userAccount2.Id, ex.Message);
            return null;
        }

        return connectionDocument is not null
            ? _mapper.MapConnectionFromDocument(connectionDocument) 
            : null;
    }

    public List<Connection> GetConnectionsForUser(IUserAccount userAccount)
    {
        throw new NotImplementedException();
    }
}
