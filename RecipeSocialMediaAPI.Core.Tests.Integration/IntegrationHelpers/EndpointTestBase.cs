using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using RecipeSocialMediaAPI.Core.Tests.Integration.IntegrationHelpers.FakeDependencies;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;

namespace RecipeSocialMediaAPI.Core.Tests.Integration.IntegrationHelpers;

public abstract class EndpointTestBase : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly HttpClient _client;

    public EndpointTestBase(WebApplicationFactory<Program> factory)
    {
        _client = factory
            .WithWebHostBuilder(builder => builder.ConfigureServices(services =>
            {
                var fakeRecipeRepository = new FakeRecipeRepository();
                services.AddSingleton<IRecipeQueryRepository>(fakeRecipeRepository);
                services.AddSingleton<IRecipePersistenceRepository>(fakeRecipeRepository);

                var fakeUserRepository = new FakeUserRepository();
                services.AddSingleton<IUserQueryRepository>(fakeUserRepository);
                services.AddSingleton<IUserPersistenceRepository>(fakeUserRepository);
            }))
            .CreateClient();
    }
}
