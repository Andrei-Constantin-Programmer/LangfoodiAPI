﻿using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.DAL;
using RecipeSocialMediaAPI.Utilities;
using MongoDB.Bson;
using RecipeSocialMediaAPI.Services.Interfaces;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.DAL.Documents;

namespace RecipeSocialMediaAPI.Services
{
    public class UserTokenService : IUserTokenService
    {
        private readonly IMongoCollectionWrapper<UserTokenDocument> _userTokenCollection;
        private readonly IMongoCollectionWrapper<UserDocument> _userCollection;
        private readonly IClock _clock;

        public UserTokenService(IMongoFactory factory, IConfigManager config, IClock clock)
        {
            _userTokenCollection = factory.GetCollection<UserTokenDocument>(new UserTokenRepository(), config);
            _userCollection = factory.GetCollection<UserDocument>(new UserRepository(), config);
            _clock = clock;
        }

        public UserDocument GetUserFromTokenWithPassword(string token)
        {
            ObjectId tokenObj = ObjectId.Parse(token);
            UserTokenDocument userToken = _userTokenCollection.Find(x => x._id == tokenObj)!;

            return _userCollection.Find(x => x._id == userToken.UserId)!;
        }

        public UserDocument GetUserFromToken(string token)
        {
            UserDocument user = GetUserFromTokenWithPassword(token);
            user.Password = "";

            return user;
        }

        #region Validation Methods
        public bool CheckValidToken(string token)
        {
            return CheckTokenExists(token) 
                && !CheckTokenExpired(token);
        }

        public bool CheckTokenExists(UserDto user)
        {
            UserDocument? userDoc = null;
            if (user.Email != string.Empty)
            {
                userDoc = _userCollection.Find(x => x.Email.ToLower() == user.Email.ToLower());
            }
            else if (user.UserName != string.Empty)
            {
                userDoc = _userCollection.Find(x => x.UserName == user.UserName);
            }

            return _userTokenCollection.Contains(x => x.UserId == userDoc!._id);
        }

        public bool CheckTokenExpired(UserDto user)
        {
            UserDocument? userDoc = null;
            if (user.Email != string.Empty)
            {
                userDoc = _userCollection.Find(x => x.Email.ToLower() == user.Email.ToLower());
            }
            else if (user.UserName != string.Empty)
            {
                userDoc = _userCollection.Find(x => x.UserName == user.UserName);
            }
            UserTokenDocument tokenDoc = _userTokenCollection.Find(x => x.UserId == userDoc!._id)!;

            return _clock.Now >= tokenDoc.ExpiryDate;
        }

        public bool CheckTokenExpired(string token)
        {
            ObjectId tokenObj = ObjectId.Parse(token);
            UserTokenDocument tokenDoc = _userTokenCollection.Find(x => x._id! == tokenObj)!;

            return _clock.Now >= tokenDoc.ExpiryDate;
        }

        public bool CheckTokenExists(string token)
        {
            ObjectId tokenObj = ObjectId.Parse(token);

            return _userTokenCollection.Contains(x => x._id! == tokenObj);
        }
        #endregion
    }
}
