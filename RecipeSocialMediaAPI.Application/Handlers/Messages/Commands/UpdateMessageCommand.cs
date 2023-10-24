using MediatR;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.Mappers.Messages.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Validation;
using RecipeSocialMediaAPI.Domain.Utilities;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record UpdateMessageCommand(UpdateMessageContract UpdateMessageContract) : IValidatableRequest;

internal class UpdateMessageHandler : IRequestHandler<UpdateMessageCommand>
{
    private readonly IMessageMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMessagePersistenceRepository _messagePersistenceRepository;
    private readonly IMessageQueryRepository _messageQueryRepository;

    public UpdateMessageHandler(IMessageMapper mapper, IDateTimeProvider dateTimeProvider, IMessagePersistenceRepository messagePersistenceRepository, IMessageQueryRepository messageQueryRepository)
    {
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
        _messagePersistenceRepository = messagePersistenceRepository;
        _messageQueryRepository = messageQueryRepository;
    }

    public Task Handle(UpdateMessageCommand request, CancellationToken cancellationToken) => throw new NotImplementedException();
}