using RecipeSocialMediaAPI.Domain.Models.Users;
using System.Collections.Immutable;

namespace RecipeSocialMediaAPI.Domain.Models.Messaging;

public class Group
{
    private readonly HashSet<UserAccount> _users;

    public string GroupId { get; }

    public string GroupName { get; set; }

    public ImmutableList<UserAccount> Users { get => _users.ToImmutableList(); }

    public Group(string groupId, string groupName, IEnumerable<UserAccount>? users = null)
    {
        GroupId = groupId;
        GroupName = groupName;
        _users = users?.ToHashSet() ?? new HashSet<UserAccount>();
    }

    public bool AddUser(UserAccount user)
    {
        return _users.Add(user);
    }

    public bool RemoveUser(UserAccount user)
    {
        return _users.Remove(user);
    }
}
