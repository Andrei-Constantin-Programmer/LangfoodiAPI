﻿using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Infrastructure.Mappers.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

namespace RecipeSocialMediaAPI.Infrastructure.Repositories.Messages;

public class GroupPersistenceRepository : IGroupPersistenceRepository
{
    private readonly IMongoCollectionWrapper<GroupDocument> _groupCollection;
    private readonly IGroupDocumentToModelMapper _mapper;

    public GroupPersistenceRepository(IMongoCollectionFactory mongoCollectionFactory, IGroupDocumentToModelMapper mapper)
    {
        _groupCollection = mongoCollectionFactory.CreateCollection<GroupDocument>();
        _mapper = mapper;
    }

    public async Task<Group> CreateGroupAsync(string groupName, string groupDescription, List<IUserAccount> users, CancellationToken cancellationToken = default)
    {
        GroupDocument groupDocument = await _groupCollection.InsertAsync(new(groupName, groupDescription, users.Select(user => user.Id).ToList()), cancellationToken);

        return await _mapper.MapGroupFromDocumentAsync(groupDocument, cancellationToken);
    }

    public async Task<bool> UpdateGroupAsync(Group group, CancellationToken cancellationToken = default)
    {
        return await _groupCollection.UpdateAsync(
            new GroupDocument(group.GroupName, group.GroupDescription, group.Users.Select(user => user.Id).ToList(), group.GroupId),
            groupDoc => groupDoc.Id == group.GroupId, cancellationToken);
    }

    public async Task<bool> DeleteGroupAsync(Group group, CancellationToken cancellationToken = default) 
        => await DeleteGroupAsync(group.GroupId, cancellationToken);

    public async Task<bool> DeleteGroupAsync(string groupId, CancellationToken cancellationToken = default) 
        => await _groupCollection.DeleteAsync(groupDoc => groupDoc.Id == groupId, cancellationToken);
}
