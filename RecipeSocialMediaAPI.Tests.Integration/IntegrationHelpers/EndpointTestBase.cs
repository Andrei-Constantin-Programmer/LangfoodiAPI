using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Tests.Integration.IntegrationHelpers.FakeDependencies;

namespace RecipeSocialMediaAPI.Tests.Integration.IntegrationHelpers
{
    public abstract class EndpointTestBase : IClassFixture<WebApplicationFactory<Program>>
    {
        protected readonly HttpClient _client;

        public EndpointTestBase(WebApplicationFactory<Program> factory)
        {
            _client = factory
                .WithWebHostBuilder(builder => builder.ConfigureServices(services =>
                {
                    services.AddSingleton<IRecipeRepository, FakeRecipeRepository>();
                }))
                .CreateClient();
        }
    }
}
