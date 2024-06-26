﻿using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Utilities;
using System.Collections.Immutable;

namespace RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;

public class ImageMessage : Message
{
    private readonly IDateTimeProvider _dateTimeProvider;

    private string? _textContent;
    public string? TextContent
    {
        get => _textContent;
        set
        {
            _textContent = value;
            UpdatedDate = _dateTimeProvider.Now;
        }
    }

    private readonly List<string> _imageURLs;
    public ImmutableList<string> ImageURLs => _imageURLs.ToImmutableList();

    public ImageMessage(IDateTimeProvider dateTimeProvider, 
        string id, IUserAccount sender, IEnumerable<string> imageURLs, string? textContent, DateTimeOffset sentDate, DateTimeOffset? updatedDate, Message? repliedToMessage = null, List<IUserAccount>? seenBy = null)
        : base(id, sender, sentDate, updatedDate, repliedToMessage, seenBy)
    {
        _dateTimeProvider = dateTimeProvider;

        if (!imageURLs.Any())
        {
            throw new ArgumentException("Cannot have an empty list of images for an Image Message");
        }

        _imageURLs = imageURLs.ToList();
        _textContent = textContent;
    }

    public void AddImage(string imageURL)
    {
        _imageURLs.Add(imageURL);
        UpdatedDate = _dateTimeProvider.Now;
    }
}
