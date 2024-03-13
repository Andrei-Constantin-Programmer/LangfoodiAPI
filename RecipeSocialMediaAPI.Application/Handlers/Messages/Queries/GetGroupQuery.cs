﻿using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

public record GetGroupQuery(string GroupId) : IRequest<GroupDTO>;

internal class GetGroupHandler : IRequestHandler<GetGroupQuery, GroupDTO>
{
    private readonly IGroupQueryRepository _groupQueryRepository;

    public GetGroupHandler(IGroupQueryRepository groupQueryRepository)
    {
        _groupQueryRepository = groupQueryRepository;
    }

    public async Task<GroupDTO> Handle(GetGroupQuery request, CancellationToken cancellationToken)
    {
        Group group = await _groupQueryRepository.GetGroupByIdAsync(request.GroupId, cancellationToken)
            ?? throw new GroupNotFoundException(request.GroupId);

        return new GroupDTO(
            group.GroupId,
            group.GroupName,
            group.GroupDescription, 
            group.Users
                .Select(user => user.Id)
                .ToList());
    }
}