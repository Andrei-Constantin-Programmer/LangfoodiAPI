namespace RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;

public enum ConnectionStatus
{
    Blocked = -2,
    Muted = -1,

    Pending = 0,

    Connected = 1,
    Favourite = 2,
}