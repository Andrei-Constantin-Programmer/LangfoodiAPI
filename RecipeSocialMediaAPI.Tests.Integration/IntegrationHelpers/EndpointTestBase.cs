using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using RecipeSocialMediaAPI.Core.Tests.Integration.IntegrationHelpers.FakeDependencies;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;

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
                services.AddSingleton<IUserRepository, FakeUserRepository>();
            }))
            .CreateClient();
    }
}
