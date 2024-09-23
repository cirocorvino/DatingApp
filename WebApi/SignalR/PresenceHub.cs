using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using WebApi.Extensions;

namespace WebApi.SignalR;

[Authorize]
public class PresenceHub(PresenceTracker tracker) : Hub
{
    public override async Task OnConnectedAsync()
    {
        if (Context.User == null) throw new Exception("Cannot get current user claim");

        await tracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);
        await Clients.Others.SendAsync("UserIsOnLine", Context.User?.GetUsername());

        var currentUsers = await tracker.GetOnlineUsers();
        await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (Context.User == null) throw new Exception("Cannot get current user claim");

        await tracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);
        await Clients.Others.SendAsync("UserIsOffline", Context.User?.GetUsername());

        var currentUsers = await tracker.GetOnlineUsers();
        await Clients.All.SendAsync("GetOnlineUsers", currentUsers);

        await base.OnDisconnectedAsync(exception);
    }
}
