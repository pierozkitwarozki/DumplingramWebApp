using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Dumplingram.API.Data;
using Dumplingram.API.Dtos;
using Dumplingram.API.Models;
using Microsoft.AspNetCore.SignalR;

namespace Dumplingram.API.SignalR
{
    public class MessageHub : Hub
    {
        private IDumplingramRepository _repo;
        private IMapper _mapper;
        private IHubContext<PresenceHub> _presenceHub;

        private PresenceTracker _presenceTracker;

        public MessageHub(IDumplingramRepository repo, IMapper mapper, IHubContext<PresenceHub> presenceHub, PresenceTracker presenceTracker)
        {
            _repo = repo;
            _mapper = mapper;
            _presenceHub = presenceHub;
            _presenceTracker = presenceTracker;
        }

        public override async Task OnConnectedAsync()
        {
            await RemoveFromGroup();
            string id = Context.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"].ToString();
            // rember to get other user id to string, not username as in course

            var groupName = GetGroupName(id, otherUser);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await AddToGroup(groupName);

            var messages =
                await _repo.GetMessageThread(int.Parse(id), int.Parse(otherUser));

            await Clients.Group(groupName).SendAsync("RecieveMessageThread", messages);
        }

        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            await RemoveFromGroup();
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            if(string.IsNullOrEmpty(createMessageDto.Content)) return;

            var userId = int.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var recipient = await _repo.GetUser(createMessageDto.RecipientId);

            if (recipient == null)
                throw new HubException("Użytkownik nie istnieje.");

            if (userId == recipient.ID)
                throw new HubException("Nie możesz wysłać wiadomości do samego siebie.");

            var sender = await _repo.GetUser(userId);

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.Username,
                RecipientUsername = recipient.Username,
                Content = createMessageDto.Content
            };


            var groupName = GetGroupName(userId.ToString(), recipient.ID.ToString());
            var group = await _repo.GetGroup(groupName);


            var connections = await _presenceTracker.GetConnectionsForUser(recipient.ID.ToString());
            var connectionsForSender = await _presenceTracker.GetConnectionsForUser(sender.ID.ToString());

            // so i know that both users is connected
            if (group.Connections.Any(x => x.UserId == recipient.ID.ToString()))
            {
                message.DateRead = DateTime.UtcNow;
            }
            else
            {
                //only notified when not in conversation
                if (connections != null)
                {
                    // for 'global' notification

                    await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived",
                    new
                    {
                        username = sender.Username
                    });
                }
            }

            await _repo.Add(message);

            if (await _repo.SaveAll())
            {
                await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));

                if (connections != null && connectionsForSender != null)
                {
                    // to refresh private conversation list

                    await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceivedNoNotification");
                    await _presenceHub.Clients.Clients(connectionsForSender).SendAsync("NewMessageReceivedNoNotification");
                }
            }
        }

        private string GetGroupName(string caller, string other)
        {
            // return conversation name
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}_{other}" : $"{other}_{caller}";
        }

        private async Task<bool> AddToGroup(string groupName)
        {
            string id = Context.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var group = await _repo.GetGroup(groupName);
            var connection = new Connection(Context.ConnectionId, id);

            if (group == null)
            {
                group = new Group(groupName);
                await _repo.Add(group);
            }

            group.Connections.Add(connection);

            return await _repo.SaveAll();
        }

        private async Task RemoveFromGroup()
        {
            var connections = await _repo.GetConnections(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            foreach (var connection in connections)
            {
                _repo.Delete(connection);
            }

            await _repo.SaveAll();
        }
    }
}