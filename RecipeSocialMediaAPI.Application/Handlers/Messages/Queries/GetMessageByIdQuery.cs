using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Mappers.Messages.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

public record GetMessageByIdQuery(string Id) : IRequest<MessageDto>;

internal class GetMessageByIdHandler : IRequestHandler<GetMessageByIdQuery, MessageDto>
{
    private readonly IMessageMapper _mapper;
    private readonly IMessageQueryRepository _messageQueryRepository;

    public GetMessageByIdHandler(IMessageMapper mapper, IMessageQueryRepository messageQueryRepository)
    {
        _mapper = mapper;
        _messageQueryRepository = messageQueryRepository;
    }

    public async Task<MessageDto> Handle(GetMessageByIdQuery request, CancellationToken cancellationToken)
    {
        Message message = (await _messageQueryRepository.GetMessageAsync(request.Id, cancellationToken))
            ?? throw new MessageNotFoundException(request.Id);

        return _mapper.MapMessageToMessageDTO(message);
    }
}
