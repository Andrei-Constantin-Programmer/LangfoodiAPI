using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Unit.TestHelpers;

internal class TestImageMessage : TestMessage
{
    public string Text { get; set; }
    public List<string> ImageURLs { get; set; }

    public TestImageMessage(string id, IUserAccount sender, string text, IEnumerable<string> imageURLs, DateTimeOffset sentDate, DateTimeOffset? updatedDate, Message? repliedToMessage = null, List<IUserAccount>? seenBy = null)
        : base(id, sender, sentDate, updatedDate, repliedToMessage, seenBy)
    {
        Text = text;
        ImageURLs = imageURLs.ToList();
    }
}
