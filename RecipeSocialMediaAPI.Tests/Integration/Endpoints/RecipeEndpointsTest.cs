using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using RecipeSocialMediaAPI.DTO;
using RecipeSocialMediaAPI.Tests.Integration.IntegrationHelpers;
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
            var data = result.Content.ReadFromJsonAsync<List<RecipeDTO>>().Result;

            // Assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            data.Should().NotBeNull();
            data.Should().BeEmpty();
        }
        
        [Fact]
        public async void RecipesCreate_ValidRecipe_ReturnsOk()
        {
            // Arrange
            JsonContent content = JsonContent.Create(new RecipeDTO()
            {
                Title = "TestTitle",
                Description = "TestDescription",
                Chef = "TestChef"
            }, typeof(RecipeDTO));

            // Act
            var result = await _client.PostAsync("/recipes/create", content);

            // Assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async void RecipesCreateAndGet_AfterValidRecipeCreated_GetReturnsTheNewRecipe()
        {
            // Arrange
            RecipeDTO testRecipe = new()
            {
                Title = "TestTitle",
                Description = "TestDescription",
                Chef = "TestChef"
            };

            JsonContent content = JsonContent.Create(testRecipe, typeof(RecipeDTO));

            // Act
            await _client.PostAsync("/recipes/create", content);

            var result = await _client.GetAsync("/recipes/get");
            var data = result.Content.ReadFromJsonAsync<List<RecipeDTO>>().Result;

            // Assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            data.Should().NotBeNull();
            data.Should().HaveCount(1);
            data![0].Title.Should().Be(testRecipe.Title);
            data![0].Description.Should().Be(testRecipe.Description);
            data![0].Chef.Should().Be(testRecipe.Chef);
            data![0].CreationDate.Should().NotBeNull();
        }
    }
}
