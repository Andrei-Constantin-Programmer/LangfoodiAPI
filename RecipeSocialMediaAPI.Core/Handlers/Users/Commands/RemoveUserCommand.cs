using MediatR;
using RecipeSocialMediaAPI.Application.Repositories;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Core.Handlers.Users.Commands;

public record RemoveUserCommand(string EmailOrId) : IRequest;

internal class RemoveUserHandler : IRequestHandler<RemoveUserCommand>
{
    private readonly IUserRepository _userRepository;

    public RemoveUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Task Handle(RemoveUserCommand request, CancellationToken cancellationToken)
    {
        User user = _userRepository.GetUserById(request.EmailOrId)
            ?? _userRepository.GetUserByEmail(request.EmailOrId)
            ?? throw new UserNotFoundException();

        bool isSuccessful = _userRepository.DeleteUser(user.Id);

        return isSuccessful
            ? Task.CompletedTask 
            : throw new Exception($"Could not remove user with id {user.Id}.");
    }
}
