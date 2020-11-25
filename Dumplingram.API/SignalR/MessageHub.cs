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
        private IConnectionGroupRepository _connectionGroupRepo;
        private IMessageRepository _messageRepo;
        private IUserRepository _usersRepo;
        private IMapper _mapper;
        private IHubContext<PresenceHub> _presenceHub;

        private PresenceTracker _presenceTracker;

        public MessageHub(IConnectionGroupRepository connectionGroupRepo, IMapper mapper, IMessageRepository messageRepo,
            IUserRepository userRepo,
            IHubContext<PresenceHub> presenceHub, PresenceTracker presenceTracker)
        {
            _connectionGroupRepo = connectionGroupRepo;
            _mapper = mapper;
            _presenceHub = presenceHub;
            _presenceTracker = presenceTracker;
            _messageRepo = messageRepo;
            _usersRepo = userRepo;
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
                await _messageRepo.GetMessageThreadAsync(int.Parse(id), int.Parse(otherUser));

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
            var recipient = await _usersRepo.GetUserAsync(createMessageDto.RecipientId);

            if (recipient == null)
                throw new HubException("Użytkownik nie istnieje.");

            if (userId == recipient.ID)
                throw new HubException("Nie możesz wysłać wiadomości do samego siebie.");

            var sender = await _usersRepo.GetUserAsync(userId);

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.Username,
                RecipientUsername = recipient.Username,
                Content = createMessageDto.Content
            };


            var groupName = GetGroupName(userId.ToString(), recipient.ID.ToString());
            var group = await _connectionGroupRepo.GetGroupAsync(groupName);


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

            await _messageRepo.AddAsync(message);

            if (await _messageRepo.SaveAllAsync())
            {
                await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));

                if (connections != null) 
                {
                    // to refresh private conversation list

                    await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceivedNoNotification");
                    
                }

                if(connectionsForSender != null)
                {
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

            var group = await _connectionGroupRepo.GetGroupAsync(groupName);
            var connection = new Connection(Context.ConnectionId, id);

            if (group == null)
            {
                group = new Group(groupName);
                await _connectionGroupRepo.AddAsync(group);
            }

            group.Connections.Add(connection);

            return await _connectionGroupRepo.SaveAllAsync();
        }

        private async Task RemoveFromGroup()
        {
            var connections = await _connectionGroupRepo.GetConnectionsAsync(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            foreach (var connection in connections)
            {
                _connectionGroupRepo.Delete(connection);
            }

            await _connectionGroupRepo.SaveAllAsync();
        }
    }
}