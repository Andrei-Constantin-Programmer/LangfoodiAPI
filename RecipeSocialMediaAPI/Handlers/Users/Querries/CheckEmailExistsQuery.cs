using MediatR;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Services;

namespace RecipeSocialMediaAPI.Handlers.Users.Querries
{
    internal record CheckEmailExistsQuery(UserDto User) : IRequest<bool>;

    internal class CheckEmailExistsHandler : IRequestHandler<CheckEmailExistsQuery, bool>
    {
        private readonly IUserService _userService;

        public CheckEmailExistsHandler(IUserService userService)
        {
            _userService = userService;
        }

        public Task<bool> Handle(CheckEmailExistsQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_userService.DoesEmailExist(request.User.Email));
        }
    }
}
