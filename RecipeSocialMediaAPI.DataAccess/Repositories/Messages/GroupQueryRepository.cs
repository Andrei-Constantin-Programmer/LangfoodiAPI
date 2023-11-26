using Microsoft.Extensions.Logging;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Users;
using System.Text.RegularExpressions;

namespace RecipeSocialMediaAPI.DataAccess.Repositories.Messages;
internal class GroupQueryRepository
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

    public Group GetGroupById(string groupId)
    {

        GroupDocument? groupDocument;
        try
        {
            groupDocument = _groupCollection.Find(
                groupDoc => groupDoc.GroupId == groupId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was an error trying to get the group with the id {GroupID}: {ErrorMessage}", groupId, ex.Message);
            return null;
        }

        return groupDocument is not null
            ? _mapper.MapGroupFromDocument(groupDocument)
            : null;
    }

    public List<Group> GetGroupsByUser(IUserAccount userAccount)
    {
        List<GroupDocument> groups = new();
        try
        {

            groups = _groupCollection
                .GetAll(groupDoc => groupDoc.UserIds.Contains(userAccount.Id));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was an error trying to get the groups for user with id {UserId}: {ErrorMessage}", userAccount.Id, ex.Message);
        }

        return groups
            .Select(_mapper.MapGroupFromDocument)
            .ToList();
    }
}
