using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using RecipeSocialMediaAPI.Core.Tests.Integration.IntegrationHelpers.FakeDependencies;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using Moq;
using RecipeSocialMediaAPI.Application.WebClients.Interfaces;
using RecipeSocialMediaAPI.Application.Services.Interfaces;
using RecipeSocialMediaAPI.Application.Tests.Unit.TestHelpers;
using RecipeSocialMediaAPI.Application.Cryptography.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Services;
using RecipeSocialMediaAPI.Domain.Utilities;
using RecipeSocialMediaAPI.Application.Utilities;

namespace RecipeSocialMediaAPI.Core.Tests.Integration.IntegrationHelpers;

public abstract class EndpointTestBase : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly Mock<ICloudinarySignatureService> _cloudinarySignatureServiceMock;
    protected readonly Mock<ICloudinaryWebClient> _cloudinaryWebClientMock;
    protected readonly HttpClient _client;

    internal IDateTimeProvider _dateTimeProvider;
    internal FakeRecipeRepository _fakeRecipeRepository;
    internal FakeUserRepository _fakeUserRepository;
    internal FakeConnectionRepository _fakeConnectionRepository;
    internal FakeMessageRepository _fakeMessageRepository;

    internal FakeCryptoService _fakeCryptoService; 

    public EndpointTestBase(WebApplicationFactory<Program> factory)
    {
        _cloudinaryWebClientMock = new Mock<ICloudinaryWebClient>();
        _cloudinarySignatureServiceMock = new Mock<ICloudinarySignatureService>();

        _dateTimeProvider = new DateTimeProvider();

        _fakeRecipeRepository = new FakeRecipeRepository();
        _fakeUserRepository = new FakeUserRepository();
        _fakeConnectionRepository = new FakeConnectionRepository();
        _fakeMessageRepository = new FakeMessageRepository(new MessageFactory(_dateTimeProvider), _fakeRecipeRepository);
        _fakeCryptoService = new FakeCryptoService();

        _client = factory
            .WithWebHostBuilder(builder => builder.ConfigureServices(services =>
            {
                services.AddSingleton(_dateTimeProvider);

                services.AddSingleton<IRecipeQueryRepository>(_fakeRecipeRepository);
                services.AddSingleton<IRecipePersistenceRepository>(_fakeRecipeRepository);

                services.AddSingleton<IUserQueryRepository>(_fakeUserRepository);
                services.AddSingleton<IUserPersistenceRepository>(_fakeUserRepository);

                services.AddSingleton<IConnectionQueryRepository>(_fakeConnectionRepository);
                services.AddSingleton<IConnectionPersistenceRepository>(_fakeConnectionRepository);

                services.AddSingleton<IMessageQueryRepository>(_fakeMessageRepository);
                services.AddSingleton<IMessagePersistenceRepository>(_fakeMessageRepository);

                services.AddSingleton<ICryptoService>(_fakeCryptoService);

                services.AddSingleton(_cloudinaryWebClientMock.Object);
                services.AddSingleton(_cloudinarySignatureServiceMock.Object);
            }))
            .CreateClient();
    }
}
