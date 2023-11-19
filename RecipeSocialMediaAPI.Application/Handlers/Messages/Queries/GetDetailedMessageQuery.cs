using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Mappers.Messages.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;

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
