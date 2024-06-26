﻿using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

public record GetGroupsByUserQuery(string UserId) : IRequest<List<GroupDto>>;

internal class GetGroupsByUserHandler : IRequestHandler<GetGroupsByUserQuery, List<GroupDto>>
{
    private readonly IGroupQueryRepository _groupQueryRepository;
    private readonly IUserQueryRepository _userQueryRepository;

    public GetGroupsByUserHandler(IGroupQueryRepository groupQueryRepository, IUserQueryRepository userQueryRepository)
    {
        _groupQueryRepository = groupQueryRepository;
        _userQueryRepository = userQueryRepository;
    }

    public async Task<List<GroupDto>> Handle(GetGroupsByUserQuery request, CancellationToken cancellationToken)
    {
        IUserAccount user = (await _userQueryRepository.GetUserByIdAsync(request.UserId, cancellationToken))?.Account
            ?? throw new UserNotFoundException($"No user found with id {request.UserId}");

        return (await _groupQueryRepository
            .GetGroupsByUserAsync(user, cancellationToken))
            .Select(group => new GroupDto(
                group.GroupId,
                group.GroupName,
                group.GroupDescription,
                group.Users
                    .Select(user => user.Id)
                    .ToList()))
            .ToList();
    }
}
