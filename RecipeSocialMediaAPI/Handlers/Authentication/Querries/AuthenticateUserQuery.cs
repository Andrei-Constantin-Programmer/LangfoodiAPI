using AutoMapper;
using MediatR;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.MongoConfiguration;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Handlers.Users.Commands;
using BCrypter = BCrypt.Net.BCrypt;

namespace RecipeSocialMediaAPI.Handlers.Authentication.Querries;

internal record AuthenticateUserQuery(string UsernameOrEmail, string Password) : IRequest<UserDto>;

internal class AuthenticateUserHandler : IRequestHandler<AuthenticateUserQuery, UserDto>
{
    private readonly IMongoRepository<UserDocument> _userRepository;
    private readonly IMapper _mapper;

    public AuthenticateUserHandler(IMongoCollectionFactory _collectionFactory, IMapper mapper)
    {
        _userRepository = _collectionFactory.GetCollection<UserDocument>();
        _mapper = mapper;
    }

    public Task<UserDto> Handle(AuthenticateUserQuery request, CancellationToken cancellationToken)
    {
        UserDocument? user = _userRepository.Find(user => user.UserName == request.UsernameOrEmail)
                         ?? _userRepository.Find(user => user.Email == request.UsernameOrEmail);

        var successfulLogin = BCrypter.Verify(request.Password, user?.Password);

        if (user is null
            || !successfulLogin)
        {
            throw new InvalidCredentialsException();
        }

        return Task.FromResult(_mapper.Map<UserDto>(user));
    }
}
