﻿namespace RecipeSocialMediaAPI.Core.Contracts;

public record UpdateUserContract
{
    required public string Id { get; set; }
    required public string UserName { get; set; }
    required public string Email { get; set; }
    required public string Password { get; set; }
}
