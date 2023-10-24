using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Unit.TestHelpers;

internal record TestFullMessage : TestMessage
{
    public string Text { get; set; }
    public List<RecipeAggregate> Recipes { get; set; }
    public List<string> ImageURLs { get; set; }

    public TestFullMessage(string id, IUserAccount sender, string text, IEnumerable<RecipeAggregate> recipes, IEnumerable<string> imageURLs, DateTimeOffset sentDate, DateTimeOffset? updatedDate, Message? repliedToMessage = null)
        : base(id, sender, sentDate, updatedDate, repliedToMessage)
    {
        Text = text;
        Recipes = recipes.ToList();
        ImageURLs = imageURLs.ToList();
    }
}
