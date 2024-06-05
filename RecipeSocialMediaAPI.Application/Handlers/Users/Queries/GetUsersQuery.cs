using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Mappers.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Application.Utilities;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Handlers.Users.Queries;

public record GetUsersQuery(string UserId, string ContainedString, UserQueryOptions QueryOptions) : IRequest<List<UserAccountDto>>;

internal class GetUsersHandler : IRequestHandler<GetUsersQuery, List<UserAccountDto>>
{
    private readonly IUserQueryRepository _userQueryRepository;
    private readonly IUserMapper _userMapper;
    private readonly IConnectionQueryRepository _connectionQueryRepository;

    public GetUsersHandler(IUserQueryRepository userQueryRepository, IUserMapper userMapper, IConnectionQueryRepository connectionQueryRepository)
    {
        _userQueryRepository = userQueryRepository;
        _userMapper = userMapper;
        _connectionQueryRepository = connectionQueryRepository;
    }

    public async Task<List<UserAccountDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        IUserAccount queryingUser = (await _userQueryRepository.GetUserByIdAsync(request.UserId, cancellationToken))?.Account
            ?? throw new UserNotFoundException($"No user found with id {request.UserId}");

        var allUsers = await _userQueryRepository.GetAllUserAccountsContainingAsync(request.ContainedString, cancellationToken);
        var connections = (await _connectionQueryRepository.GetConnectionsForUserAsync(queryingUser, cancellationToken)).ToList();
        var usersFound = GetFilteredUsers(queryingUser, allUsers, request.QueryOptions, connections);

        return usersFound
            .Select(_userMapper.MapUserAccountToUserAccountDto)
            .ToList();
    }

    private static IEnumerable<IUserAccount> GetFilteredUsers(
        IUserAccount queryingUser,
        IEnumerable<IUserAccount> allUsers,
        UserQueryOptions queryOptions,
        List<IConnection> connections) => queryOptions switch
        {
            UserQueryOptions.All => allUsers,
            UserQueryOptions.NonSelf => allUsers.Where(user => user.Id != queryingUser.Id),
            UserQueryOptions.Connected => GetUsersFilteredByConnection(
                queryingUser,
                allUsers,
                user => connections
                    .Any(conn => conn.Account1.Id == user.Id
                              || conn.Account2.Id == user.Id)),
            UserQueryOptions.NotConnected => GetUsersFilteredByConnection(
                queryingUser,
                allUsers,
                user => connections
                    .All(conn => conn.Account1.Id != user.Id
                              && conn.Account2.Id != user.Id)),

            _ => throw new ArgumentException($"Unsupported query options {queryOptions}")
        };

    private static IEnumerable<IUserAccount> GetUsersFilteredByConnection(
        IUserAccount queryingUser,
        IEnumerable<IUserAccount> allUsers,
        Predicate<IUserAccount> condition)
        => allUsers.Where(user => user.Id != queryingUser.Id && condition(user));
}
