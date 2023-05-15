using RecipeSocialMediaAPI.DTO;
using RecipeSocialMediaAPI.Services;

namespace RecipeSocialMediaAPI.Endpoints
{
    public static class UserEndpoints
    {
        public static void MapUserEndpoints(this WebApplication app)
        {
            app.MapPost("/users/createuser", (UserDTO newUser) =>
            {
                return TypedResults.Ok(true);
            });

            app.MapPost("/users/updateuser", (UserDTO user) =>
            {
                return TypedResults.Ok(true);
            });

            app.MapDelete("/users/removeuser/{userid}", (int userId) =>
            {
                return TypedResults.Ok(true);
            });

            app.MapPost("/users/username/exists", (UserDTO user) =>
            {
                return TypedResults.Ok(UserService.CheckUserNameExists(user));
            });
        }
    }
}
