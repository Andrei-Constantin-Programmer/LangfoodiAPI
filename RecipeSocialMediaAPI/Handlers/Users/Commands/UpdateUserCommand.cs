﻿using AutoMapper;
using MediatR;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.MongoConfiguration;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Endpoints;
using RecipeSocialMediaAPI.Services;
using RecipeSocialMediaAPI.Utilities;

namespace RecipeSocialMediaAPI.Handlers.Users.Commands
{
    public record UpdateUserCommand(UserDto User, string Token) : IRequest;

    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand>
    {
        private readonly IUserValidationService _userValidationService;
        private readonly IMapper _mapper;
        private readonly IUserTokenService _userTokenService;

        private readonly IMongoRepository<UserDocument> _userCollection;

        public UpdateUserHandler(IUserValidationService userValidationService, IMapper mapper, IUserTokenService userTokenService, IMongoCollectionFactory factory)
        {
            _userValidationService = userValidationService;
            _mapper = mapper;
            _userTokenService = userTokenService;

            _userCollection = factory.GetCollection<UserDocument>();
        }

        public Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            if (!_userTokenService.CheckTokenExistsAndNotExpired(request.Token))
            {
                throw new TokenNotFoundOrExpiredException();
            }

            request.User.Password = _userValidationService.HashPassword(request.User.Password);
            UserDocument newUserDoc = _mapper.Map<UserDocument>(request.User);
            newUserDoc._id = _userTokenService.GetUserFromToken(request.Token)._id;

            var result = _userCollection.UpdateRecord(newUserDoc, x => x._id == newUserDoc._id);

            if (!result)
            {
                throw new Exception($"Could not update user with id {newUserDoc._id}.");
            }

            return Task.CompletedTask;
        }
    }
}
