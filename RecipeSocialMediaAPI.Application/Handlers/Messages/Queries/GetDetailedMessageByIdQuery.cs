using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Mappers.Messages.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

public record GetMessageDetailedByIdQuery(string Id) : IRequest<MessageDetailedDTO>;

internal class GetMessageDetailedByIdHandler : IRequestHandler<GetMessageDetailedByIdQuery, MessageDetailedDTO>
{
    private readonly IMessageMapper _mapper;
    private readonly IMessageQueryRepository _messageQueryRepository;

    public GetMessageDetailedByIdHandler(IMessageMapper mapper, IMessageQueryRepository messageQueryRepository)
    {
        _mapper = mapper;
        _messageQueryRepository = messageQueryRepository;
    }

    public async Task<MessageDetailedDTO> Handle(GetMessageDetailedByIdQuery request, CancellationToken cancellationToken)
    {
        Message message = _messageQueryRepository.GetMessage(request.Id)
            ?? throw new MessageNotFoundException(request.Id);

        return await Task.FromResult(_mapper.MapMessageToDetailedMessageDTO(message));
    }
}
