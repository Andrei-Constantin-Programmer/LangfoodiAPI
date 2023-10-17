using Microsoft.Extensions.Logging;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.DataAccess.Exceptions;
using RecipeSocialMediaAPI.DataAccess.Mappers;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.DataAccess.Repositories.Messages;

public class MessageQueryRepository
{
    private readonly ILogger<MessageQueryRepository> _logger;
    private readonly IMessageDocumentToModelMapper _mapper;
    private readonly IMongoCollectionWrapper<MessageDocument> _messageCollection;
    private readonly IUserQueryRepository _userQueryRepository;
    private readonly IRecipeQueryRepository _recipeQueryRepository;

    public MessageQueryRepository(ILogger<MessageQueryRepository> logger, IMessageDocumentToModelMapper mapper, IMongoCollectionFactory mongoCollectionFactory, IUserQueryRepository userQueryRepository, IRecipeQueryRepository recipeQueryRepository)
    {
        _logger = logger;
        _mapper = mapper;
        _messageCollection = mongoCollectionFactory.CreateCollection<MessageDocument>();
        _userQueryRepository = userQueryRepository;
        _recipeQueryRepository = recipeQueryRepository;
    }

    public Message? GetMessage(string id)
    {
        MessageDocument? messageDocument;
        try
        {
            messageDocument = _messageCollection.Find(messageDoc => messageDoc.Id == id);
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

        IUserAccount? sender = _userQueryRepository.GetUserById(messageDocument.SenderId)?.Account;
        if (sender is null)
        {
            _logger.LogWarning("The sender with id {SenderId} was not found for message with id {MessageId}", messageDocument.SenderId, messageDocument.Id);
            return null;
        }

        Message? repliedToMessage = messageDocument.MessageRepliedToId is not null 
                                    ? GetMessage(messageDocument.MessageRepliedToId) 
                                    : null;
        
        return MapMessageFromDocument(messageDocument, sender, repliedToMessage);
    }

    private Message MapMessageFromDocument(MessageDocument messageDocument, IUserAccount sender, Message? repliedToMessage)
    {
        var (text, recipeIds, imageURLs) = messageDocument.MessageContent;
        return (text, recipeIds, imageURLs) switch
        {
            (object, null, null) => _mapper.MapMessageDocumentToTextMessage(messageDocument, sender, repliedToMessage),
            (_, null, object) => _mapper.MapMessageDocumentToImageMessage(messageDocument, sender, repliedToMessage),
            (_, object, null) => GetRecipeMessage(repliedToMessage),

            _ => throw new MalformedMessageDocumentException(messageDocument)
        };

        Message GetRecipeMessage(Message? repliedToMessage)
        {
            var recipes = recipeIds
                .Select(_recipeQueryRepository.GetRecipeById)
                .OfType<RecipeAggregate>();

            return _mapper.MapMessageDocumentToRecipeMessage(messageDocument, sender, recipes, repliedToMessage);
        }
    }
}
