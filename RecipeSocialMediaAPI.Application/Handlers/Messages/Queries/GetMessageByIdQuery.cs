using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

public record GetMessageByIdQuery(string Id) : IRequest<MessageDTO>;

internal class GetMessageByIdHandler : IRequestHandler<GetMessageByIdQuery, MessageDTO>
{
    private readonly IMessageQueryRepository _messageQueryRepository;

    public GetMessageByIdHandler(IMessageQueryRepository messageQueryRepository)
    {
        _messageQueryRepository = messageQueryRepository;
    }

    public Task<MessageDTO> Handle(GetMessageByIdQuery request, CancellationToken cancellationToken)
    {
        Message? message = _messageQueryRepository.GetMessage(request.Id);

        return message is null
            ? throw new MessageNotFoundException(request.Id)
            : throw new NotImplementedException();
    }
}