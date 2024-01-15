using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using RecipeSocialMediaAPI.Application.Contracts.Recipes;
using RecipeSocialMediaAPI.Application.Contracts.Users;
using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Core.Tests.Integration.IntegrationHelpers;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Net;
using System.Net.Http.Json;

namespace RecipeSocialMediaAPI.Core.Tests.Integration.Endpoints;

public class RecipeEndpointsTests : EndpointTestBase
{
    private readonly NewUserContract _testUserContract;
    private readonly NewRecipeContract _testRecipeContract;

    public RecipeEndpointsTests(WebApplicationFactory<Program> factory) : base(factory)
    {
        _testRecipeContract = new(
            Title: "Test",
            Description: "Test",
            ChefId: "0",
            Tags: new HashSet<string>(),
            NumberOfServings: 1,
            KiloCalories: 2300,
            CookingTime: 500,
            Ingredients: new List<IngredientDTO>() {
                new()
                {
                    Name = "eggs",
                    Quantity = 1, 
                    UnitOfMeasurement = "whole"
                }
            },
            RecipeSteps: new Stack<RecipeStepDTO>(new[] 
            { 
                new RecipeStepDTO()
                {
                    Text = "step",
                    ImageUrl = "url"
                }
            })
        );

        _testUserContract = new(
            Handler: "testHandler",
            UserName: "TestUsername",
            Email: "test@mail.com",
            Password: "Test@123"
        );
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task RecipeGetById_WhenRecipeExists_ReturnRecipe()
    {
        // Given
        string recipeId = "0";

        await _client.PostAsJsonAsync("/user/create", _testUserContract);
        await _client.PostAsJsonAsync("/recipe/create", _testRecipeContract);

        // When
        var result = await _client.PostAsync($"/recipe/get/id?id={recipeId}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<RecipeDetailedDTO>();

        data.Should().NotBeNull();
        data!.Id.Should().Be(recipeId);
        data.Title.Should().Be(_testRecipeContract.Title);
        data.Description.Should().Be(_testRecipeContract.Description);
        data.KiloCalories.Should().Be(_testRecipeContract.KiloCalories);
        data.NumberOfServings.Should().Be(_testRecipeContract.NumberOfServings);
        data.CookingTime.Should().Be(_testRecipeContract.CookingTime);
        data.Chef.Handler.Should().Be(_testUserContract.Handler);
        data.Chef.UserName.Should().Be(_testUserContract.UserName);
        data.Ingredients.First().Name.Should().Be("eggs");
        data.Ingredients.First().Quantity.Should().Be(1);
        data.Ingredients.First().UnitOfMeasurement.Should().Be("whole");
        data.RecipeSteps.First().Text.Should().Be("step");
        data.RecipeSteps.First().ImageUrl.Should().Be("url");
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void RecipeGetById_WhenRecipeDoesNotExist_ReturnNotFound()
    {
        // Given
        string recipeId = "0";

        // When
        var result = await _client.PostAsync($"/recipe/get/id?id={recipeId}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task RecipesGetFromUserId_WhenAtLeastOneRecipeExists_ReturnAllRelatedRecipes()
    {
        // Given
        string userid = "0";

        NewRecipeContract secondTestRecipe = new(
            Title: "Test",
            Description: "Test",
            ChefId: "0",
            Tags: new HashSet<string>(),
            NumberOfServings: 1,
            KiloCalories: 2300,
            CookingTime: 500,
            Ingredients: new List<IngredientDTO>() {
                new()
                {
                    Name = "eggs",
                    Quantity = 1,
                    UnitOfMeasurement = "whole"
                }
            },
            RecipeSteps: new Stack<RecipeStepDTO>(new[]
            {
                new RecipeStepDTO()
                {
                    Text = "step",
                    ImageUrl = "url"
                }
            })
        );

        await _client.PostAsJsonAsync("/user/create", _testUserContract);
        await _client.PostAsJsonAsync("/recipe/create", _testRecipeContract);
        await _client.PostAsJsonAsync("/recipe/create", secondTestRecipe);

        // When
        var result = await _client.PostAsync($"/recipe/get/userid?id={userid}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<List<RecipeDTO>>();

        data.Should().NotBeNull();
        data.Should().NotBeEmpty();
        data!.Count.Should().Be(2);
        
        data.First().ChefUsername.Should().Be(_testUserContract.UserName);
        data.First().Title.Should().Be(_testRecipeContract.Title);
        data.First().Description.Should().Be(_testRecipeContract.Description);
        data.First().NumberOfServings.Should().Be(_testRecipeContract.NumberOfServings);
        data.First().CookingTime.Should().Be(_testRecipeContract.CookingTime);
        data.First().KiloCalories.Should().Be(_testRecipeContract.KiloCalories);
        data.First().CreationDate.Should().NotBeNull();

        data.Last().ChefUsername.Should().Be(_testUserContract.UserName);
        data.Last().Title.Should().Be(secondTestRecipe.Title);
        data.Last().Description.Should().Be(secondTestRecipe.Description);
        data.Last().NumberOfServings.Should().Be(secondTestRecipe.NumberOfServings);
        data.Last().CookingTime.Should().Be(secondTestRecipe.CookingTime);
        data.Last().KiloCalories.Should().Be(secondTestRecipe.KiloCalories);
        data.Last().CreationDate.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void RecipesGetFromUserId_WhenOneRecipeExists_ReturnFoundRecipe()
    {
        // Given
        string userid = "0";

        await _client.PostAsJsonAsync("/user/create", _testUserContract);
        await _client.PostAsJsonAsync("/recipe/create", _testRecipeContract);

        // When
        var result = await _client.PostAsync($"/recipe/get/userid?id={userid}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<List<RecipeDTO>>();

        data.Should().NotBeNull();
        data.Should().NotBeEmpty();
        data!.First().ChefUsername.Should().Be(_testUserContract.UserName);
        data!.First().Title.Should().Be(_testRecipeContract.Title);
        data!.First().Description.Should().Be(_testRecipeContract.Description);
        data!.First().NumberOfServings.Should().Be(_testRecipeContract.NumberOfServings);
        data!.First().CookingTime.Should().Be(_testRecipeContract.CookingTime);
        data!.First().KiloCalories.Should().Be(_testRecipeContract.KiloCalories);
        data!.First().CreationDate.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task RecipesGetFromUserId_WhenNoRecipesExist_ReturnEmptyList()
    {
        // Given
        string userid = "0";

        // When
        var result = await _client.PostAsync($"/recipe/get/userid?id={userid}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<List<RecipeDTO>>();

        data.Should().NotBeNull();
        data.Should().BeEmpty();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task RecipesGetFromUsername_WhenAtLeastOneRecipeExists_ReturnAllRelatedRecipes()
    {
        // Given
        string username = "TestUsername";

        NewRecipeContract secondTestRecipe = new(
            Title: "Test",
            Description: "Test",
            ChefId: "0",
            Tags: new HashSet<string>(),
            NumberOfServings: 1,
            KiloCalories: 2300,
            CookingTime: 500,
            Ingredients: new List<IngredientDTO>() {
                new()
                {
                    Name = "eggs",
                    Quantity = 1,
                    UnitOfMeasurement = "whole"
                }
            },
            RecipeSteps: new Stack<RecipeStepDTO>(new[]
            {
                new RecipeStepDTO()
                {
                    Text = "step",
                    ImageUrl = "url"
                }
            })
        );

        await _client.PostAsJsonAsync("/user/create", _testUserContract);
        await _client.PostAsJsonAsync("/recipe/create", _testRecipeContract);
        await _client.PostAsJsonAsync("/recipe/create", secondTestRecipe);

        // When
        var result = await _client.PostAsync($"/recipe/get/username?username={username}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<List<RecipeDTO>>();

        data.Should().NotBeNull();
        data.Should().NotBeEmpty();
        data!.Count.Should().Be(2);

        data.First().ChefUsername.Should().Be(_testUserContract.UserName);
        data.First().Title.Should().Be(_testRecipeContract.Title);
        data.First().Description.Should().Be(_testRecipeContract.Description);
        data.First().NumberOfServings.Should().Be(_testRecipeContract.NumberOfServings);
        data.First().CookingTime.Should().Be(_testRecipeContract.CookingTime);
        data.First().KiloCalories.Should().Be(_testRecipeContract.KiloCalories);
        data.First().CreationDate.Should().NotBeNull();

        data.Last().ChefUsername.Should().Be(_testUserContract.UserName);
        data.Last().Title.Should().Be(secondTestRecipe.Title);
        data.Last().Description.Should().Be(secondTestRecipe.Description);
        data.Last().NumberOfServings.Should().Be(secondTestRecipe.NumberOfServings);
        data.Last().CookingTime.Should().Be(secondTestRecipe.CookingTime);
        data.Last().KiloCalories.Should().Be(secondTestRecipe.KiloCalories);
        data.Last().CreationDate.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task RecipesGetFromUserName_WhenOneRecipeExists_ReturnFoundRecipe()
    {
        // Given
        string username = "TestUsername";

        await _client.PostAsJsonAsync("/user/create", _testUserContract);
        await _client.PostAsJsonAsync("/recipe/create", _testRecipeContract);

        // When
        var result = await _client.PostAsync($"/recipe/get/username?username={username}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<List<RecipeDTO>>();

        data.Should().NotBeNull();
        data.Should().NotBeEmpty();
        data!.First().ChefUsername.Should().Be(_testUserContract.UserName);
        data!.First().Title.Should().Be(_testRecipeContract.Title);
        data!.First().Description.Should().Be(_testRecipeContract.Description);
        data!.First().NumberOfServings.Should().Be(_testRecipeContract.NumberOfServings);
        data!.First().CookingTime.Should().Be(_testRecipeContract.CookingTime);
        data!.First().KiloCalories.Should().Be(_testRecipeContract.KiloCalories);
        data!.First().CreationDate.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task RecipesGetFromUsername_WhenNoRecipesExist_ReturnEmptyList()
    {
        // Given
        string username = "TestUsername";

        // When
        var result = await _client.PostAsync($"/recipe/get/username?username={username}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<List<RecipeDTO>>();

        data.Should().NotBeNull();
        data.Should().BeEmpty();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task RecipeCreate_WhenUserDoesNotExist_ReturnNotFound()
    {
        // When
        var result = await _client.PostAsJsonAsync("/recipe/create", _testRecipeContract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task RecipeCreate_WhenUserExists_ReturnCreatedRecipe()
    {
        // Given
        string recipeId = "0";
        await _client.PostAsJsonAsync("/user/create", _testUserContract);

        // When
        var result = await _client.PostAsJsonAsync("/recipe/create", _testRecipeContract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<RecipeDetailedDTO>();

        data.Should().NotBeNull();
        data!.Id.Should().Be(recipeId);
        data.Title.Should().Be(_testRecipeContract.Title);
        data.Description.Should().Be(_testRecipeContract.Description);
        data.KiloCalories.Should().Be(_testRecipeContract.KiloCalories);
        data.NumberOfServings.Should().Be(_testRecipeContract.NumberOfServings);
        data.CookingTime.Should().Be(_testRecipeContract.CookingTime);
        data.Chef.UserName.Should().Be(_testUserContract.UserName);
        data.Ingredients.First().Name.Should().Be("eggs");
        data.Ingredients.First().Quantity.Should().Be(1);
        data.Ingredients.First().UnitOfMeasurement.Should().Be("whole");
        data.RecipeSteps.First().Text.Should().Be("step");
        data.RecipeSteps.First().ImageUrl.Should().Be("url");
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task RecipeUpdate_WhenRecipeExists_UpdateTheRecipe()
    {
        // Given
        string recipeId = "0";
        UpdateRecipeContract newRecipe = new(
            Id: recipeId,
            Title: "New Title",
            Description: "New Desc",
            KiloCalories: 1000,
            Ingredients: new List<IngredientDTO>() {
                new()
                {
                    Name = "lemons",
                    Quantity = 2,
                    UnitOfMeasurement = "whole"
                }
            },
            RecipeSteps: _testRecipeContract.RecipeSteps,
            Tags: new HashSet<string>()
        );

        await _client.PostAsJsonAsync("/user/create", _testUserContract);
        await _client.PostAsJsonAsync("/recipe/create", _testRecipeContract);

        // When
        var updateResult = await _client.PutAsJsonAsync($"/recipe/update", newRecipe);

        // Then
        var getResult = await _client.PostAsync($"/recipe/get/id?id={recipeId}", null);
        updateResult.StatusCode.Should().Be(HttpStatusCode.OK);
        getResult.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await getResult.Content.ReadFromJsonAsync<RecipeDetailedDTO>();

        data.Should().NotBeNull();
        data!.Id.Should().Be(recipeId);
        data.Title.Should().Be(newRecipe.Title);
        data.Description.Should().Be(newRecipe.Description);
        data.KiloCalories.Should().Be(newRecipe.KiloCalories);
        data.NumberOfServings.Should().Be(_testRecipeContract.NumberOfServings);
        data.CookingTime.Should().Be(_testRecipeContract.CookingTime);
        data.Chef.UserName.Should().Be(_testUserContract.UserName);
        data.Ingredients.First().Name.Should().Be("lemons");
        data.Ingredients.First().Quantity.Should().Be(2);
        data.Ingredients.First().UnitOfMeasurement.Should().Be("whole");
        data.RecipeSteps.First().Text.Should().Be("step");
        data.RecipeSteps.First().ImageUrl.Should().Be("url");
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task RecipeUpdate_WhenRecipeDoesNotExist_ReturnNotFound()
    {
        // Given
        string recipeId = "0";
        UpdateRecipeContract newRecipe = new(
            Id: recipeId,
            Title: "New Title",
            Description: "New Desc",
            KiloCalories: 1000,
            Ingredients: new List<IngredientDTO>() {
                new()
                {
                    Name = "lemons",
                    Quantity = 2,
                    UnitOfMeasurement = "whole"
                }
            },
            RecipeSteps: _testRecipeContract.RecipeSteps,
            Tags: new HashSet<string>()
        );

        // When
        var updateResult = await _client.PutAsJsonAsync($"/recipe/update", newRecipe);

        // Then
        var getResult = await _client.PostAsync($"/recipe/get/id?id={recipeId}", null);
        updateResult.StatusCode.Should().Be(HttpStatusCode.NotFound);
        getResult.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void RecipeRemove_WhenRecipeDoesNotExist_ReturnNotFound()
    {
        // Given
        string recipeId = "0";
        await _client.PostAsJsonAsync("/user/create", _testUserContract);
        await _client.PostAsJsonAsync("/recipe/create", _testRecipeContract);

        // When
        var removeResult = await _client.DeleteAsync($"/recipe/remove?id={recipeId}");
        var getResult = await _client.PostAsync($"/recipe/get/id?id={recipeId}", null);

        // Then
        removeResult.StatusCode.Should().Be(HttpStatusCode.OK);
        getResult.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void RecipeRemove_WhenRecipeExists_RemoveRecipe()
    {
        // Given
        string recipeId = "0";

        // When
        var removeResult = await _client.DeleteAsync($"/recipe/remove?id={recipeId}");

        // Then
        removeResult.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
