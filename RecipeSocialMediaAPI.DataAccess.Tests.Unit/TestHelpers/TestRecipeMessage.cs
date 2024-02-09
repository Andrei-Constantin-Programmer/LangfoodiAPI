using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Unit.TestHelpers;

internal class TestRecipeMessage : TestMessage
{
    public string Text { get; set; }
    public List<RecipeAggregate> Recipes { get; set; }

    public TestRecipeMessage(string id, IUserAccount sender, string text, IEnumerable<RecipeAggregate> recipes, DateTimeOffset sentDate, DateTimeOffset? updatedDate, Message? repliedToMessage = null, List<IUserAccount>? seenBy = null)
        : base(id, sender, sentDate, updatedDate, repliedToMessage, seenBy)
    {
        Text = text;
        Recipes = recipes.ToList();
    }
}
