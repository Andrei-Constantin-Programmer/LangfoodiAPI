﻿using RecipeSocialMediaAPI.Data.DTO;

namespace RecipeSocialMediaAPI.Services
{
    internal interface IUserValidationService
    {
        bool ValidPassword(string password);
        bool ValidEmail(string email);
        bool ValidUserName(string userName);
        Task<bool> ValidUserAsync(UserDto user);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }
}
