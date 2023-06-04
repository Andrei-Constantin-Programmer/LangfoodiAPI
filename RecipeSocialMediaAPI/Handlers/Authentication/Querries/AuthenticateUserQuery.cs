using MediatR;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.MongoConfiguration;
using RecipeSocialMediaAPI.DAL.Repositories;

using BCrypter = BCrypt.Net.BCrypt;

namespace RecipeSocialMediaAPI.Handlers.Authentication.Querries;

internal record AuthenticateUserQuery(string UsernameOrEmail, string Password) : IRequest<bool>;

internal class AuthenticateUserHandler : IRequestHandler<AuthenticateUserQuery, bool>
{
    private readonly IMongoRepository<UserDocument> _userRepository;

    public AuthenticateUserHandler(IMongoCollectionFactory _collectionFactory)
    {
        _userRepository = _collectionFactory.GetCollection<UserDocument>();
    }

    public Task<bool> Handle(AuthenticateUserQuery request, CancellationToken cancellationToken)
    {
        UserDocument? user = _userRepository.Find(user => user.UserName == request.UsernameOrEmail)
                         ?? _userRepository.Find(user => user.Email == request.UsernameOrEmail);

        if (user is null)
        {
            return Task.FromResult(false);
        }

        bool successfulLogin = BCrypter.Verify(request.Password, user.Password);

        return Task.FromResult(successfulLogin);
    }
}