using Moq;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;

namespace RecipeSocialMediaAPI.Core.Tests.Integration.IntegrationHelpers.FakeDependencies;

internal class FakeMessageRepository : IMessageQueryRepository, IMessagePersistenceRepository
{
    private readonly IMessageFactory _messageFactory;
    private readonly IRecipeQueryRepository _recipeQueryRepository;

    private readonly List<Message> _collection;

    public FakeMessageRepository(IMessageFactory messageFactory, IRecipeQueryRepository recipeQueryRepository)
    {
        _messageFactory = messageFactory;
        _recipeQueryRepository = recipeQueryRepository;

        _collection = new();
    }

    public async Task<Message?> GetMessage(string id, CancellationToken cancellationToken = default) => await Task.FromResult(_collection.FirstOrDefault(m => m.Id == id));

    public async Task<IEnumerable<Message>> GetMessagesWithRecipe(string recipeId, CancellationToken cancellationToken = default) => await Task.FromResult(_collection
        .Where(m => m is RecipeMessage message 
                 && message.Recipes.Any(r => r.Id == recipeId)));

    public async Task<Message> CreateMessage(IUserAccount sender, string? text, List<string>? recipeIds, List<string>? imageURLs, DateTimeOffset sentDate, Message? messageRepliedTo, List<string> seenByUserIds, CancellationToken cancellationToken = default)
    {
        var id = _collection.Count.ToString();
        Message message;

        if (recipeIds?.Count > 0)
        {
            var recipes = (await Task.WhenAll(recipeIds
                .Select(async id => await _recipeQueryRepository.GetRecipeById(id, cancellationToken)!)))
                .OfType<RecipeAggregate>();

            message = _messageFactory
                .CreateRecipeMessage(id, sender, recipes, text, new(), sentDate, repliedToMessage: messageRepliedTo);
        }
        else if (imageURLs?.Count > 0)
        {
            message = _messageFactory.CreateImageMessage(id, sender, imageURLs, text, new(), sentDate, repliedToMessage: messageRepliedTo);
        }
        else
        {
            message = _messageFactory.CreateTextMessage(id, sender, text!, new(), sentDate, repliedToMessage: messageRepliedTo);
        }

        _collection.Add(message);
        return message;
    }

    public async Task<bool> UpdateMessage(Message message, CancellationToken cancellationToken = default)
    {
        Message? existingMessage = _collection.FirstOrDefault(x => x.Id == message.Id);
        if (existingMessage is null)
        {
            return false;
        }

        _collection.Remove(existingMessage);
        _collection.Add(message);

        return await Task.FromResult(true);
    }

    public bool DeleteMessage(Message message) => _collection.Remove(message);

    public bool DeleteMessage(string messageId)
    {
        var message = GetMessage(messageId).Result;

        return message is not null && _collection.Remove(message);
    }
}
