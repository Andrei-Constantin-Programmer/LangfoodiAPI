using MediatR;
using RecipeSocialMediaAPI.Services;

namespace RecipeSocialMediaAPI.Handlers.UserTokens.Querries
{
    internal record GetIsValidUserTokenQuery(string Token) : IRequest<bool>;

    internal class GetIsValidUserTokenHandler : IRequestHandler<GetIsValidUserTokenQuery, bool>
    {
        private readonly IUserTokenService _userTokenService;

        public GetIsValidUserTokenHandler(IUserTokenService userTokenService)
        {
            _userTokenService = userTokenService;
        }

        public Task<bool> Handle(GetIsValidUserTokenQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_userTokenService.CheckTokenExistsAndNotExpired(request.Token));
        }
    }
}
