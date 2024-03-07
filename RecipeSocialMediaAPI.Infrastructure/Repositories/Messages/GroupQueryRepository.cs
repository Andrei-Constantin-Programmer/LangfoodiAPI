using Microsoft.Extensions.Logging;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Infrastructure.Mappers.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

namespace RecipeSocialMediaAPI.Infrastructure.Repositories.Messages;

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

    public async Task<Group?> GetGroupByIdAsync(string groupId, CancellationToken cancellationToken = default)
    {
        GroupDocument? groupDocument;
        try
        {
            groupDocument = await _groupCollection.GetOneAsync(
                groupDoc => groupDoc.Id == groupId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was an error trying to get the group with the id {GroupID}: {ErrorMessage}", groupId, ex.Message);
            return null;
        }
        
        return groupDocument is not null
            ? await _mapper.MapGroupFromDocumentAsync(groupDocument, cancellationToken)
            : null;
    }

    public async Task<IEnumerable<Group>> GetGroupsByUserAsync(IUserAccount userAccount, CancellationToken cancellationToken = default)
    {
        try
        {
            return await Task.WhenAll((await _groupCollection
                .GetAllAsync(groupDoc => groupDoc.UserIds.Contains(userAccount.Id), cancellationToken))
                .Select(group => _mapper.MapGroupFromDocumentAsync(group, cancellationToken)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was an error trying to get the groups for user with id {UserId}: {ErrorMessage}", userAccount.Id, ex.Message);
            return Enumerable.Empty<Group>();
        }
    }
}
