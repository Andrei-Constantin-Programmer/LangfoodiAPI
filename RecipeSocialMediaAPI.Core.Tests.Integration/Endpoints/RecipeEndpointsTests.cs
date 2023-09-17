using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using RecipeSocialMediaAPI.Core.Contracts.Recipes;
using RecipeSocialMediaAPI.Core.Contracts.Users;
using RecipeSocialMediaAPI.Core.DTO.Recipes;
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
        _testRecipeContract = new()
        {
            Title = "Test",
            Description = "Test",
            ChefId = "0",
            Labels = new HashSet<string>(),
            NumberOfServings = 1,
            KiloCalories = 2300,
            CookingTime = 500,
            Ingredients = new List<IngredientDTO>() {
                new IngredientDTO()
                {
                    Name = "eggs",
                    Quantity = 1,
                    UnitOfMeasurement = "whole"
                }
            },
            RecipeSteps = new Stack<RecipeStepDTO>(),
        };

        _testRecipeContract.RecipeSteps.Push(new RecipeStepDTO()
        {
            Text = "step",
            ImageUrl = "url"
        });

        _testUserContract = new()
        {
            UserName = "TestUsername",
            Email = "test@mail.com",
            Password = "Test@123"
        };
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void RecipeGetById_WhenRecipeExists_ReturnRecipe()
    {
        // Given
        string recipeId = "0";

        await _client.PostAsJsonAsync("/user/create", _testUserContract);
        await _client.PostAsJsonAsync("/recipe/create", _testRecipeContract);

        // When
        var result = await _client.PostAsync($"/recipe/get/id?id={recipeId}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = result.Content.ReadFromJsonAsync<RecipeDetailedDTO>().Result;

        data.Should().NotBeNull();
        data.Id.Should().Be(recipeId);
        data.Title.Should().Be(_testRecipeContract.Title);
        data.Description.Should().Be(_testRecipeContract.Description);
        data.KiloCalories.Should().Be(2300);
        data.NumberOfServings.Should().Be(1);
        data.CookingTime.Should().Be(500);
        data.Chef.UserName.Should().Be(_testUserContract.UserName);
        data.Chef.Email.Should().Be(_testUserContract.Email);
        data.Chef.Password.Should().NotBeNull();
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
    public async void RecipesGetFromUserId_WhenAtLeastOneRecipeExists_ReturnAllFoundRecipes()
    {
        // Given
        string userid = "0";

        await _client.PostAsJsonAsync("/user/create", _testUserContract);
        await _client.PostAsJsonAsync("/recipe/create", _testRecipeContract);

        // When
        var result = await _client.PostAsync($"/recipe/get/userid?id={userid}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = result.Content.ReadFromJsonAsync<List<RecipeDTO>>().Result;

        data.Should().NotBeNull();
        data.Should().NotBeEmpty();
        data.First().ChefUsername.Should().Be(_testUserContract.UserName);
        data.First().Title.Should().Be(_testRecipeContract.Title);
        data.First().Description.Should().Be(_testRecipeContract.Description);
        data.First().NumberOfServings.Should().Be(_testRecipeContract.NumberOfServings);
        data.First().CookingTime.Should().Be(_testRecipeContract.CookingTime);
        data.First().KiloCalories.Should().Be(_testRecipeContract.KiloCalories);
        data.First().CreationDate.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void RecipesGetFromUserId_WhenNoRecipesExist_ReturnEmptyList()
    {
        // Given
        string userid = "0";

        // When
        var result = await _client.PostAsync($"/recipe/get/userid?id={userid}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = result.Content.ReadFromJsonAsync<List<RecipeDTO>>().Result;

        data.Should().NotBeNull();
        data.Should().BeEmpty();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void RecipesGetFromUserName_WhenAtLeastOneRecipeExists_ReturnAllFoundRecipes()
    {
        // Given
        string username = "TestUsername";

        await _client.PostAsJsonAsync("/user/create", _testUserContract);
        await _client.PostAsJsonAsync("/recipe/create", _testRecipeContract);

        // When
        var result = await _client.PostAsync($"/recipe/get/username?username={username}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = result.Content.ReadFromJsonAsync<List<RecipeDTO>>().Result;

        data.Should().NotBeNull();
        data.Should().NotBeEmpty();
        data.First().ChefUsername.Should().Be(_testUserContract.UserName);
        data.First().Title.Should().Be(_testRecipeContract.Title);
        data.First().Description.Should().Be(_testRecipeContract.Description);
        data.First().NumberOfServings.Should().Be(_testRecipeContract.NumberOfServings);
        data.First().CookingTime.Should().Be(_testRecipeContract.CookingTime);
        data.First().KiloCalories.Should().Be(_testRecipeContract.KiloCalories);
        data.First().CreationDate.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void RecipesGetFromUsername_WhenNoRecipesExist_ReturnEmptyList()
    {
        // Given
        string username = "TestUsername";

        // When
        var result = await _client.PostAsync($"/recipe/get/username?username={username}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = result.Content.ReadFromJsonAsync<List<RecipeDTO>>().Result;

        data.Should().NotBeNull();
        data.Should().BeEmpty();
    }
}
