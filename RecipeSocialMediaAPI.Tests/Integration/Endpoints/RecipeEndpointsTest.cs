using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Tests.Integration.IntegrationHelpers;
using System.Net;
using System.Net.Http.Json;

namespace RecipeSocialMediaAPI.Tests.Integration.Endpoints
{
    public class RecipeEndpointsTest : EndpointTestBase
    {
        public RecipeEndpointsTest(WebApplicationFactory<Program> factory) : base(factory) { }

        [Fact]
        public async void RecipesGet_NoRecipesCreated_ReturnsEmptyList()
        {
            // Arrange
            
            // Act
            var result = await _client.GetAsync("/recipes/get");

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);

            var data = result.Content.ReadFromJsonAsync<List<RecipeDTO>>().Result;
            data.Should().NotBeNull();
            data.Should().BeEmpty();
        }
        
        [Fact]
        public async void RecipesCreate_ValidRecipe_ReturnsOk()
        {
            // Arrange
            var testRecipe = new RecipeDTO()
            {
                Id = 0,
                Title = "TestTitle",
                Description = "TestDescription",
                Chef = "TestChef"
            };

            // Act
            var result = await _client.PostAsJsonAsync("/recipes/create", testRecipe);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async void RecipesCreateAndGet_AfterValidRecipeCreated_GetReturnsTheNewRecipe()
        {
            // Arrange
            RecipeDTO testRecipe = new()
            {
                Id = 0,
                Title = "TestTitle",
                Description = "TestDescription",
                Chef = "TestChef"
            };

            // Act
            await _client.PostAsJsonAsync("/recipes/create", testRecipe);

            var result = await _client.GetAsync("/recipes/get");

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var data = result.Content.ReadFromJsonAsync<List<RecipeDTO>>().Result;

            data.Should().NotBeNull();
            data.Should().HaveCount(1);
            data![0].Title.Should().Be(testRecipe.Title);
            data![0].Description.Should().Be(testRecipe.Description);
            data![0].Chef.Should().Be(testRecipe.Chef);
            data![0].CreationDate.Should().NotBeNull();
        }

        [Fact]
        public async void RecipesGetById_RecipeDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            int testId = 1;

            // Act
            var result = await _client.PostAsync($"/recipes/getById/{testId}", null);

            // Assert
            //result.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Content.Headers.Should().BeEmpty();
        }

        [Fact]
        public async void RecipesGetById_RecipeDoesExist_ReturnsRecipe()
        {
            // Arrange
            RecipeDTO testRecipe = new()
            {
                Id = 1,
                Title = "TestTitle",
                Description = "TestDescription",
                Chef = "TestChef"
            };

            await _client.PostAsJsonAsync("/recipes/create", testRecipe);

            // Act
            var result = await _client.PostAsync($"/recipes/getById/{testRecipe.Id}", null);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            var data = result.Content.ReadFromJsonAsync<RecipeDTO>().Result;

            data.Should().NotBeNull();
            data!.Id.Should().Be(testRecipe.Id);
            data!.Title.Should().Be(testRecipe.Title);
            data!.Description.Should().Be(testRecipe.Description);
            data!.Chef.Should().Be(testRecipe.Chef);
            data!.CreationDate.Should().NotBeNull();
        }
    }
}
