using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Message;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

public record GetDetailedMessageQuery(string Id) : IRequest<MessageDTO?>;

internal class GetDetailedMessageHandler
{
    public GetDetailedMessageHandler()
    {

    }

    public async Task<> Handle()
    {
        return null;
    }

}
