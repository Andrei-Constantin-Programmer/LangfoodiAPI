﻿using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Repositories.Messages;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

public record GetGroupQuery(string GroupId) : IRequest<GroupDTO>;

internal class GetGroupHandler : IRequestHandler<GetGroupQuery, GroupDTO>
{
    private readonly IGroupQueryRepository _groupQueryRepository;

    public GetGroupHandler(IGroupQueryRepository groupQueryRepository)
    {
        _groupQueryRepository = groupQueryRepository;
    }

    public Task<GroupDTO> Handle(GetGroupQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
