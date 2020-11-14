using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dumplingram.API.SignalR
{
    public class PresenceTracker
    {
        private static readonly Dictionary<string, List<string>> OnlineUsers =
            new Dictionary<string, List<string>>();

        public Task UserConnected(string id, string connectionId)
        {
            //because this method is used by all users, they cannot access the dictionary at once
            lock (OnlineUsers)
            {
                if (OnlineUsers.ContainsKey(id))
                {
                    OnlineUsers[id].Add(connectionId);
                }
                else
                {
                    OnlineUsers.Add(id, new List<string> { connectionId });
                }
            }

            return Task.CompletedTask;
        }

        public Task UserDisconnected(string id, string connectionId)
        {
            lock (OnlineUsers)
            {
                if (!OnlineUsers.ContainsKey(id))
                    return Task.CompletedTask;

                OnlineUsers[id].Remove(connectionId);
                if (OnlineUsers[id].Count == 0)
                {
                    OnlineUsers.Remove(id);
                }
            }

            return Task.CompletedTask;
        }

        public Task<string[]> GetOnlineUsers()
        {
            string[] onlineUsers;

            lock (OnlineUsers)
            {
                onlineUsers = OnlineUsers.OrderBy(k => k.Key)
                    .Select(k => k.Key)
                    .ToArray();
            }

            return Task.FromResult(onlineUsers);
        }

        public Task<List<string>> GetConnectionsForUser(string userId)
        {
            List<string> connectionIds;
            lock (OnlineUsers)
            {
                connectionIds = OnlineUsers.GetValueOrDefault(userId);
            }

            return Task.FromResult(connectionIds);
        }
    }
}