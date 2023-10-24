using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Message;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

public record GetMessageByIdQuery(string Id) : IRequest<MessageDTO>;