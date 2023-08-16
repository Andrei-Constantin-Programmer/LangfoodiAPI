using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using RecipeSocialMediaAPI.Core.DTO.Recipes;
using RecipeSocialMediaAPI.Core.Tests.Integration.IntegrationHelpers;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Net;
using System.Net.Http.Json;

namespace RecipeSocialMediaAPI.Core.Tests.Integration.Endpoints;

public class RecipeEndpointsTests : EndpointTestBase
{
    public RecipeEndpointsTests(WebApplicationFactory<Program> factory) : base(factory) { }

    [Fact (Skip = "Recipes under construction")]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void RecipeGet_WhenNoRecipesCreated_ReturnsEmptyList()
    {
        // Given
        
        // When
        var result = await _client.GetAsync("/recipe/get");

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var data = result.Content.ReadFromJsonAsync<List<RecipeDTO>>().Result;
        data.Should().NotBeNull();
        data.Should().BeEmpty();
    }

    [Fact(Skip = "Recipes under construction")]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void RecipeCreate_WhenValidRecipe_ReturnsOk()
    {
        // Given
        var testRecipe = new RecipeDTO()
        {
            Id = 0,
            Title = "TestTitle",
            Description = "TestDescription",
            ChefUsername = "TestChef"
        };

        // When
        var result = await _client.PostAsJsonAsync("/recipe/create", testRecipe);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact(Skip = "Recipes under construction")]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void RecipeCreateAndGet_AfterValidRecipeCreated_GetReturnsTheNewRecipe()
    {
        // Given
        RecipeDTO testRecipe = new()
        {
            Id = 0,
            Title = "TestTitle",
            Description = "TestDescription",
            ChefUsername = "TestChef"
        };

        // When
        await _client.PostAsJsonAsync("/recipe/create", testRecipe);

        var result = await _client.GetAsync("/recipe/get");

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var data = result.Content.ReadFromJsonAsync<List<RecipeDTO>>().Result;

        data.Should().NotBeNull();
        data.Should().HaveCount(1);
        data![0].Title.Should().Be(testRecipe.Title);
        data![0].Description.Should().Be(testRecipe.Description);
        data![0].ChefUsername.Should().Be(testRecipe.ChefUsername);
        data![0].CreationDate.Should().NotBeNull();
    }

    [Fact(Skip = "Recipes under construction")]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void RecipeGetById_WhenRecipeDoesNotExist_ReturnsNotFound()
    {
        // Given
        int testId = 1;

        // When
        var result = await _client.PostAsync($"/recipe/getById/{testId}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(Skip = "Recipes under construction")]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void RecipeGetById_WhenRecipeDoesExist_ReturnsRecipe()
    {
        // Given
        RecipeDTO testRecipe = new()
        {
            Id = 1,
            Title = "TestTitle",
            Description = "TestDescription",
            ChefUsername = "TestChef"
        };

        await _client.PostAsJsonAsync("/recipe/create", testRecipe);

        // When
        var result = await _client.PostAsync($"/recipe/getById/{testRecipe.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = result.Content.ReadFromJsonAsync<RecipeDTO>().Result;

        data.Should().NotBeNull();
        data!.Id.Should().Be(testRecipe.Id);
        data!.Title.Should().Be(testRecipe.Title);
        data!.Description.Should().Be(testRecipe.Description);
        data!.ChefUsername.Should().Be(testRecipe.ChefUsername);
        data!.CreationDate.Should().NotBeNull();
    }
}
