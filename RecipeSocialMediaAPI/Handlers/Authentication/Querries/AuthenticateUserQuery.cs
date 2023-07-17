using AutoMapper;
using MediatR;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.MongoConfiguration;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.DTO;
using RecipeSocialMediaAPI.Exceptions;
using RecipeSocialMediaAPI.Handlers.Users.Commands;
using BCrypter = BCrypt.Net.BCrypt;

namespace RecipeSocialMediaAPI.Handlers.Authentication.Querries;

internal record AuthenticateUserQuery(string UsernameOrEmail, string Password) : IRequest<UserDTO>;

internal class AuthenticateUserHandler : IRequestHandler<AuthenticateUserQuery, UserDTO>
{
    private readonly IMongoRepository<UserDocument> _userRepository;
    private readonly IMapper _mapper;

    public AuthenticateUserHandler(IMongoCollectionFactory _collectionFactory, IMapper mapper)
    {
        _userRepository = _collectionFactory.GetCollection<UserDocument>();
        _mapper = mapper;
    }

    public Task<UserDTO> Handle(AuthenticateUserQuery request, CancellationToken cancellationToken)
    {
        UserDocument? user = _userRepository.Find(user => user.UserName == request.UsernameOrEmail)
                         ?? _userRepository.Find(user => user.Email == request.UsernameOrEmail);

        if (user is null)
        {
            throw new UserNotFoundException();
        }

        var successfulLogin = BCrypter.Verify(request.Password, user?.Password);
        if (!successfulLogin)
        {
            throw new InvalidCredentialsException();
        }

        return Task.FromResult(_mapper.Map<UserDTO>(user));
    }
}
