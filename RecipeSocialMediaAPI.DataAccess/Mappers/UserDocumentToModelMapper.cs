﻿using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Model;

namespace RecipeSocialMediaAPI.DataAccess.Mappers;

public class UserDocumentToModelMapper : IUserDocumentToModelMapper
{
    public User MapUserDocumentToUser(UserDocument userDocument)
    {
        if (userDocument.Id is null)
        {
            throw new ArgumentException("Cannot map User Document with null ID to User");
        }

        return new(userDocument.Id, userDocument.UserName, userDocument.Email, userDocument.Password);
    }
}
