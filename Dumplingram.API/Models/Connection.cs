namespace Dumplingram.API.Models
{
    public class Connection
    {
        public string ConnectionId { get; set; }
        public string UserId { get; set; }

        public Connection(string connectionId, string userId)
        {
            ConnectionId = connectionId;
            UserId = userId;
        }

        public Connection() {}
    }
}