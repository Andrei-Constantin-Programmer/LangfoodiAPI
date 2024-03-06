using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using RecipeSocialMediaAPI.Application.Contracts.Recipes;
using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Presentation.Tests.Integration.IntegrationHelpers;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace RecipeSocialMediaAPI.Presentation.Tests.Integration.Endpoints;

public class RecipeEndpointsTests : EndpointTestBase
{
    private readonly TestUserCredentials _testUser;
    private readonly RecipeAggregate _testRecipe;
    private readonly RecipeAggregate _secondTestRecipe;

    public RecipeEndpointsTests(WebApplicationFactory<Program> factory) : base(factory)
    {
        _testUser = new()
        {
            Account = new TestUserAccount()
            {
                Id = "0",
                Handler = "testHandler",
                UserName = "TestUsername",
            },
            Email = "test@mail.com",
            Password = "Test@123"
        };

        _testRecipe = new(
            id: "0",
            title: "Test",
            description: "Test",
            chef: _testUser.Account,
            tags: new HashSet<string>(),
            recipe: new(
            numberOfServings: 1,
            kiloCalories: 2300,
            cookingTimeInSeconds: 500,
            ingredients: new List<Ingredient>() {
                new("eggs", 1, "whole")
            },
            steps: new Stack<RecipeStep>(new[]
            {
                new RecipeStep("step", new RecipeImage("url"))
            })),
            creationDate: new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
            lastUpdatedDate: new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)
        );

        _secondTestRecipe = new(
            id: "0",
            title: "Test2",
            description: "Test 2",
            chef: _testUser.Account,
            tags: new HashSet<string>(),
            recipe: new(
            numberOfServings: 1,
            kiloCalories: 1900,
            cookingTimeInSeconds: 200,
            ingredients: new List<Ingredient>() {
                new("eggs", 2, "whole")
            },
            steps: new Stack<RecipeStep>(new[]
            {
                new RecipeStep("step1", new RecipeImage("url")),
                new RecipeStep("step2", new RecipeImage("url2"))
            })),
            creationDate: new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
            lastUpdatedDate: new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)
        );
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task RecipeGetById_WhenRecipeExists_ReturnRecipe()
    {
        // Given
        string recipeId = "0";

        var user = await _fakeUserRepository
            .CreateUserAsync(_testUser.Account.Handler, _testUser.Account.UserName, _testUser.Email, _fakeCryptoService.Encrypt(_testUser.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        _ = _fakeRecipeRepository
            .CreateRecipeAsync(_testRecipe.Title, _testRecipe.Recipe, _testRecipe.Description, _testRecipe.Chef, _testRecipe.Tags, _testRecipe.CreationDate, _testRecipe.LastUpdatedDate, _testRecipe.ThumbnailId);

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"/recipe/get/id?id={recipeId}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<RecipeDetailedDTO>();

        data.Should().NotBeNull();
        data!.Id.Should().Be(recipeId);
        data.Title.Should().Be(_testRecipe.Title);
        data.Description.Should().Be(_testRecipe.Description);
        data.KiloCalories.Should().Be(_testRecipe.Recipe.KiloCalories);
        data.NumberOfServings.Should().Be(_testRecipe.Recipe.NumberOfServings);
        data.CookingTime.Should().Be(_testRecipe.Recipe.CookingTimeInSeconds);
        data.Chef.Handler.Should().Be(_testUser.Account.Handler);
        data.Chef.UserName.Should().Be(_testUser.Account.UserName);
        data.Ingredients.First().Name.Should().Be("eggs");
        data.Ingredients.First().Quantity.Should().Be(1);
        data.Ingredients.First().UnitOfMeasurement.Should().Be("whole");
        data.RecipeSteps.First().Text.Should().Be("step");
        data.RecipeSteps.First().ImageUrl.Should().Be("url");
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task RecipeGetById_WhenRecipeDoesNotExist_ReturnNotFound()
    {
        // Given
        string recipeId = "0";

        var user = await _fakeUserRepository
            .CreateUserAsync(_testUser.Account.Handler, _testUser.Account.UserName, _testUser.Email, _fakeCryptoService.Encrypt(_testUser.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"/recipe/get/id?id={recipeId}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task RecipeGetById_WhenNoTokenIsUsed_ReturnUnauthorised()
    {
        // Given
        string recipeId = "0";

        _ = _fakeUserRepository
            .CreateUserAsync(_testUser.Account.Handler, _testUser.Account.UserName, _testUser.Email, _fakeCryptoService.Encrypt(_testUser.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        _ = _fakeRecipeRepository
            .CreateRecipeAsync(_testRecipe.Title, _testRecipe.Recipe, _testRecipe.Description, _testRecipe.Chef, _testRecipe.Tags, _testRecipe.CreationDate, _testRecipe.LastUpdatedDate, _testRecipe.ThumbnailId);

        // When
        var result = await _client.PostAsync($"/recipe/get/id?id={recipeId}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task RecipesGetFromUserId_WhenAtLeastOneRecipeExists_ReturnAllRelatedRecipes()
    {
        // Given
        string userid = "0";

        var user = await _fakeUserRepository
            .CreateUserAsync(_testUser.Account.Handler, _testUser.Account.UserName, _testUser.Email, _fakeCryptoService.Encrypt(_testUser.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        _ = _fakeRecipeRepository
            .CreateRecipeAsync(_testRecipe.Title, _testRecipe.Recipe, _testRecipe.Description, _testRecipe.Chef, _testRecipe.Tags, _testRecipe.CreationDate, _testRecipe.LastUpdatedDate, _testRecipe.ThumbnailId);
        _ = _fakeRecipeRepository
            .CreateRecipeAsync(_secondTestRecipe.Title, _secondTestRecipe.Recipe, _secondTestRecipe.Description, _secondTestRecipe.Chef, _secondTestRecipe.Tags, _secondTestRecipe.CreationDate, _secondTestRecipe.LastUpdatedDate, _secondTestRecipe.ThumbnailId);

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"/recipe/get/userid?id={userid}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<List<RecipeDTO>>();

        data.Should().NotBeNull();
        data.Should().NotBeEmpty();
        data!.Count.Should().Be(2);
        
        data.First().ChefUsername.Should().Be(_testUser.Account.UserName);
        data.First().Title.Should().Be(_testRecipe.Title);
        data.First().Description.Should().Be(_testRecipe.Description);
        data.First().NumberOfServings.Should().Be(_testRecipe.Recipe.NumberOfServings);
        data.First().CookingTime.Should().Be(_testRecipe.Recipe.CookingTimeInSeconds);
        data.First().KiloCalories.Should().Be(_testRecipe.Recipe.KiloCalories);
        data.First().CreationDate.Should().NotBeNull();

        data.Last().ChefUsername.Should().Be(_testUser.Account.UserName);
        data.Last().Title.Should().Be(_secondTestRecipe.Title);
        data.Last().Description.Should().Be(_secondTestRecipe.Description);
        data.Last().NumberOfServings.Should().Be(_secondTestRecipe.Recipe.NumberOfServings);
        data.Last().CookingTime.Should().Be(_secondTestRecipe.Recipe.CookingTimeInSeconds);
        data.Last().KiloCalories.Should().Be(_secondTestRecipe.Recipe.KiloCalories);
        data.Last().CreationDate.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task RecipesGetFromUserId_WhenOneRecipeExists_ReturnFoundRecipe()
    {
        // Given
        var user = await _fakeUserRepository
            .CreateUserAsync(_testUser.Account.Handler, _testUser.Account.UserName, _testUser.Email, _fakeCryptoService.Encrypt(_testUser.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        _ = await _fakeRecipeRepository
            .CreateRecipeAsync(_testRecipe.Title, _testRecipe.Recipe, _testRecipe.Description, _testRecipe.Chef, _testRecipe.Tags, _testRecipe.CreationDate, _testRecipe.LastUpdatedDate, _testRecipe.ThumbnailId);

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"/recipe/get/userid?id={user.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<List<RecipeDTO>>();

        data.Should().NotBeNull();
        data.Should().NotBeEmpty();
        data!.First().ChefUsername.Should().Be(_testUser.Account.UserName);
        data!.First().Title.Should().Be(_testRecipe.Title);
        data!.First().Description.Should().Be(_testRecipe.Description);
        data!.First().NumberOfServings.Should().Be(_testRecipe.Recipe.NumberOfServings);
        data!.First().CookingTime.Should().Be(_testRecipe.Recipe.CookingTimeInSeconds);
        data!.First().KiloCalories.Should().Be(_testRecipe.Recipe.KiloCalories);
        data!.First().CreationDate.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task RecipesGetFromUserId_WhenNoRecipesExist_ReturnEmptyList()
    {
        // Given
        var user = await _fakeUserRepository
            .CreateUserAsync(_testUser.Account.Handler, _testUser.Account.UserName, _testUser.Email, _fakeCryptoService.Encrypt(_testUser.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"/recipe/get/userid?id={user.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<List<RecipeDTO>>();

        data.Should().NotBeNull();
        data.Should().BeEmpty();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task RecipesGetFromUserId_WhenNoTokenIsUsed_ReturnUnauthorised()
    {
        // Given
        var user = await _fakeUserRepository
            .CreateUserAsync(_testUser.Account.Handler, _testUser.Account.UserName, _testUser.Email, _fakeCryptoService.Encrypt(_testUser.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        _ = await _fakeRecipeRepository
            .CreateRecipeAsync(_testRecipe.Title, _testRecipe.Recipe, _testRecipe.Description, _testRecipe.Chef, _testRecipe.Tags, _testRecipe.CreationDate, _testRecipe.LastUpdatedDate, _testRecipe.ThumbnailId);

        // When
        var result = await _client.PostAsync($"/recipe/get/userid?id={user.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task RecipesGetFromUsername_WhenAtLeastOneRecipeExists_ReturnAllRelatedRecipes()
    {
        // Given
        var user = await _fakeUserRepository
            .CreateUserAsync(_testUser.Account.Handler, _testUser.Account.UserName, _testUser.Email, _fakeCryptoService.Encrypt(_testUser.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        await _fakeRecipeRepository
            .CreateRecipeAsync(_testRecipe.Title, _testRecipe.Recipe, _testRecipe.Description, _testRecipe.Chef, _testRecipe.Tags, _testRecipe.CreationDate, _testRecipe.LastUpdatedDate, _testRecipe.ThumbnailId);
        await _fakeRecipeRepository
            .CreateRecipeAsync(_secondTestRecipe.Title, _secondTestRecipe.Recipe, _secondTestRecipe.Description, _secondTestRecipe.Chef, _secondTestRecipe.Tags, _secondTestRecipe.CreationDate, _secondTestRecipe.LastUpdatedDate, _secondTestRecipe.ThumbnailId);

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"/recipe/get/username?username={user.Account.UserName}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<List<RecipeDTO>>();

        data.Should().NotBeNull();
        data.Should().NotBeEmpty();
        data!.Count.Should().Be(2);

        data.First().ChefUsername.Should().Be(_testUser.Account.UserName);
        data.First().Title.Should().Be(_testRecipe.Title);
        data.First().Description.Should().Be(_testRecipe.Description);
        data.First().NumberOfServings.Should().Be(_testRecipe.Recipe.NumberOfServings);
        data.First().CookingTime.Should().Be(_testRecipe.Recipe.CookingTimeInSeconds);
        data.First().KiloCalories.Should().Be(_testRecipe.Recipe.KiloCalories);
        data.First().CreationDate.Should().NotBeNull();

        data.Last().ChefUsername.Should().Be(_testUser.Account.UserName);
        data.Last().Title.Should().Be(_secondTestRecipe.Title);
        data.Last().Description.Should().Be(_secondTestRecipe.Description);
        data.Last().NumberOfServings.Should().Be(_secondTestRecipe.Recipe.NumberOfServings);
        data.Last().CookingTime.Should().Be(_secondTestRecipe.Recipe.CookingTimeInSeconds);
        data.Last().KiloCalories.Should().Be(_secondTestRecipe.Recipe.KiloCalories);
        data.Last().CreationDate.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task RecipesGetFromUserName_WhenOneRecipeExists_ReturnFoundRecipe()
    {
        // Given
        var user = await _fakeUserRepository
            .CreateUserAsync(_testUser.Account.Handler, _testUser.Account.UserName, _testUser.Email, _fakeCryptoService.Encrypt(_testUser.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        await _fakeRecipeRepository
            .CreateRecipeAsync(_testRecipe.Title, _testRecipe.Recipe, _testRecipe.Description, _testRecipe.Chef, _testRecipe.Tags, _testRecipe.CreationDate, _testRecipe.LastUpdatedDate, _testRecipe.ThumbnailId);

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"/recipe/get/username?username={user.Account.UserName}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<List<RecipeDTO>>();

        data.Should().NotBeNull();
        data.Should().NotBeEmpty();
        data!.First().ChefUsername.Should().Be(_testUser.Account.UserName);
        data!.First().Title.Should().Be(_testRecipe.Title);
        data!.First().Description.Should().Be(_testRecipe.Description);
        data!.First().NumberOfServings.Should().Be(_testRecipe.Recipe.NumberOfServings);
        data!.First().CookingTime.Should().Be(_testRecipe.Recipe.CookingTimeInSeconds);
        data!.First().KiloCalories.Should().Be(_testRecipe.Recipe.KiloCalories);
        data!.First().CreationDate.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task RecipesGetFromUsername_WhenNoRecipesExist_ReturnEmptyList()
    {
        // Given
        var user = await _fakeUserRepository
            .CreateUserAsync(_testUser.Account.Handler, _testUser.Account.UserName, _testUser.Email, _fakeCryptoService.Encrypt(_testUser.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"/recipe/get/username?username={user.Account.UserName}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<List<RecipeDTO>>();

        data.Should().NotBeNull();
        data.Should().BeEmpty();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task RecipesGetFromUserName_WhenNoTokenIsUsed_ReturnUnauthorised()
    {
        // Given
        var user = await _fakeUserRepository
            .CreateUserAsync(_testUser.Account.Handler, _testUser.Account.UserName, _testUser.Email, _fakeCryptoService.Encrypt(_testUser.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        await _fakeRecipeRepository
            .CreateRecipeAsync(_testRecipe.Title, _testRecipe.Recipe, _testRecipe.Description, _testRecipe.Chef, _testRecipe.Tags, _testRecipe.CreationDate, _testRecipe.LastUpdatedDate, _testRecipe.ThumbnailId);

        // When
        var result = await _client.PostAsync($"/recipe/get/username?username={user.Account.UserName}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task RecipeCreate_WhenUserDoesNotExist_ReturnNotFound()
    {
        // Given
        TestUserCredentials user = new()
        {
            Account = new TestUserAccount
            {
                Id = "u1",
                Handler = "user_1",
                UserName = "User 1"
            },
            Email = "u1@mail.com",
            Password = "Pass@123"
        };

        NewRecipeContract newRecipe = new(
            Title: "New Title",
            Description: "New Desc",
            KiloCalories: 1000,
            ChefId: user.Account.Id,
            Ingredients: new List<IngredientDTO>() { new("lemons", 2, "whole") },
            RecipeSteps: new Stack<RecipeStepDTO>(new RecipeStepDTO[]
            {
                new("step1", "url1"),
                new("step2", "url2")
            }),
            Tags: new HashSet<string>()
        );

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsJsonAsync("/recipe/create", newRecipe);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task RecipeCreate_WhenUserExists_ReturnCreatedRecipe()
    {
        // Given
        string recipeId = "0";
        var user = await _fakeUserRepository
            .CreateUserAsync(_testUser.Account.Handler, _testUser.Account.UserName, _testUser.Email, _fakeCryptoService.Encrypt(_testUser.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        NewRecipeContract newRecipe = new(
            Title: "New Title",
            Description: "New Desc",
            KiloCalories: 1000,
            ChefId: _testUser.Account.Id,
            Ingredients: new List<IngredientDTO>() { new("lemons", 2, "whole") },
            RecipeSteps: new Stack<RecipeStepDTO>(new RecipeStepDTO[]
            {
                new("step1", "url1"),
                new("step2", "url2")
            }),
            Tags: new HashSet<string>()
        );

        // When
        var result = await _client.PostAsJsonAsync("/recipe/create", newRecipe);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<RecipeDetailedDTO>();

        data.Should().NotBeNull();
        data!.Id.Should().Be(recipeId);
        data.Title.Should().Be(newRecipe.Title);
        data.Description.Should().Be(newRecipe.Description);
        data.KiloCalories.Should().Be(newRecipe.KiloCalories);
        data.NumberOfServings.Should().Be(newRecipe.NumberOfServings);
        data.CookingTime.Should().Be(newRecipe.CookingTime);
        data.Chef.UserName.Should().Be(_testUser.Account.UserName);
        data.Ingredients.First().Name.Should().Be("lemons");
        data.Ingredients.First().Quantity.Should().Be(2);
        data.Ingredients.First().UnitOfMeasurement.Should().Be("whole");
        data.RecipeSteps.First().Text.Should().Be("step1");
        data.RecipeSteps.First().ImageUrl.Should().Be("url1");
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task RecipeCreate_WhenNoTokenIsUsed_ReturnUnauthorised()
    {
        // Given
        _ = _fakeUserRepository
            .CreateUserAsync(_testUser.Account.Handler, _testUser.Account.UserName, _testUser.Email, _fakeCryptoService.Encrypt(_testUser.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        NewRecipeContract newRecipe = new(
            Title: "New Title",
            Description: "New Desc",
            KiloCalories: 1000,
            ChefId: _testUser.Account.Id,
            Ingredients: new List<IngredientDTO>() { new("lemons", 2, "whole") },
            RecipeSteps: new Stack<RecipeStepDTO>(new RecipeStepDTO[]
            {
                new("step1", "url1"),
                new("step2", "url2")
            }),
            Tags: new HashSet<string>()
        );

        // When
        var result = await _client.PostAsJsonAsync("/recipe/create", newRecipe);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task RecipeUpdate_WhenRecipeExists_UpdateTheRecipe()
    {
        // Given
        string recipeId = "0";
        UpdateRecipeContract newRecipe = new(
            Id: recipeId,
            Title: "New Title",
            Description: "New Desc",
            KiloCalories: 1000,
            Ingredients: new List<IngredientDTO>() { new("lemons", 2, "whole") },
            RecipeSteps: new Stack<RecipeStepDTO>(new RecipeStepDTO[]
            {
                new("step1", "url1"),
                new("step2", "url2")
            }),
            Tags: new HashSet<string>()
        );

        var user = await _fakeUserRepository
            .CreateUserAsync(_testUser.Account.Handler, _testUser.Account.UserName, _testUser.Email, _fakeCryptoService.Encrypt(_testUser.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        await _fakeRecipeRepository
            .CreateRecipeAsync(_testRecipe.Title, _testRecipe.Recipe, _testRecipe.Description, _testRecipe.Chef, _testRecipe.Tags, _testRecipe.CreationDate, _testRecipe.LastUpdatedDate, _testRecipe.ThumbnailId);

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var updateResult = await _client.PutAsJsonAsync($"/recipe/update", newRecipe);

        // Then
        updateResult.StatusCode.Should().Be(HttpStatusCode.OK);

        var recipe = await _fakeRecipeRepository.GetRecipeByIdAsync(recipeId);
        recipe.Should().NotBeNull();
        recipe!.Id.Should().Be(recipeId);
        recipe.Title.Should().Be(newRecipe.Title);
        recipe.Description.Should().Be(newRecipe.Description);
        recipe.Recipe.KiloCalories.Should().Be(newRecipe.KiloCalories);
        recipe.Recipe.NumberOfServings.Should().Be(_testRecipe.Recipe.NumberOfServings);
        recipe.Recipe.CookingTimeInSeconds.Should().Be(_testRecipe.Recipe.CookingTimeInSeconds);
        recipe.Chef.UserName.Should().Be(_testUser.Account.UserName);
        recipe.Recipe.Ingredients.First().Name.Should().Be("lemons");
        recipe.Recipe.Ingredients.First().Quantity.Should().Be(2);
        recipe.Recipe.Ingredients.First().UnitOfMeasurement.Should().Be("whole");
        recipe.Recipe.Steps.First().Text.Should().Be("step1");
        recipe.Recipe.Steps.First().Image!.ImageUrl.Should().Be("url1");
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task RecipeUpdate_WhenRecipeDoesNotExist_ReturnNotFound()
    {
        // Given
        string recipeId = "0";
        UpdateRecipeContract newRecipe = new(
            Id: recipeId,
            Title: "New Title",
            Description: "New Desc",
            KiloCalories: 1000,
            Ingredients: new List<IngredientDTO>() { new("lemons", 2, "whole") },
            RecipeSteps: new Stack<RecipeStepDTO>(new RecipeStepDTO[]
            {
                new("step1", "url1"),
                new("step2", "url2")
            }),
            Tags: new HashSet<string>()
        );

        var user = await _fakeUserRepository
            .CreateUserAsync(_testUser.Account.Handler, _testUser.Account.UserName, _testUser.Email, _fakeCryptoService.Encrypt(_testUser.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var updateResult = await _client.PutAsJsonAsync($"/recipe/update", newRecipe);

        // Then
        updateResult.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var recipe = await _fakeRecipeRepository.GetRecipeByIdAsync(recipeId);
        recipe.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task RecipeUpdate_WhenNoTokenIsUsed_ReturnUnauthorised()
    {
        // Given
        string recipeId = "0";
        UpdateRecipeContract newRecipe = new(
            Id: recipeId,
            Title: "New Title",
            Description: "New Desc",
            KiloCalories: 1000,
            Ingredients: new List<IngredientDTO>() { new("lemons", 2, "whole") },
            RecipeSteps: new Stack<RecipeStepDTO>(new RecipeStepDTO[]
            {
                new("step1", "url1"),
                new("step2", "url2")
            }),
            Tags: new HashSet<string>()
        );

        _ = await _fakeUserRepository
            .CreateUserAsync(_testUser.Account.Handler, _testUser.Account.UserName, _testUser.Email, _fakeCryptoService.Encrypt(_testUser.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        await _fakeRecipeRepository
            .CreateRecipeAsync(_testRecipe.Title, _testRecipe.Recipe, _testRecipe.Description, _testRecipe.Chef, _testRecipe.Tags, _testRecipe.CreationDate, _testRecipe.LastUpdatedDate, _testRecipe.ThumbnailId);

        // When
        var updateResult = await _client.PutAsJsonAsync($"/recipe/update", newRecipe);

        // Then
        updateResult.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task RecipeRemove_WhenRecipeDoesNotExist_ReturnNotFound()
    {
        // Given
        string recipeId = "0";
        var user = await _fakeUserRepository
            .CreateUserAsync(_testUser.Account.Handler, _testUser.Account.UserName, _testUser.Email, _fakeCryptoService.Encrypt(_testUser.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        await _fakeRecipeRepository
            .CreateRecipeAsync(_testRecipe.Title, _testRecipe.Recipe, _testRecipe.Description, _testRecipe.Chef, _testRecipe.Tags, _testRecipe.CreationDate, _testRecipe.LastUpdatedDate, _testRecipe.ThumbnailId);

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var removeResult = await _client.DeleteAsync($"/recipe/remove?id={recipeId}");

        // Then
        removeResult.StatusCode.Should().Be(HttpStatusCode.OK);

        var recipe = await _fakeRecipeRepository.GetRecipeByIdAsync(recipeId);
        recipe.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task RecipeRemove_WhenRecipeExists_RemoveRecipe()
    {
        // Given
        string recipeId = "0";

        var user = await _fakeUserRepository
            .CreateUserAsync(_testUser.Account.Handler, _testUser.Account.UserName, _testUser.Email, _fakeCryptoService.Encrypt(_testUser.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var removeResult = await _client.DeleteAsync($"/recipe/remove?id={recipeId}");

        // Then
        removeResult.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task RecipeRemove_WhenNoTokenIsUsed_ReturnUnauthorised()
    {
        // Given
        string recipeId = "0";

        _ = await _fakeUserRepository
            .CreateUserAsync(_testUser.Account.Handler, _testUser.Account.UserName, _testUser.Email, _fakeCryptoService.Encrypt(_testUser.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        // When
        var removeResult = await _client.DeleteAsync($"/recipe/remove?id={recipeId}");

        // Then
        removeResult.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
