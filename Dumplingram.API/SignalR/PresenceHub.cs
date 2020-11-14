using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Dumplingram.API.SignalR
{
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _tracker;
        public PresenceHub(PresenceTracker tracker)
        {
            _tracker = tracker;

        }
        public override async Task OnConnectedAsync()
        {
            string id = Context.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            await _tracker.UserConnected(id, Context.ConnectionId);

            await Clients.Others.SendAsync("UserIsOnline", id);

            var currentUsers = await _tracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string id = Context.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            await _tracker.UserDisconnected(id, Context.ConnectionId);
            await Clients.Others.SendAsync("UserIsOffline", id);

            var currentUsers = await _tracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
            
            await base.OnDisconnectedAsync(exception);
        }




    }
}