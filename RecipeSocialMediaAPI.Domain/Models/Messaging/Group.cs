using RecipeSocialMediaAPI.Domain.Models.Users;
using System.Collections.Immutable;

namespace RecipeSocialMediaAPI.Domain.Models.Messaging;

public class Group
{
    private readonly HashSet<IUserAccount> _users;

    public string GroupId { get; }

    public string GroupName { get; set; }

    public string GroupDescription { get; set; }

    public ImmutableList<IUserAccount> Users { get => _users.ToImmutableList(); }

    public Group(string groupId, string groupName, string groupDescription, IEnumerable<IUserAccount>? users = null)
    {
        GroupId = groupId;
        GroupName = groupName;
        GroupDescription = groupDescription;
        _users = users?.ToHashSet() ?? new HashSet<IUserAccount>();
    }

    public bool AddUser(IUserAccount user)
    {
        return _users.Add(user);
    }

    public bool RemoveUser(IUserAccount user)
    {
        return _users.Remove(user);
    }
}
