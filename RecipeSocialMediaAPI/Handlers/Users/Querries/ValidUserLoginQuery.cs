﻿using MediatR;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.MongoConfiguration;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Data.DTO;
using BCrypter = BCrypt.Net.BCrypt;

namespace RecipeSocialMediaAPI.Handlers.Users.Queries
{
    internal record ValidUserLoginQuery(UserDTO User) : IRequest<bool>;

    internal class ValidUserLoginHandler : IRequestHandler<ValidUserLoginQuery, bool>
    {
        private readonly IMongoRepository<UserDocument> _userCollection;

        public ValidUserLoginHandler(IMongoCollectionFactory collectionFactory)
        {
            _userCollection = collectionFactory.GetCollection<UserDocument>();
        }

        public Task<bool> Handle(ValidUserLoginQuery request, CancellationToken cancellationToken)
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

            return Task.FromResult(userDoc != null
                && BCrypter.Verify(request.User.Password, userDoc.Password));
        }
    }
}