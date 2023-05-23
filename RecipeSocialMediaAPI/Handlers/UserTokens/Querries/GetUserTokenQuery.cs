using AutoMapper;
using MediatR;
using RecipeSocialMediaAPI.DAL;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Utilities;

namespace RecipeSocialMediaAPI.Handlers.UserTokens.Querries
{
    public record GetUserTokenQuery(UserDto User) : IRequest<UserTokenDto>;

    public class GetUserTokenHandler : IRequestHandler<GetUserTokenQuery, UserTokenDto>
    {
        private readonly IMapper _mapper;
        private readonly IMongoCollectionWrapper<UserDocument> _userCollection;
        private readonly IMongoCollectionWrapper<UserTokenDocument> _userTokenCollection;
        
        public GetUserTokenHandler(IMapper mapper, IMongoFactory factory, IConfigManager config)
        {
            _mapper = mapper;
            _userCollection = factory.GetCollection<UserDocument>(new UserRepository(), config);
            _userTokenCollection = factory.GetCollection<UserTokenDocument>(new UserTokenRepository(), config);
        }

        public Task<UserTokenDto> Handle(GetUserTokenQuery request, CancellationToken cancellationToken)
        {
            UserDocument? userDoc = null;
            if (request.User.Email != string.Empty)
            {
                userDoc = _userCollection.Find(x => x.Email.ToLower() == request.User.Email.ToLower());
            }
            else if (request.User.UserName != string.Empty)
            {
                userDoc = _userCollection.Find(x => x.UserName == request.User.UserName);
            }

            return Task.FromResult(_mapper.Map<UserTokenDto>(_userTokenCollection.Find(x => x.UserId == userDoc!._id)));
        }
    }
}
