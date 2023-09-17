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
    public RecipeEndpointsTests(WebApplicationFactory<Program> factory) : base(factory) { }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void RecipeGetById_WhenRecipeDoesNotExist_ReturnBadRequest()
    {
        // Given
        NewRecipeContract testContract = new NewRecipeContract()
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

        testContract.RecipeSteps.Push(new RecipeStepDTO()
        {
            Text = "step",
            ImageUrl = "url"
        });

        NewUserContract testUserContract = new()
        {
            UserName = "TestUsername",
            Email = "test@mail.com",
            Password = "Test@123"
        };

        await _client.PostAsJsonAsync("/user/create", testUserContract);
        await _client.PostAsJsonAsync("/recipe/create", testContract);
        
        // When
        var result = await _client.PostAsync("/recipe/get/id?id=0", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = result.Content.ReadFromJsonAsync<RecipeDetailedDTO>().Result;

        data.Should().NotBeNull();
        data!.Id.Should().Be("1");
        data!.Title.Should().Be(testContract.Title);
        data!.Description.Should().Be(testContract.Description);

    }

    /*    [Fact(Skip = "Recipes under construction")]
        [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
        [Trait(Traits.MODULE, Traits.Modules.CORE)]
        public async void RecipeCreate_WhenValidRecipe_ReturnsOk()
        {
            // Given
            var testRecipe = new RecipeDTO()
            {
                Id = "0",
                Title = "TestTitle",
                Description = "TestDescription",
                ChefUsername = "TestChef",
                Labels = new HashSet<string>()
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
                Id = "0",
                Title = "TestTitle",
                Description = "TestDescription",
                ChefUsername = "TestChef",
                Labels = new HashSet<string>()
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
                Id = "0",
                Title = "TestTitle",
                Description = "TestDescription",
                ChefUsername = "TestChef",
                Labels = new HashSet<string>()
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
        }*/
}
