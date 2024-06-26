﻿using RecipeSocialMediaAPI.Domain.Models.Users;
using System.Collections.Immutable;

namespace RecipeSocialMediaAPI.Domain.Tests.Shared;

public class TestUserAccount : IUserAccount
{
    required public string Id { get; set; }
    required public string Handler { get; set; }
    required public string UserName { get; set; }
    public string? ProfileImageId { get; set; }
    public DateTimeOffset AccountCreationDate { get; set; }

    private HashSet<string> _pinnedConversationIds = new();
    public ImmutableList<string> PinnedConversationIds 
    { 
        get => _pinnedConversationIds.ToImmutableList(); 
        set => _pinnedConversationIds = value.ToHashSet(); 
    }

    private HashSet<string> _blockedConnectionIds = new();
    public ImmutableList<string> BlockedConnectionIds
    {
        get => _blockedConnectionIds.ToImmutableList();
        set => _blockedConnectionIds = value.ToHashSet();
    }

    public UserRole Role { get; set; }

    public bool AddPin(string pinnedConversationId) => _pinnedConversationIds.Add(pinnedConversationId);
    public bool RemovePin(string pinnedConversationId) => _pinnedConversationIds.Remove(pinnedConversationId);
    public bool UnblockConnection(string connectionId) => _blockedConnectionIds.Remove(connectionId);
    public bool BlockConnection(string connectionId) => _blockedConnectionIds.Add(connectionId);
}