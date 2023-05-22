using MediatR;
using RecipeSocialMediaAPI.DAL;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Utilities;

namespace RecipeSocialMediaAPI.Handlers.Users.Querries
{
    public record CheckEmailExistsQuery(UserDto User) : IRequest<bool>;

    public class CheckEmailExistsHandler : IRequestHandler<CheckEmailExistsQuery, bool>
    {
        private readonly IMongoCollection<UserDocument> _userCollection;

        public CheckEmailExistsHandler(IMongoFactory factory, IConfigManager config)
        {
            _userCollection = factory.GetCollection<UserDocument>(new UserRepository(), config);
        }

        public Task<bool> Handle(CheckEmailExistsQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_userCollection
                .Contains(x => x.Email.ToLower() == request.User.Email.ToLower()));
        }
    }
}
