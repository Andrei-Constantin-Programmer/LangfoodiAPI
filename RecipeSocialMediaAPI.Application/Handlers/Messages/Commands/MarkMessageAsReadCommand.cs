using MediatR;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record MarkMessageAsReadCommand(string UserId, string MessageId) : IRequest;

internal class MarkMessageAsReadHandler : IRequestHandler<MarkMessageAsReadCommand>
{
    private readonly IUserQueryRepository _userQueryRepository;
    private readonly IMessageQueryRepository _messageQueryRepository;
    private readonly IMessagePersistenceRepository _messagePersistenceRepository;

    public MarkMessageAsReadHandler(IUserQueryRepository userQueryRepository, IMessageQueryRepository messageQueryRepository, IMessagePersistenceRepository messagePersistenceRepository)
    {
        _userQueryRepository = userQueryRepository;
        _messageQueryRepository = messageQueryRepository;
        _messagePersistenceRepository = messagePersistenceRepository;
    }

    public Task Handle(MarkMessageAsReadCommand request, CancellationToken cancellationToken)
    {
        var user = _userQueryRepository.GetUserById(request.UserId)
            ?? throw new UserNotFoundException($"No User found with id {request.UserId}");
        var message = _messageQueryRepository.GetMessage(request.MessageId)
            ?? throw new MessageNotFoundException($"No Message found with id {request.MessageId}");

        message.MarkAsSeenBy(user.Account);

        _messagePersistenceRepository.UpdateMessage(message);

        return Task.CompletedTask;
    }
}