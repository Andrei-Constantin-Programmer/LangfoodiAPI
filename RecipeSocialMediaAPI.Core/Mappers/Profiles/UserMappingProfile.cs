﻿using AutoMapper;
using RecipeSocialMediaAPI.Core.DTO;
using RecipeSocialMediaAPI.Domain;

namespace RecipeSocialMediaAPI.Core.Mappers.Profiles;

internal class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<UserDTO, User>();
        CreateMap<User, UserDTO>();
    }
}
