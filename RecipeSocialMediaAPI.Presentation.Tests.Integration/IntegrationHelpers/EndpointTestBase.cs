﻿using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RecipeSocialMediaAPI.Application.Cryptography.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Application.Services.Interfaces;
using RecipeSocialMediaAPI.Application.Tests.Unit.TestHelpers;
using RecipeSocialMediaAPI.Application.Utilities;
using RecipeSocialMediaAPI.Application.WebClients.Interfaces;
using RecipeSocialMediaAPI.Domain.Services;
using RecipeSocialMediaAPI.Domain.Utilities;
using RecipeSocialMediaAPI.Presentation.Tests.Integration.IntegrationHelpers.FakeDependencies;

namespace RecipeSocialMediaAPI.Presentation.Tests.Integration.IntegrationHelpers;

public abstract class EndpointTestBase : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly Mock<ICloudinaryWebClient> _cloudinaryWebClientMock;
    protected readonly HttpClient _client;
    protected readonly IBearerTokenGeneratorService _bearerTokenGeneratorService;

    internal IDateTimeProvider _dateTimeProvider;
    internal FakeRecipeRepository _fakeRecipeRepository;
    internal FakeUserRepository _fakeUserRepository;
    internal FakeConnectionRepository _fakeConnectionRepository;
    internal FakeMessageRepository _fakeMessageRepository;
    internal FakeGroupRepository _fakeGroupRepository;
    internal FakeConversationRepository _fakeConversationRepository;

    internal FakePasswordCryptoService _fakePasswordCryptoService;

    public EndpointTestBase(WebApplicationFactory<Program> factory)
    {
        _cloudinaryWebClientMock = new Mock<ICloudinaryWebClient>();

        _dateTimeProvider = new DateTimeProvider();

        _fakeRecipeRepository = new FakeRecipeRepository();
        _fakeUserRepository = new FakeUserRepository();
        _fakeConnectionRepository = new FakeConnectionRepository();
        _fakeMessageRepository = new FakeMessageRepository(new MessageFactory(_dateTimeProvider), _fakeRecipeRepository);
        _fakeGroupRepository = new FakeGroupRepository();
        _fakeConversationRepository = new FakeConversationRepository();
        _fakePasswordCryptoService = new FakePasswordCryptoService();

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

                services.AddSingleton<IGroupQueryRepository>(_fakeGroupRepository);
                services.AddSingleton<IGroupPersistenceRepository>(_fakeGroupRepository);

                services.AddSingleton<IConversationQueryRepository>(_fakeConversationRepository);
                services.AddSingleton<IConversationPersistenceRepository>(_fakeConversationRepository);

                services.AddSingleton<IPasswordCryptoService>(_fakePasswordCryptoService);

                services.AddSingleton(_cloudinaryWebClientMock.Object);
            }))
            .CreateClient();

        _bearerTokenGeneratorService = factory.Services.GetService<IBearerTokenGeneratorService>()!;
    }
}
