using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Core.Contracts;
using RecipeSocialMediaAPI.Core.DTO;
using RecipeSocialMediaAPI.Core.Handlers.Users.Commands;
using RecipeSocialMediaAPI.Core.Handlers.Users.Queries;

namespace RecipeSocialMediaAPI.Core.Endpoints;

public static class EndpointGroups
{
    public static RouteGroupBuilder UserEndpoints(this RouteGroupBuilder group)
    {

        return group;
    }

    public static RouteGroupBuilder RecipeEndpoints(this RouteGroupBuilder group)
    {

        return group;
    }

    public static RouteGroupBuilder AuthenticationEndpoints(this RouteGroupBuilder group)
    {

        return group;
    }

    public static RouteGroupBuilder TestEndpoints(this RouteGroupBuilder group)
    {

        return group;
    }
}
