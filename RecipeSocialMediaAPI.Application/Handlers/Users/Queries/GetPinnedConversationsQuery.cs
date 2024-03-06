using MediatR;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Handlers.Users.Queries;

public record GetPinnedConversationsQuery(string UserId) : IRequest<List<string>>;

internal class GetPinnedConversationsHandler : IRequestHandler<GetPinnedConversationsQuery, List<string>>
{
    private readonly IUserQueryRepository _userQueryRepository;

    public GetPinnedConversationsHandler(IUserQueryRepository userQueryRepository)
    {
        _userQueryRepository = userQueryRepository;
    }

    public async Task<List<string>> Handle(GetPinnedConversationsQuery request, CancellationToken cancellationToken)
    {
        IUserAccount user = (await _userQueryRepository.GetUserByIdAsync(request.UserId, cancellationToken))?.Account
            ?? throw new UserNotFoundException($"User with id {request.UserId} does not exist");

        return await Task.FromResult(user.PinnedConversationIds.ToList());
    }
}
