using Microsoft.Extensions.Logging;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Infrastructure.Exceptions;
using RecipeSocialMediaAPI.Infrastructure.Mappers.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

namespace RecipeSocialMediaAPI.Infrastructure.Repositories.Messages;

public class MessageQueryRepository : IMessageQueryRepository
{
    private readonly ILogger<MessageQueryRepository> _logger;
    private readonly IMessageDocumentToModelMapper _mapper;
    private readonly IMongoCollectionWrapper<MessageDocument> _messageCollection;
    private readonly IUserQueryRepository _userQueryRepository;

    public MessageQueryRepository(ILogger<MessageQueryRepository> logger, IMessageDocumentToModelMapper mapper, IMongoCollectionFactory mongoCollectionFactory, IUserQueryRepository userQueryRepository)
    {
        _logger = logger;
        _mapper = mapper;
        _messageCollection = mongoCollectionFactory.CreateCollection<MessageDocument>();
        _userQueryRepository = userQueryRepository;
    }

    public async Task<Message?> GetMessageAsync(string id, CancellationToken cancellationToken = default)
    {
        MessageDocument? messageDocument;
        try
        {
            messageDocument = await _messageCollection.GetOneAsync(messageDoc => messageDoc.Id == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was an error trying to get message by id {MessageId}: {ErrorMessage}", id, ex.Message);
            messageDocument = null;
        }

        if (messageDocument is null)
        {
            return null;
        }

        IUserAccount? sender = await GetSenderAsync(messageDocument.SenderId, messageDocument.Id, cancellationToken);
        if (sender is null)
        {
            return null;
        }

        Message? repliedToMessage = await GetRepliedToMessageAsync(messageDocument, cancellationToken);

        return await _mapper.MapMessageFromDocumentAsync(messageDocument, sender, repliedToMessage, cancellationToken);
    }

    public async Task<IEnumerable<Message>> GetMessagesWithRecipeAsync(string recipeId, CancellationToken cancellationToken = default)
    {
        var messageDocuments = await _messageCollection.GetAllAsync(messageDoc 
            => messageDoc.MessageContent.RecipeIds != null 
            && messageDoc.MessageContent.RecipeIds.Contains(recipeId), cancellationToken);
        
        return (await Task.WhenAll(messageDocuments
            .Select(async messageDoc => {
                try
                {
                    return await _mapper.MapMessageFromDocumentAsync(
                        messageDoc,
                        await GetSenderAsync(messageDoc.SenderId, messageDoc.Id, cancellationToken)
                            ?? throw new UserDocumentNotFoundException(messageDoc.SenderId),
                        await GetRepliedToMessageAsync(messageDoc, cancellationToken),
                        cancellationToken
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "There was an error mapping message {MessageId}", messageDoc.Id);
                    return null;
                }
            })))
            .OfType<Message>();
    }

    private async Task<IUserAccount?> GetSenderAsync(string senderId, string? messageId, CancellationToken cancellationToken = default)
    {
        IUserAccount? sender = (await _userQueryRepository.GetUserByIdAsync(senderId, cancellationToken))?.Account;
        if (sender is null)
        {
            _logger.LogWarning("The sender with id {SenderId} was not found for message with id {MessageId}", senderId, messageId);
        }

        return sender;
    }

    private async Task<Message?> GetRepliedToMessageAsync(MessageDocument messageDocument, CancellationToken cancellationToken = default) => 
        messageDocument.MessageRepliedToId is not null
            ? await GetMessageAsync(messageDocument.MessageRepliedToId, cancellationToken)
            : null;
}
