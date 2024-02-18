using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Application.Mappers.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Users;

namespace RecipeSocialMediaAPI.Application.Handlers.Users.Queries;

public record GetUsersQuery(string ContainedString) : IRequest<List<UserAccountDTO>>;

internal class GetUsersHandler : IRequestHandler<GetUsersQuery, List<UserAccountDTO>>
{
    private readonly IUserQueryRepository _userQueryRepository;
    private readonly IUserMapper _userMapper;

    public GetUsersHandler(IUserQueryRepository userQueryRepository, IUserMapper userMapper)
    {
        _userQueryRepository = userQueryRepository;
        _userMapper = userMapper;
    }

    public async Task<List<UserAccountDTO>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(_userQueryRepository
            .GetAllUserAccountsContaining(request.ContainedString)
            .Select(_userMapper.MapUserAccountToUserAccountDto)
            .ToList());
    }
}
