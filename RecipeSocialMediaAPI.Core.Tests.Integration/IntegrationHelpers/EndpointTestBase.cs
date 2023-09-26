using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using RecipeSocialMediaAPI.Core.Tests.Integration.IntegrationHelpers.FakeDependencies;
using RecipeSocialMediaAPI.Application.Repositories;
using RecipeSocialMediaAPI.Application.Repositories.Users;

namespace RecipeSocialMediaAPI.Core.Tests.Integration.IntegrationHelpers;

public abstract class EndpointTestBase : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly HttpClient _client;

    public EndpointTestBase(WebApplicationFactory<Program> factory)
    {
        _client = factory
            .WithWebHostBuilder(builder => builder.ConfigureServices(services =>
            {
                services.AddSingleton<IRecipeRepository, FakeRecipeRepository>();

                var fakeUserRepository = new FakeUserRepository();
                services.AddSingleton<IUserQueryRepository>(fakeUserRepository);
                services.AddSingleton<IUserPersistenceRepository>(fakeUserRepository);
            }))
            .CreateClient();
    }
}
