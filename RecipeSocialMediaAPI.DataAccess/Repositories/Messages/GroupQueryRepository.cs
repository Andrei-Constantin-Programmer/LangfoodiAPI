using Microsoft.Extensions.Logging;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.DataAccess.Repositories.Messages;

public class GroupQueryRepository : IGroupQueryRepository
{
    private readonly ILogger<GroupQueryRepository> _logger;
    private readonly IGroupDocumentToModelMapper _mapper;
    private readonly IMongoCollectionWrapper<GroupDocument> _groupCollection;

    public GroupQueryRepository(ILogger<GroupQueryRepository> logger, IGroupDocumentToModelMapper groupDocumentToModelMapper, IMongoCollectionFactory mongoCollectionFactory)
    {
        _logger = logger;
        _mapper = groupDocumentToModelMapper;
        _groupCollection = mongoCollectionFactory.CreateCollection<GroupDocument>();
    }

    public async Task<Group?> GetGroupById(string groupId, CancellationToken cancellationToken = default)
    {
        GroupDocument? groupDocument;
        try
        {
            groupDocument = await _groupCollection.Find(
                groupDoc => groupDoc.Id == groupId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was an error trying to get the group with the id {GroupID}: {ErrorMessage}", groupId, ex.Message);
            return null;
        }
        
        return groupDocument is not null
            ? await _mapper.MapGroupFromDocument(groupDocument, cancellationToken)
            : null;
    }

    public async Task<IEnumerable<Group>> GetGroupsByUser(IUserAccount userAccount, CancellationToken cancellationToken = default)
    {
        try
        {
            return await Task.WhenAll((await _groupCollection
                .GetAll(groupDoc => groupDoc.UserIds.Contains(userAccount.Id), cancellationToken))
                .Select(group => _mapper.MapGroupFromDocument(group, cancellationToken)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was an error trying to get the groups for user with id {UserId}: {ErrorMessage}", userAccount.Id, ex.Message);
            return Enumerable.Empty<Group>();
        }
    }
}
